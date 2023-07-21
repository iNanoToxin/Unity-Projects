using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour {
	
	[SerializeField]
	private ComputeShader computeShader;
	private const int _threadGroupSize = 1024;

	[SerializeField]
	private BoidSettings boidSettings;
	private Boid[] _boids;
	private BoidData[] _boidData;
	
	private Camera _camera;
	private Vector2 _lBound; // Left boundary
	private Vector2 _rBound; // Right boundary
	private Vector2 _tBound; // Top boundary
	private Vector2 _bBound; // Bottom boundary
	
	[SerializeField]
	private GameObject boidPrefab;

	private void SetBounds() {
		_lBound = _camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, _camera.nearClipPlane));
		_rBound = _camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, _camera.nearClipPlane));
		_tBound = _camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, _camera.nearClipPlane));
		_bBound = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, _camera.nearClipPlane));
	}

	private void Start() {
		_boids = new Boid[boidSettings.boidCount];
		_boidData = new BoidData[boidSettings.boidCount];

		_camera = Camera.main;
		
		SetBounds();

		for (int i = 0; i < boidSettings.boidCount; i++) {
			float x = Random.Range(_lBound.x, _rBound.x);
			float y = Random.Range(_bBound.y, _tBound.y);

			_boids[i] = new Boid(boidSettings, Instantiate(boidPrefab)) {
				position = new Vector2(x, y),
				velocity = Random.insideUnitCircle * boidSettings.maxSpeed
			};

			_boids[i].SetBounds(_lBound, _rBound, _tBound, _bBound);
			_boidData[i] = new BoidData();
		}
	}
	
	private void Update() {
		ComputeBuffer computeBuffer = new ComputeBuffer(boidSettings.boidCount, BoidData.size);

		for (int i = 0; i < _boids.Length; i++) {
			_boidData[i].position = _boids[i].position;
			_boidData[i].velocity = _boids[i].velocity;

			_boidData[i].flockHeading = Vector2.zero;
			_boidData[i].flockCenter = Vector2.zero;
			_boidData[i].seperationHeading = Vector2.zero;
			_boidData[i].numFlockmates = 0;
		}
		computeBuffer.SetData(_boidData);
		
		computeShader.SetBuffer(0, "boidsBuffer", computeBuffer);
		computeShader.SetFloat("squarePerceptionRadius", Mathf.Sign(boidSettings.perceptionRadius) * boidSettings.perceptionRadius * boidSettings.perceptionRadius);
		computeShader.SetFloat("squareAvoidanceRadius", Mathf.Sign(boidSettings.avoidanceRadius) * boidSettings.avoidanceRadius * boidSettings.avoidanceRadius);
		computeShader.SetInt("numBoids", _boids.Length);
		computeShader.Dispatch(0, Mathf.CeilToInt(_boids.Length / (float) _threadGroupSize), 1, 1);
		
		computeBuffer.GetData(_boidData);
		computeBuffer.Release();
		
		SetBounds();

		for (int i = 0; i < _boids.Length; i++) {
			_boids[i].SetBounds(_lBound, _rBound, _tBound, _bBound);
			_boids[i].Update(_boidData[i]);
			_boids[i].Render();
		}
	}
}

public struct BoidData {
	public Vector2 position;
	public Vector2 velocity;

	public Vector2 flockHeading;
	public Vector2 flockCenter;
	public Vector2 seperationHeading;
	public int numFlockmates;

	public static int size => sizeof(float) * 2 * 5 + sizeof(int);
};