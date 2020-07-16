using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SeparationBound : BoidBound {

	void OnTriggerEnter(Collider other) {
		Boid otherBoid = other.GetComponentInParent<Boid>();
		if (otherBoid != null) {
			this.Boid.AddBoidToTooClose(otherBoid);
		}
	}

	void OnTriggerExit(Collider other) {
		Boid otherBoid = other.GetComponentInParent<Boid>();
		if (otherBoid != null) {
			this.Boid.RemoveBoidFromTooClose(otherBoid);
		}
	}
}
