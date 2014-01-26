using UnityEngine;
using System.Collections;

public class CameraControllerSimple : MonoBehaviour {

	public float xMargin = 1f;	
	public float yMargin = 1f;	
	public float xSmooth = 8f;	 
	public float ySmooth = 8f;	 
	public float xCamOffset = 0f;
	public float yCamOffset = 0f;
	public float OrthoZoomLevel = 4f;
	public float OrthoZoomOutLevel = 10f;

	private GameObject playerGameObject;
	private Transform player;

	private bool xMarginCheck = false;
	private bool yMarginCheck = false;
	private bool orthoCheck = false;

	void Awake ()
	{
		playerGameObject = GameObject.Find("Player");
		player = GameObject.Find("Player").transform;
	}

	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - (player.position.x+xCamOffset)) > xMargin;
	}
	
	
	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.y - (player.position.y+yCamOffset)) > yMargin;
	}

	void MoveCamera()
	{
		//starting target values
		float targetX = transform.position.x;
		float targetY = transform.position.y;
		
		// If the player has moved beyond the x margin...
		if(CheckXMargin())
			targetX = Mathf.Lerp(transform.position.x, (player.position.x+xCamOffset), xSmooth * Time.deltaTime);
		
		// If the player has moved beyond the y margin...
		if(CheckYMargin())
			targetY = Mathf.Lerp(transform.position.y, (player.position.y+yCamOffset), ySmooth * Time.deltaTime);
		
		transform.position = new Vector3(targetX, (targetY), transform.position.z);
	}
	
	void FixedUpdate ()
	{
		MoveCamera();
	}
}
