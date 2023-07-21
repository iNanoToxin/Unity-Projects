using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Boids/Settings")]
public class BoidSettings : ScriptableObject {
	[Header("Count")]
	public int boidCount = 25;
	
	[Header("Speed")]
	public float minSpeed = 2f;
	public float maxSpeed = 5f;
	public float maxSteerForce = 3f;
	
	[Header("Weights")]
	public float alignmentWeight = 1f;
	public float seperationWeight = 1f;
	public float cohesionWeight = 1f;
	
	[Header("Visual")]
	public float perceptionRadius = 2.5f;
	public float avoidanceRadius = 1f;
	public float boidScaleFactor = 1f;
	[Range(0f, 1f)]
	public float marginScale = 0.05f;
}