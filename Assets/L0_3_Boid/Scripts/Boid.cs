using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
	#if DEBUG
		public bool LogActions;
	#endif

	private Rigidbody _rigidbody;
	[SerializeField] private BoidBound _awarenessBound;
	[SerializeField] private BoidBound _separationBound;

	private HashSet<Boid> _flock = new HashSet<Boid>();
	private HashSet<Boid> _tooClose = new HashSet<Boid>();
	private HashSet<Boundary> _boundaries = new HashSet<Boundary>();
	[SerializeField] private Boundary _previousBoundary;

	[SerializeField] private float _fov = 180.0f;
	[SerializeField] private float _turnSensitivity = 2.0f;

	public BoidTarget Target;

	void Awake() {
		_rigidbody = this.GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		Vector3 nextForward, toCohesion, toAlignment, toSeparation, toTarget, toBoundary;
		nextForward = toCohesion = toAlignment = toSeparation = toTarget = toBoundary = Vector3.zero;

		// Do cohesion, alignment, and target only if within bounds
		if (!IsOutsideBoundary()) {
			toCohesion = CalcTowardsCohesion();
			toAlignment = CalcTowardsAlignment();

			if (HasTarget()) {
				toTarget = CalcTowardsTarget();
			}
		} else {
			toBoundary = CalcIntoBoundaries();
		}

		toSeparation = CalcTowardsSeparation();

		#if DEBUG // This macro is set in the project's Player settings
			toCohesion *= BoidsManager.Cohesion;
			toAlignment *= BoidsManager.Alignment;
			toSeparation *= BoidsManager.Separation;
			toTarget *= BoidsManager.Target;
		#endif

		nextForward += toCohesion + toAlignment + toBoundary + toTarget + toSeparation;
		nextForward.Normalize();

		// We now have the direction we want our boid to be facing
		// Use slerp to do a smooth transition
		Vector3 slerpedForward = Vector3.Slerp(this.transform.forward, nextForward, Time.deltaTime*_turnSensitivity);
		// Vector3 slerpedForward = nextForward;

		// If no other boids are nearby, then calculations aren't preformed
		// resulting in a zero vector
		// In that case, just use the previous forward vector
		if (slerpedForward != Vector3.zero) {
			this.transform.forward = slerpedForward;
		}

		// The boid's direction has been updated, now lets move it forward
		float speed = 20;
		#if DEBUG
			speed = BoidsManager.Speed*20;
		#endif

		// Move the boid forward (uses rigidbody physics, see commented below for no collisions)
		Vector3 nextPos = transform.forward*speed;
		_rigidbody.velocity = nextPos;

		// If not using boid collisions: uncomment below line, disable capsule collider, enable `Is Kinematic` on rigidbody, comment out rigidbody velocity update above
		// this.transform.position = nextPos;

		#if DEBUG
			if (this.LogActions) {
				Debug.Log("C: " + toCohesion + ", A: " + toAlignment + ", S: " + toSeparation + ", T: " + toTarget + ", B: " + toBoundary + "\nnextPos: " + nextPos);
			}
		#endif
	}

	private Vector3 CalcTowardsCohesion() {
		Vector3 centerOfMass = Vector3.zero;

		// There's no point in doing the calculation if there are no nearby boids
		if (_flock.Count == 0) {
			return centerOfMass;
		}

		foreach (Boid boid in _flock) {
			centerOfMass += boid.transform.position;
		}
		centerOfMass /= _flock.Count;
		Vector3 cohesion = centerOfMass - this.transform.position;
		cohesion.Normalize();

		return cohesion;
	}

	private Vector3 CalcTowardsAlignment() {
		Vector3 alignment = Vector3.zero;

		if (_flock.Count == 0) {
			return alignment;
		}

		foreach (Boid boid in _flock) {
			alignment += boid.transform.forward;
		}
		alignment /= _flock.Count;
		alignment.Normalize(); // Unnecessary since the average will be a unit vector

		return alignment;
	}

	private Vector3 CalcTowardsSeparation() {
		Vector3 separation = Vector3.zero;

		if (_tooClose.Count == 0) {
			return separation;
		}

		foreach (Boid boid in _tooClose) {
			Vector3 separate = this.transform.position - boid.transform.position;
			float sqrDistance = separate.sqrMagnitude;
			sqrDistance = Mathf.Max(sqrDistance, 0.001f);
			float distMultiplier = 1/sqrDistance;
			separation += separate * distMultiplier;
		}

		separation /= _tooClose.Count;

		return separation;
	}

	private Vector3 CalcTowardsTarget() {
		Vector3 toTarget = this.Target.transform.position - this.transform.position;
		// Within a specified distance, disable the target's effect
		// Prevents boids from forming a clump when colliding head-on
		// on opposing sides of the target
		if (Vector3.Magnitude(toTarget) <= this.Target.ZeroDistance) {
			return Vector3.zero;
		}
		toTarget.Normalize();
		return toTarget;
	}

	private Vector3 CalcIntoBoundaries() {
		Vector3 lastBound = _previousBoundary.transform.position;
		Vector3 toBound = lastBound - this.transform.position;

		return toBound;
	}

	public void AddBoidToFlock(Boid joining) {
		if (_flock.Count >= BoidsManager.MaxFlockSize) {
			return;
		}

		Vector3 toJoining = joining.transform.position - this.transform.position;

		// The perceived flock is limited to a specified FOV (default 180)
		// Potential optimization to JUST cross product and/or dot product values?
		if (Vector3.Angle(this.transform.forward, toJoining) < _fov/2.0f) {
			_flock.Add(joining);
		}
	}

	public void RemoveBoidFromFlock(Boid exiting) {
		// It's possible the exiting boid wasn't actually added to the flock
		// because when it entered, it was behind this boid (not within the FOV)
		// In that case, trying to remove it does nothing
		_flock.Remove(exiting);
	}

	public void AddBoidToTooClose(Boid near) {
		if (_tooClose.Count >= BoidsManager.MaxFlockSize) {
			return;
		}

		_tooClose.Add(near);
	}

	public void RemoveBoidFromTooClose(Boid far) {
		_tooClose.Remove(far);
	}

	public void EnterBoundary(Boundary boundary) {
		_boundaries.Add(boundary);
		_previousBoundary = boundary;
	}

	public void ExitBoundary(Boundary boundary) {
		_boundaries.Remove(boundary);
	}

	private bool IsOutsideBoundary() {
		return _boundaries.Count == 0;
	}

	private bool HasTarget() {
		return this.Target != null;
	}
}
