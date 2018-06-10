using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCarControl : MonoBehaviour {

	public static float speed = 4.5f;
	public static float turnSpeed = 100f;

	private Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		transform.position = new Vector3 (-1.33f, 0.1f, -0.5f);
		transform.rotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
	}

	public void Restart () {
		transform.position = new Vector3 (-1.33f, 0.1f, -0.5f);
		transform.rotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
	}
	
	// Update is called once per frame
	void Update () {


	}

	void FixedUpdate ()
	{

		var steer=Input.GetAxis("Horizontal");
		var gas=Input.GetAxis("Vertical");

		var moveDist=gas*speed*Time.deltaTime;
		//now the turn amount, similar drill, just turnSpeed instead of speed
		//   we multiply in gas as well, which properly reverses the steering when going 
		//   backwards, and scales the turn amount with the speed
		var turnAngle=steer * turnSpeed * Time.deltaTime;
		//now apply 'em, starting with the turn

		transform.rotation *=  Quaternion.AngleAxis(turnAngle, Vector3.up);
		//transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y  + turnAngle,transform.rotation.eulerAngles.z);
		//and now move forward by moveVect
		transform.Translate(Vector3.forward*moveDist);

		//float moveHorizontal = Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");

		//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		//rb.AddForce (movement * speed);
	}

	void OnCollisionEnter(Collision col)
	{
		transform.position = new Vector3 (-1.33f, 0.1f, -0.5f);
		transform.rotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));

	}
}
