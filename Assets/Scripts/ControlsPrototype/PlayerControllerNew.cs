using UnityEngine;
using System.Collections;

public class PlayerControllerNew : MonoBehaviour {

	private SpritePhysics physics;
	// Use this for initialization
	void Start () {
		physics = GetComponent<SpritePhysics>();
	}

	void Update()
	{
		if(Input.GetButtonDown("SuperJump"))
		{
			if(physics.isGrounded==true)
			{
				physics.isSuperJumping = true;
			}
			else
				StartCoroutine(BufferSuperJumpInput());
		}
		else if(Input.GetButtonDown("Jump"))
		{
			Debug.Log ("try to jump");
			if(physics.isGrounded==true)
			{
				Debug.Log ("actual jump");
				physics.isJumping = true;
			}
			else
				StartCoroutine(BufferJumpInput());

			if(physics.isOnStickyWall==true)
				physics.isJumping = true;
			else
				StartCoroutine(BufferJumpInput());
		}
		
		if(Input.GetButton("Sprint"))
		{
			physics.isSprinting=true;
		}
		else
			physics.isSprinting=false;
	}

	// Update is called once per frame
	void FixedUpdate () {
		physics.velocity.x += Input.GetAxis("Horizontal") * (Time.deltaTime*10);
		//needs horizontal axis to return to 0 faster.
		if(Input.GetAxis("Horizontal") !=0)
			physics.isMoving = true;
		else
			physics.isMoving = false;
	}

	//if I'm almost on the ground but not just perfectly placed I will still jump when I land.
   	private IEnumerator BufferJumpInput()
   	{
		yield return new WaitForSeconds(.1f);
		if(physics.isGrounded==true)
			physics.isJumping = true;
	}

	private IEnumerator BufferSuperJumpInput()
	{
		yield return new WaitForSeconds(.1f);
		if(physics.isGrounded==true)
		{
			physics.isSuperJumping = true;
		}
	}
}
 