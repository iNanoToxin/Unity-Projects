using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid {
	
	public Vector2 position;
	public Vector2 velocity;
	
	private readonly BoidSettings _boidSettings;
	private Vector2 _lBound; // Left boundary
	private Vector2 _rBound; // Right boundary
	private Vector2 _tBound; // Top boundary
	private Vector2 _bBound; // Bottom boundary
	private readonly Transform _boidTransform;
	
	public Boid(BoidSettings boidSettings, GameObject boid) {
		_boidSettings = boidSettings;
		_boidTransform = boid.transform;
	}

	public void SetBounds(Vector2 lBound, Vector2 rBound, Vector2 tBound, Vector2 bBound) {
		_lBound = lBound;
		_rBound = rBound;
		_tBound = tBound;
		_bBound = bBound;
	}

	public void Render() {
		_boidTransform.position = position;
		_boidTransform.localScale = new Vector3(30f, 50f, 30f) * _boidSettings.boidScaleFactor;
		
		if (velocity != Vector2.zero) {
			float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			_boidTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
		}
	}

	public void Update(BoidData boidData) {
		Vector2 acceleration = Vector2.zero;

		if (boidData.numFlockmates > 0) {
			boidData.flockCenter /= boidData.numFlockmates;
			boidData.flockCenter -= position;

			acceleration += SteerTowards(boidData.flockHeading) * _boidSettings.alignmentWeight;
			acceleration += SteerTowards(boidData.flockCenter) * _boidSettings.cohesionWeight;
			acceleration += SteerTowards(boidData.seperationHeading) * _boidSettings.seperationWeight;
		}
		
		velocity += acceleration * Time.deltaTime;

		float speed = Mathf.Clamp(velocity.magnitude, _boidSettings.minSpeed, _boidSettings.maxSpeed);
		
		velocity = velocity.normalized * speed;
		position += velocity * Time.deltaTime;
		
		float marginX = (_rBound.x - _lBound.x) * _boidSettings.marginScale;
		float marginY = (_tBound.y - _bBound.y) * _boidSettings.marginScale;
		
		if (position.x < _lBound.x + marginX) velocity.x += 1;
		if (position.x > _rBound.x - marginX) velocity.x -= 1;
		
		if (position.y < _bBound.y + marginY) velocity.y += 1;
		if (position.y > _tBound.y - marginY) velocity.y -= 1;
	}

	private Vector2 SteerTowards(Vector2 direction) {
		Vector2 v = direction.normalized * _boidSettings.maxSpeed - velocity;
		return Vector2.ClampMagnitude(v, _boidSettings.maxSteerForce);
	}
}