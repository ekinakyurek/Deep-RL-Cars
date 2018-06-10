using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour{


	public static int numcars = 1;
	public GameObject CarFab;
	public GameObject[] cars;
	public int episode = 0;
	public CarManager EpisodeController;
	//public static Quaternion InitialRotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
	//public static Vector3 InitialPosition = new Vector3(-3.05f,0.1f,0.64f);

	public System.Random rng = new System.Random();
//	public static Quaternion InitialRotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
//	public static Vector3[] Positions = {new Vector3 (-2.8f, 0.1f, 0.65f), new Vector3 (-31.38969f, 0.1f, 3.49367f),
//		new Vector3 (-20.7f, 0.1f, 7.24f), new Vector3(-1.145386f, 0.1f,7.381379f )};
//	public static Quaternion[] Rotations = { Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), Quaternion.AngleAxis(0, new Vector3(0, 1, 0)),
//		Quaternion.AngleAxis(120, new Vector3(0, 1, 0)), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))};
//	public static Vector3 InitialPosition = new Vector3 (-2.8f, 0.1f, 0.65f);

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
//		//Will send notifications that something has happened to whoever is interested
//		Subject subject = new Subject();

	// Use this for initialization
	void Start () {
		//new NNTest ();
		Application.runInBackground = true;
		cars = new GameObject[numcars];
		for (int i = 0; i < numcars; i++){
			int ind = rng.Next (0, Positions.Length);
			cars[i] = (GameObject) Instantiate(CarFab,Positions[ind],Rotations[ind]);
			cars[i].name = "CloneCar" + i;
		}
		EpisodeController = cars[0].GetComponent(typeof(CarManager)) as CarManager;
	}

	// Update is called once per frame
	void Update () {
		//The boxes should jump if the sphere is cose to origo
			if (AllDead())
			{
				foreach (GameObject c in cars) {
					CarManager m = c.GetComponent(typeof(CarManager)) as CarManager;
					int ind = rng.Next (0, Positions.Length);
					m.Restart (Positions[ind],Rotations[ind]);
				}
			}
		  episode = EpisodeController.getEpisode ();
	}
	private GUIStyle guiStyle = new GUIStyle(); //create a new variable
	void OnGUI() {
		guiStyle.fontSize = 48; //change the font size
	//	GUI.Label (new Rect (Screen.width / 2 - 850, Screen.height / 2 + 400, 200, 200), "Episode: " + episode, guiStyle);
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


