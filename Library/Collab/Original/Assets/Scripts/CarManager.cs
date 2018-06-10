using System;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;


public class CarManager : MonoBehaviour
{
	/*This expects <actionNum> q values from the neural network.
		Each index corresponds to a specific legal action determined by
		mapOutputsToActions at the end of the script. With current parameters
		turningForce = 0.5, TURN_SPEED = 3000 rotation is multiplied by 6 degrees
		quaternation at ApplyForces. 
	supposedly regular move
	private const float MAX_VEL = 10f;
	private const float ACCELERATION = 500f;
	private const float VEL_FRICT = 1f;
	private const float TURN_SPEED = 3000;

	to observe the rotations of raycasts
	private const float MAX_VEL = 1f;
	private const float ACCELERATION = 50f;
	private const float VEL_FRICT = 1f;
	private const float TURN_SPEED = 3000;
		
	*/

//		//What the box will do if the event fits it (will always fit but you will probably change that on your own)
//		public override void OnNotify()
//		{
//			
//		}

	/*Movement constants*/
	private const  float MAX_VEL = 10f;
	private const  float ACCELERATION = 200f;
	private const  float VEL_FRICT = 0.1f;
	private const  float TURN_SPEED = 800;
	private const  int MEM_LIMIT = 2000;
	private const  double PENALTY = -10.0;
	private const  float engineConst = 0.4f;
	private const  float turningConst = 0.15f;
	private const  float brakeConst = 0.1f;


	private float epsilon = 1.0f;
	private float epsilon_decay = 0.995f;
	private float epsilon_min = 0.1f;
	private int targetUpdateInterval = 4;
	private const int batch = 40;
	private double gamma  = 0.95;




	private int episodes = 1;
	public  int T=0;
	private int counter = 0;
	private const int reflexModulus = 2;
	private const int feedbackModulus = 250;


	/*Variables for raycasts*/
	public Rigidbody rb;
	public RaycastHit[] hits;
	public Vector3[] angles;
	public double[] distances;
	public float thrust;
	public static int nsensor =  11; // Number of sensors
	public static int hiddenSize = 10;
	public static int state_size = nsensor + 1; //appended speed
	public  static float maxdist = 10; // Maximum distance which a sensor can measure
//		public  static float stepangle = Mathf.PI / nsensor;


	/*Variables for movements*/
//		private double[] forces = new double[2];
	private double engineForce, turningForce;
	public MLP Qnetwork;
	public MLP QtargetNetwork;
//	public NetworkFeedback feedback;
	public History history;
	public Memory memory;
	public event System.Action HitWall;
	public System.Random rnd;
	private static int actionNum = 9;
	public static int[] hiddenSizes = {10, actionNum};

	public float Velocity
	{
		get;
		private set;
	}

	public Quaternion Rotation
	{
		get;
		private set;
	}
		

	public int ActionIndex
	{
		get;
		private set;
	}

	void Awake()
	{
		rnd =  new System.Random(GetInstanceID());
		Qnetwork = new MLP(state_size, hiddenSizes);
		QtargetNetwork = new MLP(state_size, hiddenSizes);
		Qnetwork.copyTo (QtargetNetwork);
		history = new History (feedbackModulus, state_size);
		memory = new Memory(MEM_LIMIT, rnd, state_size);
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		hits = new RaycastHit[nsensor];
		distances = new double[nsensor];
		this.counter = 0;
		HitWall += Die;
	}

