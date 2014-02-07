using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePhysics : MonoBehaviour {

	public Vector2 velocity;
	public Vector2 maxVelocity = new Vector2(.1f,.2f);
	public float gravity=0.2f;
	public Vector2 drag = new Vector2(100f,0);
	public int[] IgnoreLayers;

	public int horizontalRays = 6;
	public int verticalRays = 4;
	public float xMargin = 0.4f;
	public float yMargin = 0.4f;
	public float horizontalRayLength = .5f;
	public float verticalRayLength = .5f;

	public bool allowJump = false;
	public float jumpVelocity=0.1f;
	public float maxJumpVelocityY = .3f;
	
	public bool allowSuperJump = false;
	public float jumpModifier = 3f;
	
	public bool allowSprinting = false;
	public float sprintModifier = 1.25f;
	
	public bool allowStickyWalls = false;
	public bool allowGravityVolumes = false;

	private int layerMask;
	private float defaultGravity;

	[HideInInspector]
	public bool isMoving=false;
	[HideInInspector]
	public bool isMovingVertical = false;
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
	public bool isJumping=false;
	[HideInInspector]
	private bool isStartingJump=false;
	[HideInInspector]
	private bool isStartingWallJump = false;
	[HideInInspector]
	private bool isAlignedProperly=false;
	[HideInInspector]
	private bool isWallAlignedProperly=false;
	[HideInInspector]
	private bool isCeilingAlignedProperly = false;
	[HideInInspector]
	public bool isSprinting=false;
	[HideInInspector]
	private bool isSprintSet = false;
	[HideInInspector]
	private Vector2 defaultMaxVelocity;
	[HideInInspector]
	public bool isSuperJumping;
	[HideInInspector]
	public bool isInGravityVolume = false;
	[HideInInspector]
	public Vector2 gravityVolumeVelocityOverride;
	[HideInInspector]
	public List<GameObject> touchedGravityVolumes = new List<GameObject>();
	[HideInInspector]
	public float gravityCenterThreshold;
	[HideInInspector]
	public bool CenterGravityVolume;
	[HideInInspector]
	public bool isOnStickyWall = false;
	[HideInInspector]
	public bool RegisterMovement = true;
	[HideInInspector]
	public bool TopOfWallClimb = true;
	[HideInInspector]
	public bool isTouchingMovingPlatform = false;
	[HideInInspector]
	public Vector2 moverVelocity;

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
		if(allowGravityVolumes && isInGravityVolume)
			HandleGravityVolumeMovement();
		if(allowStickyWalls&&CheckOnWall())
			HandleStickyWallMovement();
		else //normal movement
			HandleBasicMovement();
	}

	void HandleBasicMovement()
	{
		if(defaultGravity !=gravity)
			gravity = defaultGravity;

		SetSprinting();
		
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

		if(CheckIsTouching())
			RegisterMovement = true;
		
		//handle front and back collisions
		if(isBack == true  && velocity.x<0.0f)
			velocity.x=0;
		if(isFront == true  && velocity.x>0.0f)
			velocity.x=0;
		if(isCeiling == true && velocity.y>0.0f)
			velocity.y=0;
		
		HandleJumping();
		
		ApplyDrag();
		ApplyGravity();
		ClampVelocity();

		//apply movement
		transform.Translate((velocity*60)*Time.deltaTime);
		//Vector2 tempPosition = new Vector2((transform.position.x +(velocity.x*60) *Time.deltaTime),transform.position.y + (velocity.y*60) *Time.deltaTime);
		//transform.position = tempPosition;


		//align to ground and slope properly after physics calculations have completed.
		AlignToWalls();
		AlignToGround();
		AllignIfSloped();
		AllignIfLedge();
		AlignToCeiling();
	}

	void HandleGravityVolumeMovement()
	{
		velocity = gravityVolumeVelocityOverride;
		
		GameObject[] tempArray = new GameObject[touchedGravityVolumes.Count];
		touchedGravityVolumes.CopyTo(tempArray);

		if(CenterGravityVolume==true)
		{
			if(tempArray[tempArray.Length-1].GetComponent<GravityVolumes>().Vertical)
			{
				float volumeCenterX = tempArray[tempArray.Length-1].GetComponent<BoxCollider2D>().transform.position.x;

				bool CloseEnough = false;
				if((transform.position.x<volumeCenterX+gravityCenterThreshold)&&transform.position.x>volumeCenterX-gravityCenterThreshold)
				{
					CloseEnough = true;
				}

				if(CloseEnough==false&&transform.position.x<volumeCenterX)
					velocity.x+=0.05f;
				else if(transform.position.x>volumeCenterX)
					velocity.x-=0.05f;
				else
					velocity.x=0f;
			}
			else if(tempArray[tempArray.Length-1].GetComponent<GravityVolumes>().Horizontal)
			{
				float volumeCenterY = tempArray[tempArray.Length-1].GetComponent<BoxCollider2D>().transform.position.y;

				bool CloseEnough = false;
				if((transform.position.y<volumeCenterY+gravityCenterThreshold)&&transform.position.y>volumeCenterY-gravityCenterThreshold)
				{
					CloseEnough = true;
				}

				if(CloseEnough==false&&transform.position.y < volumeCenterY)
					velocity.y+=0.05f;
				else if(transform.position.y > volumeCenterY)
					velocity.y-=0.05f;
				else
					velocity.y=0f;
			}
		}
		//apply movement
		Vector2 tempPosition = new Vector2((transform.position.x +(velocity.x*60) *Time.deltaTime),transform.position.y + (velocity.y*60) *Time.deltaTime);
		transform.position = tempPosition;
	}

	void SetSprinting()
	{
		if(allowSprinting&&isSprinting)
		{
			if(isSprintSet == false)
			{
				maxVelocity = new Vector2(maxVelocity.x*sprintModifier,maxVelocity.y);
				isSprintSet = true;
				//Debug.Log ("SPRINTING");
			}
		}
		else
		{
			maxVelocity = defaultMaxVelocity;
			isSprintSet = false;
		}
		
	}

	void HandleJumping()
	{
		//handle jumping
		if(allowJump == true)
		{
			if(allowSuperJump==false)
			{
				if(isJumping == true)
				{
					StartCoroutine(ActivateJump());
				}
			}
			else
			{
				if(isSuperJumping)
					StartCoroutine(ActivateSuperJump());
				else if(isJumping)
					StartCoroutine(ActivateJump());
			}
		}
	}

	void HandleStickyWallJumping()
	{
		if(allowJump == true)
		{
			if(isJumping == true || (allowSuperJump==true && isSuperJumping == true))
			{
				StartCoroutine(ActivateWallJump());
			}
		}
	}

	private IEnumerator ActivateJump()
	{
		//Debug.Log ("jump now");
		velocity.y=jumpVelocity;
		isJumping=false;
		isStartingJump = true;
		if(isSloped)
			yield return new WaitForSeconds(.2f);	//wait longer if sloped so that you don't instantly plant onto the ground.
		else
			yield return new WaitForSeconds(.1f);
		isStartingJump=false;
		isAlignedProperly = false;
	}

	private IEnumerator ActivateSuperJump()
	{
		velocity.y=(jumpVelocity * jumpModifier);
		isSuperJumping=false;
		isStartingJump = true;
		if(isSloped)
			yield return new WaitForSeconds(.2f);	//wait longer if sloped so that you don't instantly plant onto the ground.
		else
			yield return new WaitForSeconds(.1f);
		isStartingJump=false;
		isAlignedProperly = false;
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
		if(isStartingJump==false&&(isGrounded||isOnStickyWall))
		{	
			VelYMax = maxVelocity.y;
		}
		else
			VelYMax = maxJumpVelocityY;

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

	void AllignIfSloped()
	{
		if(isLedge==false&&isSloped&&isStartingJump==false&&isGrounded==true)
		{
			RaycastHit2D hitGround = Physics2D.Raycast(transform.position, -Vector2.up, (verticalRayLength * 2), layerMask);
			transform.position = new Vector2( transform.position.x, hitGround.point.y+verticalRayLength);
		}
	}

	void AllignIfLedge()
	{
		if(isLedge == true && isStartingJump == false && isGrounded == true)
		{
			Vector2 startPoint = new Vector2(transform.position.x + xMargin, transform.position.y);
			Vector2 endPoint = new Vector2(transform.position.x-xMargin, transform.position.y);
			
			for(int i =0;i<verticalRays;i++)
			{
				float lerpAmount = (float)i/(float)(verticalRays-1);
				Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
				//Debug.DrawRay(origin,-Vector2.up);
				
				RaycastHit2D hitGround = Physics2D.Raycast(origin, -Vector2.up, (verticalRayLength*2), layerMask);
				ledgeChecks[i]=hitGround;
			}

			for(int i =0;i<ledgeChecks.Length;i++)
			{
				if (ledgeChecks[i]) {
					transform.position = new Vector2( transform.position.x, ledgeChecks[i].point.y+verticalRayLength);
					return;
				}
			}
		}
	}

	void AlignToGround()
	{
		RayTraceGround();

		if(isGrounded == true && isAlignedProperly == false && isSloped == false)
		{
			//Debug.Log ("ALIGNTOGROUNDNOW!!!!!!!!");
			RaycastHit2D hitGround = Physics2D.Raycast(transform.position, -Vector2.up, (verticalRayLength*2), layerMask);
			if(hitGround)
			{
				transform.position = new Vector2( transform.position.x, hitGround.point.y+verticalRayLength);
				isAlignedProperly=true;
			}
		}
	}

	void AlignToWalls()
	{
		if(isFront && isWallAlignedProperly == false && isStartingJump==false)
		{
			transform.position = new Vector2(FrontAlign.x, transform.position.y);
			isWallAlignedProperly = true;
			
			//Debug.Log ("GetOnThatWall!!!");
		}
		else if(isBack && isWallAlignedProperly == false && isStartingJump==false)
		{
			transform.position = new Vector2(BackAlign.x, transform.position.y);
			isWallAlignedProperly = true;

			//Debug.Log ("GetOnThatWall!!!");
		}
	}

	void AlignToCeiling()
	{
		RayTraceUp();

		if(isGrounded==false&&isCeiling==true && isCeilingAlignedProperly == false)
		{
			transform.position = new Vector2(transform.position.x,UpAlign.y);
			isCeilingAlignedProperly = true;

			//Debug.Log ("Hit That Ceiling!!!");
		}
	}

	void HandleStickyWallMovement()
	{
		RegisterMovement = true;
		SetSprinting();
		
		RayTraceUp();
		RayTraceGround();
		
		checkSlope();
		checkLedge();

		RayTraceFront();
		RayTraceBack();

		TopWallCheck();

		if(TopOfWallClimb && velocity.y>0.0f)
		{
			velocity.y=0f;
		}

		if(isCeiling == true && velocity.y>0.0f)
		{
			velocity.y=0f;
			velocity.x=0f;
		}
		else if(isGrounded==true&&velocity.y!=0.0f)
			velocity.y=0f;

		HandleStickyWallJumping();
		
		ApplyStickyWallDrag();
		ClampVelocity();
		
		Vector2 tempPosition=transform.position;
		
		if(isStartingJump == true || TopOfWallClimb == true)
			tempPosition= new Vector2((transform.position.x+(velocity.x*60) *Time.deltaTime),(transform.position.y + (velocity.y*60) *Time.deltaTime));
		else if(isFront==true || isBack == true)
			tempPosition= new Vector2(transform.position.x,transform.position.y + (velocity.y*60) *Time.deltaTime);
		else
		{
			//velocity.y=0f;
			velocity.x=0f;
		}
		
		transform.position = tempPosition;
		//align to ground and slope properly after physics calculations have completed.
		AlignToWalls();
		AlignToCeiling();
		AlignToGround();
		AllignIfSloped();
		AllignIfLedge();

	}

	void ApplyStickyWallDrag(){
		if(isMoving == false && isOnStickyWall==true)
		{
			if(velocity.y - (drag.x * Time.deltaTime) > 0 )
			{
				//Debug.Log("apply drag");
				velocity.y -= drag.x * Time.deltaTime;
			}
			else if(velocity.y + (drag.x * Time.deltaTime) < 0 )
			{
				//Debug.Log("apply drag");
				velocity.y += drag.x * Time.deltaTime;
			}
			else
			{
				//Debug.Log("park");
				velocity.y = 0.0f;
			}
		}
	}
	
	bool CheckOnWall()
	{
		bool lastFrameOnWallCheck = isOnStickyWall;

		if((isFront==true && isGrounded==false && IsFrontSticky()))
			isOnStickyWall = true;
	    else if ((isBack==true && isGrounded==false && IsBackSticky()))
			isOnStickyWall = true;
		else
			isOnStickyWall = false;

		if(lastFrameOnWallCheck==false && isOnStickyWall==true)
		{
			velocity.y=0.0f;
		}

		if(lastFrameOnWallCheck==true && isOnStickyWall == false  && isStartingJump==false)
		{
			velocity.y = 0.0f;
			velocity.x = 0.0f;
		}

		return isOnStickyWall;
	}

	bool TopWallCheck()
	{
		TopOfWallClimb = false;

		Vector2 startPoint = new Vector2(transform.position.x, transform.position.y + yMargin);
		Vector2 endPoint = new Vector2(transform.position.x, transform.position.y-yMargin);

		if(isFront)
		{
			for(int i =0;i<horizontalRays;i++)
			{
				float lerpAmount = (float)i/(float)(horizontalRays-1);
				Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
				Debug.DrawRay(origin,Vector2.right);
				
				RaycastHit2D hitFront = Physics2D.Raycast(origin, Vector2.right, horizontalRayLength, layerMask);
				frontChecks[i]=hitFront;
			}

			int RayCounter = 0;
			for(int i =0;i<frontChecks.Length;i++)
			{
				if(frontChecks[i])
					RayCounter++;
			}

			if(RayCounter==1&&frontChecks[frontChecks.Length-1])
				TopOfWallClimb = true;
		}
		if(isBack)
		{
			for(int i =0;i<horizontalRays;i++)
			{
				float lerpAmount = (float)i/(float)(horizontalRays-1);
				Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
				//Debug.DrawRay(origin,-Vector2.right);
				
				RaycastHit2D hitBack = Physics2D.Raycast(origin, -Vector2.right, horizontalRayLength, layerMask);
				backChecks[i]=hitBack;
			}
			int RayCounter = 0;
			for(int i =0;i<backChecks.Length;i++)
			{
				if(backChecks[i])
					RayCounter++;
			}

			if(RayCounter==1&&backChecks[backChecks.Length-1])
				TopOfWallClimb = true;
		}

		return TopOfWallClimb;
	}

	bool IsFrontSticky()
	{
		Vector2 startPoint = new Vector2(transform.position.x, transform.position.y + yMargin);
		Vector2 endPoint = new Vector2(transform.position.x, transform.position.y-yMargin);
		
		for(int i =0;i<horizontalRays;i++)
		{
			float lerpAmount = (float)i/(float)(horizontalRays-1);
			Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
			Debug.DrawRay(origin,Vector2.right);
			
			RaycastHit2D hitFront = Physics2D.Raycast(origin, Vector2.right, horizontalRayLength, 1 << LayerMask.NameToLayer("StickySurface"));
			frontChecks[i]=hitFront;
		}

		for(int i =0;i<frontChecks.Length;i++)
		{
			if (frontChecks[i]) {
				return true;
			}
		}
		return false;
	}
	
	bool IsBackSticky()
	{
		Vector2 startPoint = new Vector2(transform.position.x, transform.position.y + yMargin);
		Vector2 endPoint = new Vector2(transform.position.x, transform.position.y-yMargin);
		
		for(int i =0;i<horizontalRays;i++)
		{
			float lerpAmount = (float)i/(float)(horizontalRays-1);
			Vector2 origin = Vector2.Lerp(startPoint,endPoint,lerpAmount);
			//Debug.DrawRay(origin,-Vector2.right);
			
			RaycastHit2D hitBack = Physics2D.Raycast(origin, -Vector2.right, horizontalRayLength, 1 << LayerMask.NameToLayer("StickySurface"));
			backChecks[i]=hitBack;
		}

		for(int i =0;i<backChecks.Length;i++)
		{
			if (backChecks[i]) {
				return true;
			}
		}

		return false;
	}

	private IEnumerator ActivateWallJump()
	{
		//Debug.Log ("jump now");
		
		if(isFront==true)
			velocity.x=(-1*jumpVelocity);
		else if(isBack==true)
			velocity.x=jumpVelocity;
		
		velocity.y = jumpVelocity;

		isJumping=false;
		isStartingJump = true;
		RegisterMovement = false;
		isStartingWallJump = true;
		yield return new WaitForSeconds(.1f);
		isStartingJump=false;
		isAlignedProperly = false;
		yield return new WaitForSeconds(.15f);
		isStartingWallJump = false;
		RegisterMovement = true;
	}
}
