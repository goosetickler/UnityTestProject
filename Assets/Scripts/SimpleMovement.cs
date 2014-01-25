using UnityEngine;
using System.Collections;

public class SimpleMovement : MonoBehaviour {
	public float movementSpeed = .05f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W))
			transform.position = new Vector3(transform.position.x, transform.position.y + movementSpeed, transform.position.z);

		if(Input.GetKey(KeyCode.S))
			transform.position = new Vector3(transform.position.x, transform.position.y - movementSpeed, transform.position.z);

		
		if(Input.GetKey(KeyCode.A))
			transform.position = new Vector3(transform.position.x - movementSpeed, transform.position.y, transform.position.z);
		
		if(Input.GetKey(KeyCode.D))
			transform.position = new Vector3(transform.position.x + movementSpeed, transform.position.y, transform.position.z);
	}
}
