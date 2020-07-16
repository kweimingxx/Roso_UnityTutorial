using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BoidBound : MonoBehaviour {

	void Start() {}
	[SerializeField] protected Boid Boid;
	[SerializeField] protected Collider Bound;
}
