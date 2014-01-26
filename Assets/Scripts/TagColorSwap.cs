using UnityEngine;
using System.Collections;

public class TagColorSwap : MonoBehaviour {
	public Color orange;
	public Color purple;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D col) 
	{ 
		ColorTagSwap( col );
	}

	void OnTriggerStay2D (Collider2D col) 
	{ 
		ColorTagSwap( col );
	}

	void ColorTagSwap(Collider2D col)
	{
		/////////////////////////////////////////////////////////
		/// This is horrible. 
		/// 

		if(!col.name.Contains("light"))
			return;

		if( col.tag == "White" )
		{
			renderer.material.color = Color.white;
			gameObject.tag = "White";
		}
		if( gameObject.tag == "White" )
		{
			if( col.tag == "Blue")
			{
				renderer.material.color = Color.blue;
				gameObject.tag = "Blue";
			}
			else if( col.tag == "Red")
			{
				renderer.material.color = Color.red;
				gameObject.tag = "Red";
			}
			else if( col.tag == "Yellow")
			{
				renderer.material.color = Color.yellow;
				gameObject.tag = "Yellow";
			}
		}
		else if( gameObject.tag == "Blue" )
		{
			if( col.tag == "Blue")
			{
				renderer.material.color = Color.blue;
				gameObject.tag = "Blue";
			}
			else if( col.tag == "Red")
			{
				renderer.material.color = purple;
				gameObject.tag = "Purple";
			}
			else if( col.tag == "Yellow")
			{
				renderer.material.color = Color.green;
				gameObject.tag = "Green";
			}
		}
		else if( gameObject.tag == "Red" )
		{
			if( col.tag == "Blue")
			{
				renderer.material.color = purple;
				gameObject.tag = "Purple";
			}
			else if( col.tag == "Red")
			{
				renderer.material.color = Color.red;
				gameObject.tag = "Red";
			}
			else if( col.tag == "Yellow")
			{
				renderer.material.color = orange;
				gameObject.tag = "Orange";
			}
		}
		else if( gameObject.tag == "Yellow" )
		{
			if( col.tag == "Blue")
			{
				renderer.material.color = Color.green;
				gameObject.tag = "Green";
			}
			else if( col.tag == "Red")
			{
				renderer.material.color = orange;
				gameObject.tag = "Orange";
			}
			else if( col.tag == "Yellow")
			{
				renderer.material.color = Color.yellow;
				gameObject.tag = "Yellow";
			}
		}
		else if( gameObject.tag == "Purple" )
		{
			if( col.tag == "Blue" || col.tag == "Red")
			{
				renderer.material.color = purple;
				gameObject.tag = "Purple";
			}
			else if( col.tag == "Yellow")
			{
				renderer.material.color = Color.yellow;
				gameObject.tag = "Yellow";
			}
		}
		else if( gameObject.tag == "Orange" )
		{
			if( col.tag == "Yellow" || col.tag == "Red")
			{
				renderer.material.color = orange;
				gameObject.tag = "Orange";
			}
			else if( col.tag == "Blue")
			{
				renderer.material.color = Color.blue;
				gameObject.tag = "Blue";
			}
		}
		else if( gameObject.tag == "Green" )
		{
			if( col.tag == "Yellow" || col.tag == "Blue")
			{
				renderer.material.color = Color.green;
				gameObject.tag = "Green";
			}
			else if( col.tag == "Red")
			{
				renderer.material.color = Color.red;
				gameObject.tag = "Red";
			}
		}
	}
}
