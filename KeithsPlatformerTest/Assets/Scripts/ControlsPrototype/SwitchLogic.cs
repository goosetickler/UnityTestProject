﻿using UnityEngine;
using System.Collections;

public class SwitchLogic : MonoBehaviour {

	public Transform StopPosition;
	private GameObject playerGameObject;
	private SpriteMoverPhysics physics;

	private bool isPlayerTouching=false;
	private bool isFullyPressed=false;

	// Use this for initialization
	void Start () {
		playerGameObject = GameObject.Find("Player");
		physics = GetComponent<SpriteMoverPhysics>();
	}
	
	// Update is called once per frame
	void Update () {
		if(isFullyPressed == false && isPlayerTouching==true && (transform.position.y>StopPosition.position.y))
		{
			physics.velocity.y=-.01f;
		}
		else if(isFullyPressed == false && isPlayerTouching==true && (transform.position.y<=StopPosition.position.y))
		{
			isFullyPressed = true;
			physics.velocity.y= 0f;
			Debug.Log("IsFullyPressed");
		}
		else
			physics.velocity.y= 0f;
	}

	void OnTriggerEnter2D(Collider2D collider){
		Debug.Log("SomethingIsTouching");
		if(collider.gameObject == playerGameObject)
		{
			Debug.Log("ItsThePlayer");
			isPlayerTouching = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D collider)
	{
		Debug.Log("SomethingIsNotTouching");
	}

	void OnCollisionExit2D(Collision2D collider)
	{
		Debug.Log("SomethingIsNotTouching");
	}


}