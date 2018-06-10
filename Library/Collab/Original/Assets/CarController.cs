using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class CarController : MonoBehaviour {

	public static int nsensor =  10; // Number of sensors
	public  static float maxdist = 15; // Maximum distance which a sensor can measure
	public  static float stepangle = Mathf.PI / nsensor;
	public Rigidbody rb;
	public RaycastHit[] hits;
	public Vector3[] angles;
	public float[] distances;



	void Start()
	{
		rb = GetComponent<Rigidbody>();
		hits = new RaycastHit[nsensor];
	 	angles =  new Vector3[nsensor];
		distances = new float[nsensor];

		for (int i = 0; i < nsensor; i++)
		{
			angles [i] = new Vector3 (-Mathf.Sin (i * stepangle), 0, Mathf.Cos (i * stepangle));
		}


	}

	void FixedUpdate()
	{

	}

	void Update(){
		
		for (int i = 0; i < nsensor; i++)
		{
			Debug.DrawRay(transform.position, angles[i], Color.red);

			if (Physics.Raycast(transform.position, angles[i], out hits[i], maxdist))
			{
				distances [i] = hits[i].distance;
			}else{
				distances [i] = maxdist;
			}

		}


		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
	}
}
