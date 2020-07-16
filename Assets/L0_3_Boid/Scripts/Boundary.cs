using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Boundary : MonoBehaviour {

	[SerializeField] private Collider Bound;

	void Start() {

	}

	void OnTriggerEnter(Collider other) {
		Boid boid = other.GetComponentInParent<Boid>();
		boid.EnterBoundary(this);
	}

	void OnTriggerExit(Collider other) {
		Boid boid = other.GetComponentInParent<Boid>();
		boid.ExitBoundary(this);
	}
}
