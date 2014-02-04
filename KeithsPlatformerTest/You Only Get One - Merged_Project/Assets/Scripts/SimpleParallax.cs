using UnityEngine;
using System.Collections;

public class SimpleParallax : MonoBehaviour {
	private GameObject Player;
	public float MovementSpeed = 1;
	public bool ParallaxX = true;
	public bool ParallaxY = true;
	public bool ParallaxXY = false;
	
	// Use this for initialization
	void Start () 
	{
	}

	void Awake()
	{
		Player = GameObject.Find("Player");
	}

	// Update is called once per frame
	void Update () 
	{
		float Smoothing = Random.Range( 0.025f, 0.095f);
		Vector2 PlayerVelocity = Player.rigidbody2D.velocity;
		if( ParallaxX == true)
		{
			PlayerVelocity.y = 0;
			PlayerVelocity.x = PlayerVelocity.x * -MovementSpeed;
			transform.Translate(PlayerVelocity * Smoothing * Time.deltaTime);
		}
		else if( ParallaxY == true)
		{
			PlayerVelocity.x = 0;
			PlayerVelocity.y = PlayerVelocity.y * -MovementSpeed;
			transform.Translate(PlayerVelocity * Smoothing * Time.deltaTime);
		}
		else if( ParallaxXY == true && ParallaxX == false && ParallaxY == false )
		{
			PlayerVelocity.x = PlayerVelocity.x * -MovementSpeed;
			PlayerVelocity.y = PlayerVelocity.y * -MovementSpeed;
			transform.Translate(PlayerVelocity * Smoothing * Time.deltaTime);
		}
	}
}
