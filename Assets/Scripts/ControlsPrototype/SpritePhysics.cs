using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePhysics : MonoBehaviour {

	public Vector2 velocity;
	public Vector2 maxVelocity = new Vector2(.1f,.2f);

	public Vector2 drag = new Vector2(100f,0);
	public float jumpVelocity=0.1f;
	public float gravity=0.2f;

	public bool allowSprinting = false;
	public float sprintModifier = 1.25f;
	public bool allowSuperJump = false;
	public float jumpModifier = 3f;
	public bool allowStickyWalls = false;
	public bool allowGravityVolumes = false;

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
	public bool isJumping=false;
	[HideInInspector]
	private bool isStartingJump=false;
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
	public bool isOnStickyWall = false;

	//variables for raycasting
	private int horizontalRays = 6;
	private int verticalRays = 4;
	private float xMargin = 0.4f;
	private float yMargin = 0.4f;

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
		layerMask = 1<<11;
		layerMask |= 1<<12;
		layerMask = ~layerMask;

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
		if(isInGravityVolume && isInGravityVolume)
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
			Debug.Log ("not ground");

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
		
		HandleJumping();
		
		ApplyDrag();
		ApplyGravity();
		ClampVelocity();
		
		//apply movement
		Vector2 tempPosition = new Vector2((transform.position.x +(velocity.x*60) *Time.deltaTime),transform.position.y + (velocity.y*60) *Time.deltaTime);
		transform.position = tempPosition;
		
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
		
		if(tempArray[tempArray.Length-1].GetComponent<GravityVolumes>().Vertical)
		{
			float volumeCenterX = tempArray[tempArray.Length-1].GetComponent<BoxCollider2D>().transform.position.x;
			
			if(transform.position.x<volumeCenterX)
				velocity.x+=0.05f;
			else if(transform.position.x>volumeCenterX)
				velocity.x-=0.05f;
			else
				velocity.x=0f;
		}
		else if(tempArray[tempArray.Length-1].GetComponent<GravityVolumes>().Horizontal)
		{
			float volumeCenterY = tempArray[tempArray.Length-1].GetComponent<BoxCollider2D>().transform.position.y;
			
			if(transform.position.y < volumeCenterY)
				velocity.y+=0.05f;
			else if(transform.position.y > volumeCenterY)
				velocity.y-=0.05f;
			else
				velocity.y=0f;
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

	void HandleStickyWallJumping()
	{
		if(isJumping == true || (allowSuperJump==true && isSuperJumping == true))
		{
			StartCoroutine(ActivateWallJump());
		}
	}

	private IEnumerator ActivateJump()
	{
		Debug.Log ("jump now");
		velocity.y=jumpVelocity;
		isJumping=false;
		isStartingJump = true;
		yield return new WaitForSeconds(.1f);
		isStartingJump=false;
		isAlignedProperly = false;
	}

	private IEnumerator ActivateSuperJump()
	{
		velocity.y=(jumpVelocity * jumpModifier);
		isSuperJumping=false;
		isStartingJump = true;
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
			
			RaycastHit2D hitGround = Physics2D.Raycast(origin, -Vector2.up, 0.5f, layerMask);
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
			
			RaycastHit2D hitUp = Physics2D.Raycast(origin, Vector2.up, 0.5f, layerMask);
			upChecks[i]=hitUp;
		}
		
		isCeiling = false;
		
		for(int i =0;i<upChecks.Length;i++)
		{
			if (upChecks[i]) {
				isCeiling = true;

				UpAlign = new Vector2( transform.position.x, upChecks[i].point.y-.5f);
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
				
				RaycastHit2D hitFront = Physics2D.Raycast(origin, Vector2.right, 0.5f, layerMask);
				frontChecks[i]=hitFront;
			}

			isFront = false;

			for(int i =0;i<frontChecks.Length;i++)
			{
				if (frontChecks[i]) {
					isFront = true;
					FrontAlign = new Vector2( frontChecks[i].point.x-0.5f, transform.position.y);

					if(lastFrameFrontCheck==false&&isFront==true)
						isWallAlignedProperly = false;

					return;
				}
			}
		}
		else
		{
			RaycastHit2D hitFront = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, layerMask);
		
			if (hitFront) {
				isFront = true;
				//Debug.Log("I've hit the front");
				FrontAlign = new Vector2( hitFront.point.x-0.5f, transform.position.y);
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
				
				RaycastHit2D hitBack = Physics2D.Raycast(origin, -Vector2.right, 0.5f, layerMask);
				backChecks[i]=hitBack;
			}
			
			isBack = false;
			
			for(int i =0;i<backChecks.Length;i++)
			{
				if (backChecks[i]) {
					isBack = true;
					//Debug.Log("I've hit the back");

					BackAlign = new Vector2( backChecks[i].point.x+0.5f, transform.position.y);

					if(lastFrameBackCheck==false&&isBack==true)
						isWallAlignedProperly = false;

					return;
				}
			}
		}
		else
		{
			RaycastHit2D hitBack = Physics2D.Raycast(transform.position, -Vector2.right, 0.5f, layerMask);
			
			if (hitBack) {
				isBack = true;
				//Debug.Log("I've hit the back");
				BackAlign = new Vector2( hitBack.point.x+0.5f, transform.position.y);
			}
			else
				isBack = false;

			if(lastFrameBackCheck==false&&isBack==true)
				isWallAlignedProperly = false;
		}
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
		
		if(velocity.y > maxVelocity.y)
			velocity.y = maxVelocity.y;
		else if(velocity.y < -maxVelocity.y)
			velocity.y = -maxVelocity.y;
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
			RaycastHit2D hitGround = Physics2D.Raycast(transform.position, -Vector2.up, 1.0f, layerMask);
			transform.position = new Vector2( transform.position.x, hitGround.point.y+.5f);
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
				
				RaycastHit2D hitGround = Physics2D.Raycast(origin, -Vector2.up, 1.0f, layerMask);
				ledgeChecks[i]=hitGround;
			}

			for(int i =0;i<ledgeChecks.Length;i++)
			{
				if (ledgeChecks[i]) {
					transform.position = new Vector2( transform.position.x, ledgeChecks[i].point.y+.5f);
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
			Debug.Log ("ALIGNTOGROUNDNOW!!!!!!!!");
			RaycastHit2D hitGround = Physics2D.Raycast(transform.position, -Vector2.up, 1.0f, layerMask);
			if(hitGround)
			{
				transform.position = new Vector2( transform.position.x, hitGround.point.y+.5f);
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
			
			Debug.Log ("GetOnThatWall!!!");
		}
		else if(isBack && isWallAlignedProperly == false && isStartingJump==false)
		{
			transform.position = new Vector2(BackAlign.x, transform.position.y);
			isWallAlignedProperly = true;

			Debug.Log ("GetOnThatWall!!!");
		}
	}

	void AlignToCeiling()
	{
		RayTraceUp();

		if(isGrounded==false&&isCeiling==true && isCeilingAlignedProperly == false)
		{
			transform.position = new Vector2(transform.position.x,UpAlign.y);
			isCeilingAlignedProperly = true;

			Debug.Log ("Hit That Ceiling!!!");
		}
	}

	void HandleStickyWallMovement()
	{
		SetSprinting();
		
		RayTraceUp();
		RayTraceGround();
		
		checkSlope();
		checkLedge();

		RayTraceFront();
		RayTraceBack();
		
		if(isCeiling == true && (velocity.y>0.0f  ||  (isFront && velocity.x>0.0f) || (isBack && velocity.x<0.0f)))
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
		
		if(isStartingJump == true)
			tempPosition= new Vector2((transform.position.x+(velocity.x*60) *Time.deltaTime),transform.position.y + (velocity.y*60) *Time.deltaTime);
		else if(isFront==true)
			tempPosition= new Vector2(transform.position.x,transform.position.y + (velocity.x*60) *Time.deltaTime);
		else if(isBack==true)
			tempPosition = new Vector2(transform.position.x,transform.position.y + ((-1*velocity.x)*60) *Time.deltaTime);
		else
		{
			velocity.y=0f;
			velocity.x=0f;
		}
		
		transform.position = tempPosition;
		//align to ground and slope properly after physics calculations have completed.
		AlignToCeiling();
		AlignToGround();
		AllignIfSloped();
		AllignIfLedge();
		AlignToWalls();
	}

	void ApplyStickyWallDrag(){
		if(isMoving == false && isOnStickyWall==true)
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
	
	bool CheckOnWall()
	{
		bool lastFrameOnWallCheck = isOnStickyWall;

		if((isFront==true && isGrounded==false && IsFrontSticky()))
			isOnStickyWall = true;
	    else if ((isBack==true && isGrounded==false && IsBackSticky()))
			isOnStickyWall = true;
		else
			isOnStickyWall = false;

		if(lastFrameOnWallCheck==true && isOnStickyWall == false  && isStartingJump==false)
		{
			velocity.y = 0.0f;
			velocity.x = 0.0f;
		}

		return isOnStickyWall;
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
			
			RaycastHit2D hitFront = Physics2D.Raycast(origin, Vector2.right, 0.5f, 1 << LayerMask.NameToLayer("StickySurface"));
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
			
			RaycastHit2D hitBack = Physics2D.Raycast(origin, -Vector2.right, 0.5f, 1 << LayerMask.NameToLayer("StickySurface"));
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
		Debug.Log ("jump now");
		
		if(isFront==true)
			velocity.x=(-1*jumpVelocity);
		else if(isBack==true)
			velocity.x=jumpVelocity;
		
		velocity.y = jumpVelocity;
		isJumping=false;
		isStartingJump = true;
		yield return new WaitForSeconds(.1f);
		isStartingJump=false;
		isAlignedProperly = false;
	}
}
