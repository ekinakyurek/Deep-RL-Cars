using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour2 : MonoBehaviour{


	public static int numcars = 4;
	public GameObject CarFab;
	public GameObject[] cars;
	public int episode = 0;
	public CarManager2 EpisodeController;
	public MLP Qnetwork;
	public MLP QtargetNetwork;
	public System.Random rng = new System.Random();
	public static int nsensor =  11; // Number of sensors
	public static int numobstacles = 3;
//	public static int state_size = nsensor + 1; //appended speed
	public static int d_queue_length = 3;
	public static int state_size = d_queue_length*(nsensor + 1); //appended speed, appended 2 entry from rotation vector //duplicate
//	private static int actionNum = 9;
	public static int[] hiddenSizes = {10,9};
//	public GameObject Obstacle;
//	public GameObject[] obstacles;
	private int car_inst = 1;

	////WARNING CHECK THE CUBE POSITIONS IN CARMANAGER2 WHEN CHANGING CAR NUMBER

	//public static Quaternion InitialRotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
	//public static Vector3 InitialPosition = new Vector3(-3.05f,0.1f,0.64f);
	//	public static Quaternion InitialRotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
	//	public static Vector3 InitialPosition = new Vector3 (-2.8f, 0.1f, 0.65f);


	//	public static Vector3[] Positions = {new Vector3 (-2.8f, 0.1f, 1.3f), new Vector3 (-2.8f, 0.1f, 0.29f), 
	//		new Vector3 (-30f, 0.1f, 3.49367f), new Vector3 (-31.5f, 0.1f, 3.49367f),
	//		new Vector3 (-21.77f, 0.1f, 6.46f), new Vector3 (-21.77f, 0.1f, 8.36f),
	//		new Vector3(-0.145386f, 0.1f,6.5f ), new Vector3(-1.145386f, 0.1f,8f )};
	//	
	//	public static Quaternion[] Rotations = { Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), 
	//		Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), Quaternion.AngleAxis(0, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(120, new Vector3(0, 1, 0)), Quaternion.AngleAxis(120, new Vector3(0, 1, 0)), 
	//		Quaternion.AngleAxis(90, new Vector3(0, 1, 0)), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))};

	//	public static Vector3[] Positions = {new Vector3 (-1.33f, 0.1f, 0.87f), 
	//		new Vector3 (-10.43f, 0.1f, 0.34f),
	//		new Vector3 (-19.47f, 0.1f, -2.13f), 
	//		new Vector3 (-28.98f, 0.1f,-0.59f ),
	//		new Vector3 (-31f, 0.1f, 3.49367f),
	//		new Vector3 (-21.77f, 0.1f, 6.46f),
	//		new Vector3 (-13.693f, 0.1f, 5.487f),
	//		new Vector3(-0.145386f, 0.1f,6.5f )};
	//
	//	public static Quaternion[] Rotations = { Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), 
	//		Quaternion.AngleAxis(240, new Vector3(0, 1, 0)), 
	//		Quaternion.AngleAxis(270, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(295, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(0, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(120, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(60, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(90, new Vector3(0, 1, 0))};

	public static Vector3[] Positions = { new Vector3 (-3.94f, 0.1f, 0.165f), 
		new Vector3 (-0.16f, 0.1f, 4.0185f),
		new Vector3 (3.8563f, 0.1f, 0.3039f), 
		new Vector3 (-0.18f, 0.1f, -3.66f)
	};
	//		new Vector3 (-31f, 0.1f, 3.49367f),
	//		new Vector3 (-21.77f, 0.1f, 6.46f),
	//		new Vector3 (-13.693f, 0.1f, 5.487f),
	//		new Vector3(-0.145386f, 0.1f,6.5f )};

	public static Quaternion[] Rotations = { Quaternion.AngleAxis (90, new Vector3 (0, 1, 0)),
		Quaternion.AngleAxis (180, new Vector3 (0, 1, 0)), 
		Quaternion.AngleAxis (270, new Vector3 (0, 1, 0)),
		Quaternion.AngleAxis (0, new Vector3 (0, 1, 0))
	};

	public static Quaternion[] Alt_Rotations = { Quaternion.AngleAxis (-90, new Vector3 (0, 1, 0)),
		Quaternion.AngleAxis (0, new Vector3 (0, 1, 0)), 
		Quaternion.AngleAxis (90, new Vector3 (0, 1, 0)),
		Quaternion.AngleAxis (180, new Vector3 (0, 1, 0))
	};
	//		Quaternion.AngleAxis(0, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(120, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(60, new Vector3(0, 1, 0)),
	//		Quaternion.AngleAxis(90, new Vector3(0, 1, 0))};

//	4.942 // distance to the center of the cube sqrt ile carpildi.

	public static Vector3[] OPositions = {new Vector3 (-31.566f, 0.1f, 13.79664f), 
		new Vector3 (-8.73f, 0.1f, 0.42f),
		new Vector3 (0.81f, 0.1f, 6.32f)
	};

	public static Quaternion[] ORotations = { Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
		Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
		Quaternion.AngleAxis(0, new Vector3(0, 1, 0))
	};


	//	public static Vector3[] MPositions = {new Vector3 (-2.8f, 0.1f, 1.3f), new Vector3 (-2.8f, 0.1f, -0.29f)};
	//	public static Quaternion[] MRotations = { Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), Quaternion.AngleAxis(270, new Vector3(0, 1, 0))};

	// Use this for initialization
	void Start () {
		//new NNTest ();
		Application.runInBackground = true;
		cars = new GameObject[numcars];
		Qnetwork = new MLP(state_size, hiddenSizes);
//		obstacles = new GameObject[numobstacles];
		QtargetNetwork = new MLP(state_size, hiddenSizes);
		int i = 0;
//		for (int i = 0; i < numcars; i++){
			//			int ind = rng.Next (0, Positions.Length);
			//			cars[i] = (GameObject) Instantiate(CarFab,Positions[ind],Rotations[ind]);
		double d = rng.NextDouble();
		cars[i] = (GameObject) Instantiate(CarFab,Positions[i], d > 0.5 ? Rotations[i] : Alt_Rotations[i]);
			CarManager2 m = cars[i].GetComponent(typeof(CarManager2)) as CarManager2;
			m.SetMLP (this.Qnetwork, this.QtargetNetwork);
			cars[i].name = "CloneCar" + i;
//		}
		EpisodeController = cars[0].GetComponent(typeof(CarManager2)) as CarManager2;
	}

	// Update is called once per frame
	void Update () {
		//The boxes should jump if the sphere is cose to origo
		//			if (AllDead())
		//			{

		for (int i = 0; i < car_inst; i++){
			CarManager2 m = cars[i].GetComponent(typeof(CarManager2)) as CarManager2;
				//					int ind = rng.Next (0, Positions.Length);
				//					m.Restart (Positions[ind],Rotations[ind]);
			double d = rng.NextDouble();
			if (!m.isEnabled()) m.Restart (Positions[i], d > 0.5 ? Rotations[i] : Alt_Rotations[i]);
		}
		//			}
		episode = EpisodeController.getEpisode ();
	}
	private GUIStyle guiStyle = new GUIStyle(); //create a new variable
	void OnGUI() {

		guiStyle.fontSize = 48; //change the font size
		GUI.Label (new Rect (10, 10, 100, 200), "Episode: " + episode, guiStyle);
		if (GUI.Button (new Rect (10, 100, 50, 30), "Save")) {
			Debug.Log ("SAVED");
			this.QtargetNetwork.write ("QTargetNetwork");
			this.Qnetwork.write ("QNetwork");
		}

		if (GUI.Button (new Rect (10, 140, 50, 30), "Load")) {
			Debug.Log ("LOADED");
			this.QtargetNetwork.read ("QTargetNetwork");
			this.Qnetwork.read ("QNetwork");

			for (int i = 0; i < car_inst; i++){
				CarManager2 m = cars[i].GetComponent(typeof(CarManager2)) as CarManager2;
				m.epsilon = m.epsilon_min;
				Debug.Log ("Epsilon set to: " + m.epsilon);
			}

			//EpisodeController.epsilon = 0.1f;
			//EpisodeController.epsilon_min = 0.1f;
		}

		if (GUI.Button (new Rect (10, 180, 70, 30), "Obstacle")) {
			Debug.Log ("OBSTACLE");
			int index = rng.Next (0, numobstacles);
//			obstacles[index] = (GameObject) Instantiate(Obstacle,OPositions[index],ORotations[index]);
		}

		if (GUI.Button (new Rect (10, 220, 70, 30), "Add Car")) {
			
//			int index = rng.Next (0, numobstacles);
//			obstacles[index] = (GameObject) Instantiate(Obstacle,OPositions[index],ORotations[index]);

			//set the epsilon to the existing cars epsilon

			if (car_inst < numcars) {
				Debug.Log ("Car added.");
				double d = rng.NextDouble();
				cars[car_inst] = (GameObject) Instantiate(CarFab,Positions[car_inst], d>0.5 ? Rotations[car_inst] : Alt_Rotations[car_inst]);
				CarManager2 m = cars[car_inst].GetComponent(typeof(CarManager2)) as CarManager2;
				m.epsilon = m.epsilon_min;
				m.SetMLP (this.Qnetwork, this.QtargetNetwork);
				cars[car_inst].name = "CloneCar" + car_inst;
				car_inst++;
			}
		}

		if (GUI.Button (new Rect (10, 260, 70, 30), "Restart")) {
			Debug.Log ("RESTART");
			for (int i = 0; i < car_inst; i++){
				CarManager2 m = cars[i].GetComponent(typeof(CarManager2)) as CarManager2;
				m.Die ();
				double d = rng.NextDouble();
				if (!m.isEnabled()) m.Restart (Positions[i], Rotations[i]);
			}
		}

		if (GUI.Button (new Rect (10, 300, 70, 30), "Test Mode")) {
			Debug.Log ("Test mode");
			for (int i = 0; i < car_inst; i++){
				CarManager2 m = cars[i].GetComponent(typeof(CarManager2)) as CarManager2;
				m.test_mode = !m.test_mode;
				Debug.Log ("Test mode: " + m.test_mode);
			}
		}


	}

	private bool AllDead() {
		foreach (GameObject c in cars) {
			CarManager2 m = c.GetComponent(typeof(CarManager2)) as CarManager2;
			if (m.isEnabled()){
				return false;
			}
		}
		return true;
	}
}