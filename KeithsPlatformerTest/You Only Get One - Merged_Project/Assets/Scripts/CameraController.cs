﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public float xMargin = 1f;	
	public float yMargin = 1f;	
	public float xSmooth = 8f;	 
	public float ySmooth = 8f;	 
	public float xCamOffset = 0f;
	public float yCamOffset = 0f;
	public float OrthoZoomLevel = 4f;
	public float OrthoZoomOutLevel = 10f;

	private GameObject playerGameObject;
	private GameObject currentLevelMarkup;
	private Transform player;

	private bool xMarginCheck = false;
	private bool yMarginCheck = false;
	private bool orthoCheck = false;

	[HideInInspector]
	public bool DetachCamera = false;

	[HideInInspector]
	public bool ProgressIntro = false;
	[HideInInspector]
	public bool InitTransition = false;

	public bool levelInit = false;

	private string currentLevelName = "";

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

	void Update()
	{
		if(Input.GetButtonDown("Jump"))
			ProgressIntro = true;
	}
	void FixedUpdate ()
	{
		if(DetachCamera==false)
		{
			if(levelInit==false)
				MoveCamera();
			else
				TransitionToPlayer();
				//SnapCamera(player.transform);
		}
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

	void SnapCamera(Transform StartingPoint)
	{
		transform.position = new Vector3(StartingPoint.position.x+xCamOffset, (StartingPoint.position.y+yCamOffset), transform.position.z);
	}

	void TransitionToPlayer()
	{
		playerGameObject.GetComponent<PlayerController>().InMenu = true;

		if(ProgressIntro == false  && InitTransition == false)
		{
			//get current level from the player
			//get camera starting position from LevelData
			//set player starting location
			if(playerGameObject.GetComponent<PlayerController>().CurrentLevelName==1)
			{
				currentLevelMarkup = GameObject.Find("Level1Markup");
				InitTransition = true;
			}
			else if(playerGameObject.GetComponent<PlayerController>().CurrentLevelName==2)
			{
				currentLevelMarkup = GameObject.Find("Level2Markup");
				InitTransition = true;
			}
			else
			{
				//this is where the gameover stuff would go.
				//for now it just loops back to the first level.
				playerGameObject.GetComponent<PlayerController>().CurrentLevelName=1;
			}
			//add more levels to this stupid if else statement because I'm dumb..

			//setup the player
			Vector3 temp = currentLevelMarkup.GetComponent<LevelData>().playerStart.position;
			Vector2 tempPlayerStart = new Vector2(temp.x,temp.y);
			playerGameObject.GetComponent<PlayerController>().ResetPlayer(tempPlayerStart);

			//setup the camera
			camera.orthographicSize = currentLevelMarkup.GetComponent<LevelData>().OrthoZoomOutLevel;

			Vector3 tempStart = currentLevelMarkup.GetComponent<LevelData>().cameraStart.position;
			transform.position = new Vector3((tempStart.x),tempStart.y, -10);

			//set level name
			currentLevelMarkup.GetComponent<LevelData>().UI.gameObject.guiText.text=currentLevelMarkup.GetComponent<LevelData>().LevelName;
		}
		else if(ProgressIntro == true)
		{
			if(camera.orthographicSize>(OrthoZoomLevel-.1f) && camera.orthographicSize<(OrthoZoomLevel+.1f))
			{		
				orthoCheck = false;
			}
			else
			{
				camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 4, Time.deltaTime * 1);
				orthoCheck = true;
			}
		
			//starting target values
			float targetX = transform.position.x;
			float targetY = transform.position.y;

			xMarginCheck = CheckXMargin();
			yMarginCheck = CheckYMargin();
			// If the player has moved beyond the x margin...
			if(xMarginCheck)
				targetX = Mathf.Lerp(transform.position.x, (player.position.x+xCamOffset), xSmooth * Time.deltaTime);
			
			// If the player has moved beyond the y margin...
			if(yMarginCheck)
				targetY = Mathf.Lerp(transform.position.y, (player.position.y+yCamOffset), ySmooth * Time.deltaTime);

			transform.position = new Vector3(targetX, (targetY), transform.position.z);
		
			if(xMarginCheck == false && yMarginCheck == false && orthoCheck == false)
			{
				levelInit = false;
				playerGameObject.GetComponent<PlayerController>().InMenu = false;
				currentLevelMarkup.GetComponent<LevelData>().UI.gameObject.guiText.text="";
			}
		}
	}
}
