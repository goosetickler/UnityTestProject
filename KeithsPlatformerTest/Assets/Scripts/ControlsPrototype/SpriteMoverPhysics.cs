using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteMoverPhysics : MonoBehaviour {
	
	public Vector2 velocity;
	public Vector2 maxVelocity = new Vector2(.1f,.1f);
	public float gravity=0.0f;
	public Vector2 drag = new Vector2(100f,0);
	public bool isStatic = false;
	public int[] IgnoreLayers;
	
	public int horizontalRays = 6;
	public int verticalRays = 4;
	public float xMargin = 0.4f;
	public float yMargin = 0.4f;
	public float horizontalRayLength = .5f;
	public float verticalRayLength = .5f;

	private int layerMask;
	private float defaultGravity;
	
	[HideInInspector]
	public bool isMoving=false;
	[HideInInspector]
	public bool isGrounded=false;
	[HideInInspector]
	public bool isFront=false;
	[HideInInspector]
	public bool isBack=false;
	[HideInInspector]
	public bool isCeiling=false;
	[HideInInspector]
	public bool isSloped=false;
	[HideInInspector]
	public bool isLedge=false;
	[HideInInspector]
	private bool isAlignedProperly=false;
	[HideInInspector]
	private bool isWallAlignedProperly=false;
	[HideInInspector]
	private bool isCeilingAlignedProperly = false;
	[HideInInspector]
	private Vector2 defaultMaxVelocity;

	private Vector2 FrontAlign;
	private Vector2 BackAlign;
	private Vector2 UpAlign;
	
	private RaycastHit2D[] downChecks;
	private RaycastHit2D[] frontChecks;
	private RaycastHit2D[] backChecks;
	private RaycastHit2D[] upChecks;
	private RaycastHit2D[] ledgeChecks;
	
	// Use this for initialization
	void Start () 
	{
		if(IgnoreLayers.Length>0)
		{
			layerMask = 1<<IgnoreLayers[0];
			for(int i =1;i<IgnoreLayers.Length;i++)
			{
				layerMask |= 1<<IgnoreLayers[i];
			}
			layerMask = ~layerMask;
		}
		
		downChecks = new RaycastHit2D[verticalRays];
		upChecks = new RaycastHit2D[verticalRays];
		frontChecks = new RaycastHit2D[horizontalRays];
		backChecks = new RaycastHit2D[horizontalRays];
		ledgeChecks = new RaycastHit2D[verticalRays];
		
		defaultMaxVelocity = maxVelocity;
		
		defaultGravity =gravity;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void FixedUpdate() 
	{
		HandleBasicMovement();
	}
	
	void HandleBasicMovement()
	{
		RayTraceUp();
		RayTraceGround();
		
		if(isGrounded==false)
		{
			//Debug.Log ("not ground");
		}
		
		checkSlope();
		checkLedge();
		
		RayTraceFront();
		RayTraceBack();

		//handle front and back collisions
		if(isBack == true  && velocity.x<0.0f)
			velocity.x=0;
		if(isFront == true  && velocity.x>0.0f)
			velocity.x=0;
		if(isCeiling == true && velocity.y>0.0f)
			velocity.y=0;

		ApplyDrag();
		ApplyGravity();
		ClampVelocity();
		
		//apply movement
		Vector2 tempPosition = new Vector2((transform.position.x +(velocity.x*60) *Time.deltaTime),transform.position.y + (velocity.y*60) *Time.deltaTime);
		transform.position = tempPosition;
	}
	
	void RayTraceGround()
	{
		bool LastFrameGroundCheck = isGrounded;
		
		Vector2 startPoint = new Vector2(transform.position.x + xMargin, transform.position.y);
		Vector2 endPoint = new Vector2(transform.position.x-xMargin, transform.position.y);
		
		for(int i =0;i<verticalRays;i++)
		{
			float lerpAmount = (float)i/(float)(verticalRays-1);
			Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
			//Debug.DrawRay(origin,-Vector2.up);
			
			RaycastHit2D hitGround = Physics2D.Raycast(origin, -Vector2.up, verticalRayLength, layerMask);
			downChecks[i]=hitGround;
		}
		
		isGrounded = false;
		
		for(int i =0;i<downChecks.Length;i++)
		{
			if (downChecks[i]) {
				isGrounded = true;
			}
		}
		
		if(LastFrameGroundCheck==true && isGrounded == false)
			isAlignedProperly = false;
	}
	
	void RayTraceUp()
	{
		bool LastFrameUpCheck = isCeiling;
		
		Vector2 startPoint = new Vector2(transform.position.x + xMargin, transform.position.y);
		Vector2 endPoint = new Vector2(transform.position.x-xMargin, transform.position.y);
		
		for(int i =0;i<verticalRays;i++)
		{
			float lerpAmount = (float)i/(float)(verticalRays-1);
			Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
			Debug.DrawRay(origin,Vector2.up);
			
			RaycastHit2D hitUp = Physics2D.Raycast(origin, Vector2.up, verticalRayLength, layerMask);
			upChecks[i]=hitUp;
		}
		
		isCeiling = false;
		
		for(int i =0;i<upChecks.Length;i++)
		{
			if (upChecks[i]) {
				isCeiling = true;
				
				UpAlign = new Vector2( transform.position.x, upChecks[i].point.y-verticalRayLength);
				return;
				//Debug.Log("I hit the ceiling");
			}
		}
		
		if(LastFrameUpCheck == true && isCeiling == false)
			isCeilingAlignedProperly = false;
	}
	
	void RayTraceFront()
	{
		bool lastFrameFrontCheck = isFront;
		
		if(isSloped==false)
		{
			Vector2 startPoint = new Vector2(transform.position.x, transform.position.y + yMargin);
			Vector2 endPoint = new Vector2(transform.position.x, transform.position.y-yMargin);
			
			for(int i =0;i<horizontalRays;i++)
			{
				float lerpAmount = (float)i/(float)(horizontalRays-1);
				Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
				Debug.DrawRay(origin,Vector2.right);
				
				RaycastHit2D hitFront = Physics2D.Raycast(origin, Vector2.right, horizontalRayLength, layerMask);
				frontChecks[i]=hitFront;
			}
			
			isFront = false;
			
			for(int i =0;i<frontChecks.Length;i++)
			{
				if (frontChecks[i]) {
					isFront = true;
					FrontAlign = new Vector2( frontChecks[i].point.x-horizontalRayLength, transform.position.y);
					
					if(lastFrameFrontCheck==false&&isFront==true)
						isWallAlignedProperly = false;
					
					return;
				}
			}
		}
		else
		{
			RaycastHit2D hitFront = Physics2D.Raycast(transform.position, Vector2.right, horizontalRayLength, layerMask);
			
			if (hitFront) {
				isFront = true;
				//Debug.Log("I've hit the front");
				FrontAlign = new Vector2( hitFront.point.x-horizontalRayLength, transform.position.y);
			}
			else
				isFront = false;
			
			
			if(lastFrameFrontCheck==false&&isFront==true)
				isWallAlignedProperly = false;
		}
	}
	
	void RayTraceBack()
	{
		bool lastFrameBackCheck = isBack;
		
		if(isSloped==false)
		{
			Vector2 startPoint = new Vector2(transform.position.x, transform.position.y + yMargin);
			Vector2 endPoint = new Vector2(transform.position.x, transform.position.y-yMargin);
			
			for(int i =0;i<horizontalRays;i++)
			{
				float lerpAmount = (float)i/(float)(horizontalRays-1);
				Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
				//Debug.DrawRay(origin,-Vector2.right);
				
				RaycastHit2D hitBack = Physics2D.Raycast(origin, -Vector2.right, horizontalRayLength, layerMask);
				backChecks[i]=hitBack;
			}
			
			isBack = false;
			
			for(int i =0;i<backChecks.Length;i++)
			{
				if (backChecks[i]) {
					isBack = true;
					//Debug.Log("I've hit the back");
					
					BackAlign = new Vector2( backChecks[i].point.x+horizontalRayLength, transform.position.y);
					
					if(lastFrameBackCheck==false&&isBack==true)
						isWallAlignedProperly = false;
					
					return;
				}
			}
		}
		else
		{
			RaycastHit2D hitBack = Physics2D.Raycast(transform.position, -Vector2.right, horizontalRayLength, layerMask);
			
			if (hitBack) {
				isBack = true;
				//Debug.Log("I've hit the back");
				BackAlign = new Vector2( hitBack.point.x+horizontalRayLength, transform.position.y);
			}
			else
				isBack = false;
			
			if(lastFrameBackCheck==false&&isBack==true)
				isWallAlignedProperly = false;
		}
	}
	
	bool CheckIsTouching()
	{
		if(isBack||isFront||isCeiling||isGrounded)
			return true;
		else
			return false;
	}
	
	void ApplyDrag(){
		if(isMoving == false&&isGrounded==true)
		{
			if(velocity.x - (drag.x * Time.deltaTime) > 0 )
			{
				//Debug.Log("apply drag");
				velocity.x -= drag.x * Time.deltaTime;
			}
			else if(velocity.x + (drag.x * Time.deltaTime) < 0 )
			{
				//Debug.Log("apply drag");
				velocity.x += drag.x * Time.deltaTime;
			}
			else
			{
				//Debug.Log("park");
				velocity.x = 0.0f;
			}
		}
	}
	
	void ApplyGravity(){
		if(isGrounded == false)
			velocity.y -= gravity * Time.deltaTime;
		else
		{
			if(velocity.y<0)
				velocity.y=0;
		}
	}
	
	void ClampVelocity()
	{
		//maxVelocity
		if(velocity.x > maxVelocity.x )
			velocity.x = maxVelocity.x;
		else if(velocity.x < -maxVelocity.x )
			velocity.x = -maxVelocity.x;
		
		float VelYMax = 0.0f;
		VelYMax = maxVelocity.y;
		
		if(velocity.y > VelYMax)
			velocity.y = VelYMax;
		else if(velocity.y < -VelYMax)
			velocity.y = -VelYMax;
	}
	
	void checkSlope()
	{
		if(downChecks[0].point.y!=downChecks[1].point.y||downChecks[0].point.y!=downChecks[2].point.y||downChecks[0].point.y!=downChecks[3].point.y)
		{
			isSloped =  true;
			return;
		}
		else
			isSloped =  false;
	}
	
	void checkLedge()
	{
		if((downChecks[0].collider==null&&downChecks[3].collider!=null)||(downChecks[0].collider!=null&&downChecks[3].collider==null))
		{
			//cast some new rays to check and see if we are on a ledge
			Vector2 startPoint = new Vector2(transform.position.x + xMargin, transform.position.y);
			Vector2 endPoint = new Vector2(transform.position.x-xMargin, transform.position.y);
			
			for(int i =0;i<verticalRays;i++)
			{
				float lerpAmount = (float)i/(float)(verticalRays-1);
				Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
				//Debug.DrawRay(origin,-Vector2.up);
				
				RaycastHit2D hitGround = Physics2D.Raycast(origin, -Vector2.up, 1.0f, layerMask);
				ledgeChecks[i]=hitGround;
			}
			
			if((ledgeChecks[0].collider==null&&ledgeChecks[3].collider!=null)||(ledgeChecks[0].collider!=null&&ledgeChecks[3].collider==null))
			{
				isLedge = true;
				return;
			}
		}
		
		isLedge = false;
	}
}
		