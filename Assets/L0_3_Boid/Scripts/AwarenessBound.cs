using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AwarenessBound : BoidBound {

	void OnTriggerEnter(Collider other) {
		Boid otherBoid = other.GetComponentInParent<Boid>();
		this.Boid.AddBoidToFlock(otherBoid);
	}

	void OnTriggerExit(Collider other) {
		Boid otherBoid = other.GetComponentInParent<Boid>();
		this.Boid.RemoveBoidFromFlock(otherBoid);
	}
}
