//#define TRACK1
//#define TRACK2
#define TRACK3
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour{

	public static int numcars = 1;
	public static int numinst = 1;
	public static int numobstacles = 3;
	public GameObject CarFab;
	public GameObject[] cars;
	public GameObject Obstacle;
	public GameObject[] obstacles;
	public int episode = 0;
	public CarManager EpisodeController;
	public MLP Qnetwork;
	public MLP QtargetNetwork;
	public System.Random rng = new System.Random();
	public static bool RandomInit = false;
	public static bool curriculum = true;
	public static bool load_at_init = false;
	public static bool multipletrainers = false;
	public static bool use_lead_epsilon = true;

	#if TRACK1
	public static int RTT = 600;
	public static int track = 1;
	public static Vector3[] Positions = {new Vector3 (-2.8f, 0.1f, 1.3f), new Vector3 (-2.8f, 0.1f, 0.29f), 
	new Vector3 (-30f, 0.1f, 3.49367f), new Vector3 (-31.5f, 0.1f, 3.49367f),
	new Vector3 (-21.77f, 0.1f, 6.46f), new Vector3 (-21.77f, 0.1f, 8.36f),
	new Vector3(-0.145386f, 0.1f,6.5f ), new Vector3(-1.145386f, 0.1f,8f )};

	public static Quaternion[] Rotations= { Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), 
	Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), Quaternion.AngleAxis(0, new Vector3(0, 1, 0)),
	Quaternion.AngleAxis(120, new Vector3(0, 1, 0)), Quaternion.AngleAxis(120, new Vector3(0, 1, 0)), 
	Quaternion.AngleAxis(90, new Vector3(0, 1, 0)), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))};

	public static Vector3[] OPositions = {new Vector3 (-31.566f, 0.1f, 13.79664f), 
	new Vector3 (-8.73f, 0.1f, 0.42f),
	new Vector3 (0.81f, 0.1f, 6.32f)
	};

	public static Quaternion[] ORotations = { Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
	Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
	Quaternion.AngleAxis(0, new Vector3(0, 1, 0))
	};

	public static Quaternion[] Alt_Rotations = { Quaternion.AngleAxis (-90, new Vector3 (0, 1, 0)),
	Quaternion.AngleAxis (0, new Vector3 (0, 1, 0)), 
	Quaternion.AngleAxis (90, new Vector3 (0, 1, 0)),
	Quaternion.AngleAxis (180, new Vector3 (0, 1, 0))
	};

	#endif

	#if TRACK3	
	public static int RTT = 1200;
	public static int track = 3;
	public static Vector3[] Positions = {new Vector3 (-1.33f, 0.1f, 0.87f), 
		new Vector3 (-10.43f, 0.1f, 0.34f),
		new Vector3 (-19.47f, 0.1f, -2.13f), 
		new Vector3 (-28.46f, 0.1f,-0.81f ),
		new Vector3 (-31f, 0.1f, 15.61f),
		new Vector3 (-23.87f, 0.1f, 19.91f),
		new Vector3 (-18.1f, 0.1f, 15.71f),
		new Vector3(-12.04f, 0.1f,17.76f ),
		new Vector3 (-6.57f, 0.1f, 21.18f),
		new Vector3 (0.46f, 0.1f, 18.74f),
		new Vector3 (-2.57f, 0.1f, 12.77f),
		new Vector3(-9.62f, 0.1f,8.64f ),
		new Vector3(2.38f, 0.1f,6.23f )
		
	
	};

	public static Quaternion[] Rotations = { Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), 
		Quaternion.AngleAxis(240, new Vector3(0, 1, 0)), 
		Quaternion.AngleAxis(270, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(295, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(0, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(120, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(90, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(45, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(90, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(160, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(270, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(110, new Vector3(0, 1, 0)),
		Quaternion.AngleAxis(180, new Vector3(0, 1, 0))
	};

	public static Vector3[] OPositions = {new Vector3 (-31.566f, 0.1f, 13.79664f), 
										  new Vector3 (-8.73f, 0.1f, 0.42f),
										  new Vector3 (0.81f, 0.1f, 6.32f)
	};

	public static Quaternion[] ORotations = { Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
											  Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
											  Quaternion.AngleAxis(0, new Vector3(0, 1, 0))
	};

	public static Quaternion[] Alt_Rotations = { Quaternion.AngleAxis (-90, new Vector3 (0, 1, 0)),
		Quaternion.AngleAxis (0, new Vector3 (0, 1, 0)), 
		Quaternion.AngleAxis (90, new Vector3 (0, 1, 0)),
		Quaternion.AngleAxis (180, new Vector3 (0, 1, 0))
	};

	#endif


	#if TRACK2
	public static int RTT = 1200;
	public static int track = 2;
	public static Vector3[] Positions = { new Vector3 (-3.94f, 0.1f, 0.165f), 
		new Vector3 (-0.16f, 0.1f, 4.0185f),
		new Vector3 (3.8563f, 0.1f, 0.3039f), 
		new Vector3 (-0.18f, 0.1f, -3.66f)
	};

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

	public static Vector3[] OPositions = {new Vector3 (-31.566f, 0.1f, 13.79664f), 
		new Vector3 (-8.73f, 0.1f, 0.42f),
		new Vector3 (0.81f, 0.1f, 6.32f)
	};

	public static Quaternion[] ORotations = { Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
		Quaternion.AngleAxis(0, new Vector3(0, 1, 0)), 
		Quaternion.AngleAxis(0, new Vector3(0, 1, 0))
	};
	#endif

	void Awake(){
		//Test Classes
		new NNTest ();
		ImageManipulation.test();
	}

	void Start () {
		Application.runInBackground = true;
		//init variables
		cars = new GameObject[numcars];
		obstacles = new GameObject[numobstacles];
		Qnetwork = new MLP(CarManager.state_size, CarManager.hiddenSizes);
		QtargetNetwork = new MLP(CarManager.state_size, CarManager.hiddenSizes);
		Qnetwork.copyTo (QtargetNetwork);	

		//*Transfer Learning */
		if (load_at_init) {
			this.QtargetNetwork.read ("QTargetNetwork");
			this.Qnetwork.read ("QNetwork");
		}

		for (int i = 0; i < (numinst==0 ? ++numinst:numinst) ; i++){
			int index = getIndex (i);
			Quaternion cur_rotation = getRotation(index);
			Vector3 cur_position = getPosition (index);
	
			cars[i] = (GameObject) Instantiate(CarFab,cur_position,cur_rotation);
			CarManager m = cars[i].GetComponent(typeof(CarManager)) as CarManager;
			m.SetMLP (this.Qnetwork, this.QtargetNetwork);

			if (!multipletrainers && i != 0) {
				m.training = false;
				m.display_qvalues = false;
				m.report = false;
				m.epsilon = m.epsilon_min;
			} else {
				m.training = true;
				m.display_qvalues = i == 0 ? true : false;	
				m.report = true;
				if (i == 0) {
					cars [0].GetComponent<Renderer> ().material.color = Color.green;
					EpisodeController = cars [0].GetComponent (typeof(CarManager)) as CarManager;
				}
			}
			cars[i].name = "CloneCar" + i;
		}
	}

	// Update is called once per frame
	void Update () {
		
		episode = EpisodeController.getEpisode ();
		for (int i = 0; i < numinst; i++){			
			int index = getIndex (i);
			Quaternion cur_rotation = getRotation(index);
			Vector3 cur_position = getPosition (index);

			CarManager m = cars[i].GetComponent(typeof(CarManager)) as CarManager;
			if (!m.isEnabled ()) {
				if (use_lead_epsilon) m.epsilon = EpisodeController.epsilon;	
				m.Restart (cur_position, cur_rotation);
			}
		}
	}
	private GUIStyle guiStyle = new GUIStyle(); //create a new variable
	void OnGUI() {
		guiStyle.fontSize = 48; //change the font size
		GUI.Label (new Rect (10, 10, 100, 200), "Episode: " + episode, guiStyle);
		if (GUI.Button (new Rect (10, 100, 75, 30), "Save")) {
			Debug.Log ("SAVED");
			this.QtargetNetwork.write ("QTargetNetwork");
			this.Qnetwork.write ("QNetwork");
		}

		if (GUI.Button (new Rect (90, 100, 75, 30), "SaveGraph")) {
			Memory.saveGraphs (EpisodeController.returnValues, EpisodeController.returnCounts, "graph.txt");
			Debug.Log ("Graph SAVED");
		}

		if (GUI.Button (new Rect (10, 140, 75, 30), "Load")) {
			Debug.Log ("LOADED");
			this.QtargetNetwork.read ("QTargetNetwork");
			this.Qnetwork.read ("QNetwork");
			EpisodeController.epsilon = 0.1f;
			EpisodeController.epsilon_min = 0.1f;
			EpisodeController.training = false;
			RandomInit = false;
			curriculum = false;
		}

		if (GUI.Button (new Rect (10, 180, 75, 30), "Obstacle")) {
			Debug.Log ("OBSTACLE");
			int index = rng.Next (0, numobstacles);
			obstacles[index] = (GameObject) Instantiate(Obstacle,OPositions[index],ORotations[index]);
		}

		if (GUI.Button (new Rect (90,140, 70, 30), "Add Car")) {

			if (numinst < numcars) {
				int index = getIndex (numinst);
				Quaternion cur_rotation = getRotation(index);
				Vector3 cur_position = getPosition (index);
				cars[numinst] = (GameObject) Instantiate(CarFab,cur_position, cur_rotation);
				CarManager m = cars[numinst].GetComponent(typeof(CarManager)) as CarManager;
				if (!multipletrainers && numinst != 0) {
					m.training = false;
					m.display_qvalues = false;
					m.report = false;
					m.epsilon = m.epsilon_min;
				} else {
					m.display_qvalues = false;
					m.SetMLP (this.Qnetwork, this.QtargetNetwork);
				}
				cars[numinst].name = "CloneCar" + numinst;
				numinst++;
			}
				
		}


		if (GUI.Button (new Rect (90, 180, 70, 30), "Restart")) {
			Debug.Log ("RESTART");
			for (int i = 0; i < numinst; i++){
				CarManager m = cars[i].GetComponent(typeof(CarManager)) as CarManager;
				m.Die ();
				if (!m.isEnabled ()) {
					int index = getIndex (i);
					Quaternion cur_rotation = getRotation(index);
					Vector3 cur_position = getPosition (index);
					m.Restart (cur_position, cur_rotation);
				}
			}

			UserCarControl usercar= GameObject.Find("UserCar").GetComponent(typeof(UserCarControl)) as UserCarControl;
			usercar.Restart();

				

		}

		if (GUI.Button (new Rect (170, 100, 70, 30), "Test Mode")) {
			Debug.Log ("Test mode");
			for (int i = 0; i < numinst; i++){
				CarManager m = cars[i].GetComponent(typeof(CarManager)) as CarManager;
				m.training = !m.training;
				Debug.Log ("Test mode: " + !m.training);
			}

		}
			
		GUI.Label (new Rect (10, 330, 200, 30), "Q-Values");
	}

	private int getIndex(int i){
		//start easy location with a probability. otherwise random
		int index = 0;
		if (curriculum) {
			if (rng.NextDouble () < 0.5) {
				index = rng.Next (0, Positions.Length);
			} else {
				index = i;
			}
		}else{
			if (numcars == 1 && RandomInit) {
				index = rng.Next (0, Positions.Length);
			} else {
				index = i;
			}
		}
		return index;
	}

	private Quaternion getRotation(int index){
		if (track == 2) {
			return rng.NextDouble () > 0.5 ? Rotations [index] : Alt_Rotations [index];
		} else {
			return Rotations [index];
		}
	}


	private Vector3 getPosition(int index){
		return Positions [index];
	}

	private bool AllDead() {
			foreach (GameObject c in cars) {
			CarManager m = c.GetComponent(typeof(CarManager)) as CarManager;
			if (m.isEnabled()){
					return false;
				}
			}
		return true;
	}
}