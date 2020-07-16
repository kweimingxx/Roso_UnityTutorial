using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoidTarget : MonoBehaviour {
	public float ZeroDistance = 8f;

	void OnTriggerEnter(Collider other) {
		Boid boid = other.GetComponentInParent<Boid>();
		boid.Target = this;
	}

	void OnTriggerExit(Collider other) {
		Boid boid = other.GetComponentInParent<Boid>();
		if (boid.Target == this) {
			boid.Target = null;
		}
	}
}
