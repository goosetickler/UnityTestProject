using UnityEngine;
using System.Collections;

public class MoverLogic : MonoBehaviour {

	public Transform StopPosition;
	private Vector2 StartPosition;
	private Vector2 CurrentMoveToPosition;
	public bool ActivateMover=false;
	private GameObject playerGameObject;
	private SpriteMoverPhysics physics;
	public bool isHorizontal = false;
	public bool isVertical = false;
	public bool isLooping = false;
	private bool startMoveRight = false;
	private bool startMoveUp = false;
	public float moveVelocity = 0.05f;

	// Use this for initialization
	void Start () {
		playerGameObject = GameObject.Find("Player");
		physics = GetComponent<SpriteMoverPhysics>();

		if(isHorizontal)
		{
			if(transform.position.x<StopPosition.position.x)
				startMoveRight = true;
			else
				startMoveRight = false;
		}

		if(isVertical)
		{
			if(transform.position.y<StopPosition.position.y)
				startMoveUp = true;
			else
				startMoveUp = false;
		}

		StartPosition = transform.position;
		CurrentMoveToPosition = StopPosition.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isHorizontal)
			CheckHorizontalMovement();
		else if(isVertical)
			CheckVerticalMovement();
	}

	void CheckHorizontalMovement()
	{
		if(startMoveRight==true)
		{
			if(ActivateMover==true && (transform.position.x<CurrentMoveToPosition.x))
			{
				physics.velocity.x=moveVelocity;
			}
			else if(ActivateMover==true && (transform.position.x>=CurrentMoveToPosition.x))
			{
				if(isLooping==true)
				{
					startMoveRight=false;
					CurrentMoveToPosition = StartPosition;
				}
				else
				{
					ActivateMover = false;
					physics.velocity.x=.0f;
				}
			}
		}
		else
		{
			if(ActivateMover==true && (transform.position.x>CurrentMoveToPosition.x))
			{
				physics.velocity.x=-moveVelocity;
			}
			else if(ActivateMover==true && (transform.position.x<=CurrentMoveToPosition.x))
			{
				if(isLooping==true)
				{
					startMoveRight=true;
					CurrentMoveToPosition = StopPosition.position;
				}
				else
				{
					ActivateMover = false;
					physics.velocity.x=.0f;
				}
			}
		}
	}

	void CheckVerticalMovement()
	{
		if(startMoveUp==true)
		{
			if(ActivateMover==true && (transform.position.y<CurrentMoveToPosition.y))
			{
				physics.velocity.y=moveVelocity;
			}
			else if(ActivateMover==true && (transform.position.y>=CurrentMoveToPosition.y))
			{
				if(isLooping==true)
				{
					startMoveUp=false;
					CurrentMoveToPosition = StartPosition;
				}
				else
				{
					ActivateMover = false;
					physics.velocity.y=.0f;
				}
			}
		}
		else
		{
			if(ActivateMover==true && (transform.position.y>CurrentMoveToPosition.y))
			{
				physics.velocity.y=-moveVelocity;
			}
			else if(ActivateMover==true && (transform.position.y<=CurrentMoveToPosition.y))
			{
				if(isLooping==true)
				{
					startMoveUp=true;
					CurrentMoveToPosition = StopPosition.position;
				}
				else
				{
					ActivateMover = false;
					physics.velocity.y=.0f;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		Debug.Log("SomethingIsTouching");

		if(ActivateMover==true)	
		{
			//playerGameObject.transform.parent = transform;
			playerGameObject.GetComponent<SpritePhysics>().isTouchingMovingPlatform = true;
		}
	}

	void OnTriggerExit2D(Collider2D collider){
		Debug.Log("SomethingIsNotTouching");
		//playerGameObject.transform.parent = null;
		playerGameObject.GetComponent<SpritePhysics>().isTouchingMovingPlatform = false;
	}
}
