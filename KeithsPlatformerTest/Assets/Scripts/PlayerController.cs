﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Animator anim;

	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	[HideInInspector]
	public bool superJump = false;
	
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public float jumpPadModifier = 3;

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Transform frontCheck;
	private Transform backCheck;

	private Vector2 ForceTeleLocation;
	private GameObject Body;

	private bool TeleportStarted = false;
	private bool groundImpact= false;			// Whether or not the player is grounded.
	private bool slopeImpact = false;			// Whether or not the player is grounded.
	private bool frontImpact = false;
	private bool backImpact = false;
	private bool jumpPadImpact = false;

	public int CurrentLevelName = 0;
	public bool InMenu = false;
	private bool directionCheckRight=true;

	//audio
	public AudioClip JumpClip;
	public AudioClip TeleportInClip;
	public AudioClip TeleportOutClip;
	
	private AudioSource JumpSource;
	private AudioSource TeleportInSource;
	private AudioSource TeleportOutSource;

	void Awake()
	{
		JumpSource = AddAudio(JumpClip,false,false,1.0f);
		TeleportInSource = AddAudio(TeleportInClip,false,false,1.0f);
		TeleportOutSource = AddAudio(TeleportOutClip,false,false,1.0f);

		groundCheck = transform.Find("GroundCheck");
		frontCheck = transform.Find ("FrontCheck");
		backCheck = transform.Find ("BackCheck");

		Body = GameObject.Find ("Body");
		anim = GetComponent<Animator>();
	}

	AudioSource AddAudio(AudioClip NewClip, bool IsLoop, bool OnAwake, float Volume)
	{
		
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = NewClip;
		newAudio.loop = IsLoop;
		newAudio.playOnAwake = OnAwake;
		newAudio.volume = Volume;
		
		return newAudio;
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(InMenu == false)
		{
			// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
			groundImpact = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
			slopeImpact = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Slope")); 
			jumpPadImpact = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("JumpPad"));
			frontImpact = ImpactCheck(frontCheck, "Ground");
			backImpact = ImpactCheck(backCheck, "Ground");

			// If the jump button is pressed and the player is grounded then the player should jump.
			if(Input.GetButtonDown("Jump") && (groundImpact || slopeImpact))
				jump = true;

			if(Input.GetButtonDown("Jump") && jumpPadImpact)
				superJump = true;
		}
	}

	bool ImpactCheck( Transform zoneToCheck, string layerToCheck )
	{
		bool ImpactCheck = false;

		ImpactCheck = Physics2D.Linecast(transform.position, zoneToCheck.position, 1 << LayerMask.NameToLayer(layerToCheck)); 

		if(ImpactCheck==true)
			return ImpactCheck;

		if(ImpactCheck!=true)
		{
			Vector3 UpCheck = new Vector3(0,.25f,0);
			ImpactCheck = Physics2D.Linecast(transform.position, (zoneToCheck.position + UpCheck), 1 << LayerMask.NameToLayer(layerToCheck)); 

			if(ImpactCheck==true)
				return ImpactCheck;
		}

		if(ImpactCheck!=true)
		{
			Vector3 DownCheck = new Vector3(0,-.25f, 0);
			ImpactCheck = Physics2D.Linecast(transform.position, (zoneToCheck.position + DownCheck), 1 << LayerMask.NameToLayer(layerToCheck));

			if(ImpactCheck==true)
				return ImpactCheck;
		}

		return ImpactCheck;

	}

	void FixedUpdate ()
	{
		if(TeleportStarted == true)
			transform.position = ForceTeleLocation; 
		else if (InMenu == false && TeleportStarted == false)
		{
			// Cache the horizontal input.
			float h = Input.GetAxis("Horizontal");

			if((h==0) && jump==false)
			{	
				//apply some negative force to bring object to a quicker stop
				//Vector2 NegativeForce = new Vector2((rigidbody2D.velocity.x * -1),rigidbody2D.velocity.y);
				//rigidbody2D.velocity = NegativeForce;
			}
			else
			{
				if(h>0 && frontImpact==false)
				{
					if(h * rigidbody2D.velocity.x < maxSpeed)
					{
						directionCheckRight=true;
						if(jump==false)
							rigidbody2D.AddForce(Vector2.right * h * moveForce);
						else
							rigidbody2D.AddForce(Vector2.right * h * (moveForce/1.5f));
					}
				}			
				
				if(h<0 &&backImpact == false)
				{
					directionCheckRight=false;
					if(h * rigidbody2D.velocity.x < maxSpeed)
					{
						if(jump==false)
							rigidbody2D.AddForce(Vector2.right * h * moveForce);
						else
							rigidbody2D.AddForce(Vector2.right * h * (moveForce/1.5f));
					}
				}
				
				if(((h > 0.25f) && frontImpact==true) || ((h < -0.25f) && backImpact==true))
				{
					Vector2 NullForce = new Vector2((rigidbody2D.velocity.x * 0),rigidbody2D.velocity.y);
					
					rigidbody2D.velocity = NullForce;
					Debug.Log("collision");
				}

			}

			//clamps max speed
			if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
				rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

			if(superJump)
			{
				rigidbody2D.AddForce(new Vector2(0f, (jumpForce*jumpPadModifier)));
				superJump=false;
				jump = false;
				anim.SetTrigger("JumpPad");
				JumpSource.Play();
			}
			else if(jump)
			{			
				rigidbody2D.AddForce(new Vector2(0f, jumpForce));
				jump = false;
				superJump=false;
				anim.SetTrigger("Jump");
				JumpSource.Play();
			}
		}
	}

	public void ResetPlayer(Vector2 NewPlayerStart)
	{
		TeleportInSource.Play();
		anim.SetTrigger("TeleportIn");

		Body.renderer.material.color = Color.white;
		Body.tag = "White";

		transform.position = NewPlayerStart;
		rigidbody2D.velocity = new Vector2(0,0);
	}

	public void StartTeleportOutSequence()
	{
		if(TeleportStarted==false)
			StartCoroutine(PlayTeleportOut());
	}

	private IEnumerator PlayTeleportOut()
	{
		ForceTeleLocation= new Vector2(transform.position.x,transform.position.y);
		TeleportStarted=true;
		rigidbody2D.velocity = new Vector2(0,0);
		anim.SetTrigger("TeleportOut");
		TeleportOutSource.Play();
		yield return new WaitForSeconds(2.0f);
		CurrentLevelName++;
		GameObject tempCamera = GameObject.FindGameObjectWithTag("MainCamera");
		tempCamera.GetComponent<CameraController>().ProgressIntro = false;
		tempCamera.GetComponent<CameraController>().InitTransition = false;
		tempCamera.GetComponent<CameraController>().levelInit = true;
		TeleportStarted=false;
	}

	public void ExitMenuStartGame()
	{
		CurrentLevelName++;
		GameObject tempCamera = GameObject.FindGameObjectWithTag("MainCamera");
		tempCamera.GetComponent<CameraController>().ProgressIntro = false;
		tempCamera.GetComponent<CameraController>().InitTransition = false;
		tempCamera.GetComponent<CameraController>().levelInit = true;
		TeleportStarted=false;
	}
}
