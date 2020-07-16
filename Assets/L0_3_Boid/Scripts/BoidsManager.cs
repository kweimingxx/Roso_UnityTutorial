using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : UnitySingletonPersistent<BoidsManager> {

	[Range(0, 20)] [SerializeField] private int _maxFlockSize = 5;
	[Range(0, 5)] [SerializeField] private float _speed = 0.7f;
	[Range(0, 10)] [SerializeField] private float _cohesion = 1;
	[Range(0, 10)] [SerializeField] private float _alignment = 1;
	[Range(0, 10)] [SerializeField] private float _separation = 3;
	[Range(0, 10)] [SerializeField] private float _target = 2;

	public static int MaxFlockSize {
		get { return BoidsManager.Instance._maxFlockSize; }
		set { BoidsManager.Instance._maxFlockSize = value;}
	}

	public static float Speed {
		get { return BoidsManager.Instance._speed; }
		set { BoidsManager.Instance._speed = value;}
	}

	public static float Cohesion {
		get { return BoidsManager.Instance._cohesion; }
		set { BoidsManager.Instance._cohesion = value;}
	}

	public static float Alignment {
		get { return BoidsManager.Instance._alignment; }
		set { BoidsManager.Instance._alignment = value;}
	}

	public static float Separation {
		get { return BoidsManager.Instance._separation; }
		set { BoidsManager.Instance._separation = value;}
	}

	public static float Target {
		get { return BoidsManager.Instance._target; }
		set { BoidsManager.Instance._target = value;}
	}
}
