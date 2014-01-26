using UnityEngine;
using System.Collections;

public class TintLight : MonoBehaviour {
	public bool flicker = false;
	public bool rotateLight = false;
	public bool rotateBackwards = false;
	public int rotationAngle = 45;
	public bool isOn = true;
	public GameObject[] lightComponents;
	private Animator anim;
	public Color lightColor;

	// Use this for initialization
	void Start () 
	{
		if( rotateBackwards == true )
			rotationAngle = rotationAngle * -1;

		anim = GetComponent<Animator>();
		renderer.material.color = lightColor;
		if( isOn == false )
			toggleLight();
	}

	void Update()
	{
		if( rotateLight == true )
			rotate();

		if( flicker == true )
		{
			float RandomDelay = Random.Range(.25f, .85f );
			if( !IsInvoking())
				Invoke( "toggleLight", RandomDelay );
		}
	}

	void rotate()
	{
		transform.Rotate(0,0,rotationAngle * Time.deltaTime);
	}

	public void toggleLight()
	{
		//This turns off the main spotlight
		if ( renderer.enabled == true )
		{
			renderer.enabled = false;
			//isOn = false;
		}
		else if( renderer.enabled == false )
		{
			//isOn = true;
			renderer.enabled = true;
		}
		//This turns off the collider & glow
		foreach (var item in lightComponents) 
		{
			if( item.activeSelf == true )
				item.SetActive( false );
			else if( item.activeSelf == false )
				item.SetActive( true );
		}
	}
}