	void FixedUpdate ()
	{

		if (counter % reflexModulus == 0) {

			List<double> current_dist = MeasureDistances(); // distances

			int mid_sensor = (int)nsensor / 2;

			double avg_dist_ahead = (current_dist[mid_sensor-1] + current_dist[mid_sensor] + current_dist[mid_sensor+1])/3;
			avg_dist_ahead += current_dist.Min () * 0.40;

			double current_speed = this.Velocity/MAX_VEL; // states

			current_dist.Add(current_speed); // state

			Matrix<double> state = Matrix<double>.Build.DenseOfColumnArrays (current_dist.ToArray ());

			Vector<double> Qhat = Qnetwork.Forward(state).Column(0); //get q value predictions for current state  

			int action = EpsGreedy(Qhat);

			memory.Enqueue (state, action, current_speed, avg_dist_ahead);

			takeAction (action);

			T += 1;
//			Debug.Log ("t: " + T);

			if (T % feedbackModulus == 0) {
				epsilon = epsilon > epsilon_min ? epsilon * epsilon_decay : epsilon_min;
				foreach (Entry e in memory.Sample(batch)){
					Vector<double> qhat = Qnetwork.Forward (e.state).Column(0); 
					Vector<double> qphat = QtargetNetwork.Forward(e.nextState).Column(0);
					double qtarget = e.crash ? PENALTY : e.reward + gamma * qphat [qhat.MaximumIndex()];	

					Matrix<double>[] gs = Qnetwork.Gradients (e.state, e.action, qtarget);
					Qnetwork.update (gs);
					if (Qnetwork.t % targetUpdateInterval == 0) {
						Qnetwork.copyTo (QtargetNetwork);
					}
				}
			}

//			if (T % feedbackModulus == 0) {
//
//				Matrix<double>[] states = history.getAllStates (); 
//				double[] qtargets = history.getAllQTargets ();
//				int[] actions = history.getAllActions ();
//
//				for (var i = 0; i < qtargets.Count(); i++) {
//					if (rnd.NextDouble () < batch / qtargets.Count ()) {
//						Matrix<double> current_state = states [i];
//						double qtarget = qtargets [i];
//						int a = actions [i];
//						Matrix<double>[] gs = Qnetwork.Gradients (current_state, a, qtarget);
//						Qnetwork.update (gs);
//						if (Qnetwork.t % targetUpdateInterval == 0) {
//							Qnetwork.copyTo (QtargetNetwork);
//						}
//					}
//				}
//			}
		}

		ApplyRotation (); /* Updates Rotation */

		ApplyVelocity (); /* Updates position */

		ApplyFriction (); 

		counter ++;
	}


	private void takeAction(int action){
		UpdateForces (action);
		UpdateVelocity(); /* Updates Velocity */
	}
		

//	private void UpdateNetworkOnCollision()
//	{
//		epsilon = epsilon > epsilon_min ? epsilon * epsilon_decay : epsilon_min;
//		Matrix<double>[] states = history.getAllStates (); 
//		double[] qtargets = history.getAllQTargets ();
//		Debug.Log("qtargets: " + qtargets[qtargets.Length-1]);
//		int[] actions = history.getAllActions ();
//		for (var i = 0; i < qtargets.Count(); i++) {
//			if (rnd.NextDouble () < batch / qtargets.Count ()) {
//				Matrix<double> current_state = states [i];
//				double qtarget = qtargets [i];
//				int a = actions [i];
//
//				Matrix<double>[] gs = Qnetwork.Gradients (current_state, a, qtarget);
//				Qnetwork.update (gs);
//
//				if (Qnetwork.t % targetUpdateInterval == 0) {
//					Qnetwork.copyTo (QtargetNetwork);
//				}
//			}
//		}
//	}


	private int EpsGreedy(Vector<double> Qhat){
		return rnd.NextDouble() < epsilon ? rnd.Next(0,actionNum) : Qhat.MaximumIndex ();
	}

	private void UpdateVelocity()
	{
		/*Cap inputs*/
		if (engineForce > 1)
			engineForce = 1;
		else if (engineForce < -1)
			engineForce = -1;

		if (turningForce > 1)
			turningForce = 1;
		else if (turningForce < -1)
			turningForce = -1;

		//print (this.gameObject.name + "'s engine Force: " + engineForce);
		//print (this.gameObject.name + "'s turning Force: " + turningForce);

		/*Can only accelerate if current velocity is smaller than 
		engineForce times maximum velocity. Same logic applies to the
		negative engine force case*/
		bool canAccelerate = true;
		if (engineForce > 0)
			canAccelerate = Velocity < engineForce * MAX_VEL;

		/*Update Velocity*/
		if (canAccelerate) 
		{
			Velocity += (float)engineForce * (float)ACCELERATION * Time.deltaTime;
			if (Velocity > MAX_VEL)
				Velocity = MAX_VEL;
			else if (Velocity < 0)
				Velocity = 0f;
		}
	}


