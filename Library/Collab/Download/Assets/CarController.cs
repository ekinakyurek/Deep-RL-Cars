using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarController : MonoBehaviour {
	
	public float thrust;
//	public Rigidbody rb;
//	System.Random r = new Random();
	#region Members
//	#region IDGenerator
	// Used for unique ID generation
	private static int idGenerator = 0;
	private float timeSinceLastCheckpoint;
	/// <summary>
	/// Returns the next unique id in the sequence.
	/// </summary>
	private static int NextID
	{
		get { return idGenerator++; }
	}
	#endregion

	// Maximum delay in seconds between the collection of two checkpoints until this car dies.
	private const float MAX_CHECKPOINT_DELAY = 7;

//	public Agent Agent
//	{
//		get;
//		set;
//	}

//	public float CurrentCompletionReward
//	{
//		get { return Agent.Genotype.Evaluation; }
//		set { Agent.Genotype.Evaluation = value; }
//	}

	public bool UseUserInput = false;

	// another Start() below
//	void Start()
//	{
//		rb = GetComponent<Rigidbody>();
//	}

	public CarMovement Movement
	{
		get;
		private set;
	}

	public double[] CurrentControlInputs
	{
		get { return Movement.CurrentInputs; }
	}

//	public SpriteRenderer SpriteRenderer
//	{
//		get;
//		private set;
//	}


	void Awake()
	{
		//Cache components
		Movement = GetComponent<CarMovement>();
//		SpriteRenderer = GetComponent<SpriteRenderer>();
//		sensors = GetComponentsInChildren<Sensor>();
	}

	void Start()
	{
		Movement.HitWall += Die;

		//Set name to be unique
		this.name = "Car (" + NextID + ")";
	}

	public void Restart()
	{
		Movement.enabled = true;
		timeSinceLastCheckpoint = 0;

//		foreach (Sensor s in sensors)
//			s.Show();
//
//		Agent.Reset();
		this.enabled = true;
	}


	void FixedUpdate()
	{
		//Get control inputs from Agent
		if (!UseUserInput)
		{
			//Get readings from sensors
//			double[] sensorOutput = new double[sensors.Length];
//			for (int i = 0; i < sensors.Length; i++)
//				sensorOutput[i] = sensors[i].Output;

			double[] controlInputs = {UnityEngine.Random.Range(-1.0f,1.0f), 0.001f};//=buraya manual Input giricez //Agent.FNN.ProcessInputs(sensorOutput);
//			print(" Rand: " + Random.Range(0.0f,1.0f));
			Movement.SetInputs(controlInputs);
		}

//		if (timeSinceLastCheckpoint > MAX_CHECKPOINT_DELAY)
//		{
//			Die();
//		}
	}

	void OnCollisionEnter(Collision col)
	{
		print ("collided: " + col.gameObject.name);
		if (String.Compare(col.gameObject.name.Substring (0, 3),"Cube",true) == 0) 
		{
			Die ();

		}
	}

	void Update(){

		timeSinceLastCheckpoint += Time.deltaTime;
//		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
//		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
//
//		transform.Rotate(0, x, 0);
//		transform.Translate(0, 0, z);
	}

	// Makes this car die (making it unmovable and stops the Agent from calculating the controls for the car).
	private void Die()
	{
		this.enabled = false;
		Movement.Stop();
		Movement.enabled = false;

//		foreach (Sensor s in sensors)
//			s.Hide();

//		Agent.Kill();
	}

	public void CheckpointCaptured()
	{
		timeSinceLastCheckpoint = 0;
	}
}
