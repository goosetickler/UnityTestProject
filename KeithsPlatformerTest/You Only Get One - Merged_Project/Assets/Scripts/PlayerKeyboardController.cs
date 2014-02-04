using UnityEngine;
using System.Collections;

public class PlayerKeyboardController : MonoBehaviour 
{
	public float MovementSpeed = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(Input.GetKey(KeyCode.W))
			rigidbody2D.AddForce( Vector2.up * 35f );
		
		else if(Input.GetKey(KeyCode.S))
			rigidbody2D.AddForce( Vector2.up * -MovementSpeed );
		
		else if(Input.GetKey(KeyCode.A))
			rigidbody2D.AddForce( Vector2.right * -MovementSpeed );
		
		else if(Input.GetKey(KeyCode.D))
			rigidbody2D.AddForce( Vector2.right * MovementSpeed );

		else if(Input.GetKey(KeyCode.Space))
		{
			renderer.material.color = Color.white;
			gameObject.tag = "White";
		}
			
	}
}