	private void ApplyVelocity()
	{
		Vector3 direction = new Vector3(0, 0, 1);
		transform.rotation = Rotation;
		direction = Rotation * direction;
		this.transform.position += direction * Velocity * Time.deltaTime;
	}

	private void ApplyRotation()
	{
		if (turningForce > 1)
			turningForce = 1;
		else if (turningForce < -1)
			turningForce = -1;
		
		/*Update rotation*/
		Rotation = transform.rotation;
		Rotation *=  Quaternion.AngleAxis((float) - turningForce * TURN_SPEED * Time.deltaTime, Vector3.up);
	}

	private void ApplyFriction()
	{
		if (engineForce == 0)
		{
			Velocity -= VEL_FRICT * Time.deltaTime;
			if (Velocity < 0)
				Velocity = 0;
		}
	}

	// Unity method, triggered when collision was detected.
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name.Substring (0, 4) == "Cube") {
			memory.RecordCrash ();
			if (HitWall != null)
				HitWall();
		}
		Debug.Log ("Episode " + episodes);
		episodes++;
	}

	public void Stop()
	{
		Velocity = 0;
		Rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
	}

	private void Die()
	{
		this.enabled = false;
		Stop();
	}

	public bool isEnabled() {
		return this.enabled;
	}

	public void Restart(Vector3 position, Quaternion rotation)
	{
		this.enabled = true;
		this.transform.rotation = rotation;
		this.transform.position = position;
		this.Velocity = 0f;
		this.counter = 0;
		this.engineForce = 0f;
		this.turningForce = 0f;
		this.history.reset ();

	}

	private void UpdateForces(int action)
	{
		switch (action) 
		{
		case 0: //no action
			engineForce = 0f;
			turningForce = 0f;
			break;
		case 1: //right accel
			engineForce = engineConst;
			turningForce = -turningConst;
			break;
		case 2: //left accel
			engineForce = engineConst;
			turningForce = turningConst;
			break;
		case 3: //forward accel
			engineForce = engineConst;
			turningForce = 0f;
			break;
		case 4: //right brake
			engineForce = -brakeConst;
			turningForce = -turningConst;
			break;
		case 5: //left brake
			engineForce = -brakeConst;
			turningForce = turningConst;
			break;
		case 6: //forward brake
			engineForce = -brakeConst;
			turningForce = 0f;
			break;
		case 7: //right
			engineForce = 0f;
			turningForce = -turningConst;
			break;
		case 8: //left
			engineForce = 0f;
			turningForce = turningConst;
			break;
		default:
			break;

		}
	}

	private Vector3[] getRayDirections(int nsensor) 
	{
		Vector3 vf = transform.forward;
		Vector3 vr = transform.right;
		Vector3 vl = -vr;

		Vector3[] arr = new Vector3[nsensor];

		int halfnum = (int)(nsensor / 2);

		for (int i = 0; i < nsensor; i++) {
			Vector3 v;

			if (i < (int)(nsensor / 2)) {

				v = vf * ((float)i / halfnum) + vl * ((float)(halfnum - i) / halfnum);

			} else {

				v = vr * ((float)(i-halfnum) / halfnum) + vf * ((float)(2 * halfnum - i) / halfnum);
			}
			arr [i] = v;
		}

		return arr;
	}


	private List<double> MeasureDistances()
	{
		Vector3[] rays = getRayDirections(nsensor);

		for (int i = 0; i < nsensor; i++)
		{

			Debug.DrawRay(transform.position, rays[i], Color.red);

			if (Physics.Raycast(transform.position, rays[i], out hits[i], maxdist))
			{
				distances [i] = (double)hits[i].distance;
			}else{
				distances [i] = (double)maxdist;
			}
		}
		return  Vector<double>.Build.DenseOfArray (distances).Divide (maxdist).ToList ();
	}

	public int getEpisode() 
	{
		return this.episodes;
	}
		

}
