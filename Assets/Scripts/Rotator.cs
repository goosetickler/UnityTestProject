using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	// Use this for initialization
	public int rotationAngle = 45;
	public bool rotateBackwards = false;

	void Start () 
	{
		if( rotateBackwards )
			rotationAngle = rotationAngle * -1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		rotateObject ();
	}

	void rotateObject()
	{
		transform.Rotate(0,0,rotationAngle * Time.deltaTime);
	}
}
