using UnityEngine;
using System.Collections;

public class TintLight : MonoBehaviour {
	public bool rotate_forwards = false;
	public bool rotate_backwards = false;
	public bool flicker = false;
	public GameObject[] lightComponents;
	private Animator anim;
	public Color lightColor;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		renderer.material.color = lightColor;
		if( rotate_forwards == true && rotate_backwards == false)
		{
			anim.SetTrigger("Light_Rotate");
		}
		if( rotate_forwards == false && rotate_backwards == true)
		{
			anim.SetTrigger("Light_Rotate_Backwards");

		}
	}

	void Update()
	{
		if( flicker == true )
		{
			float RandomDelay = Random.Range(.25f, .85f );
			if( !IsInvoking())
				Invoke( "toggleLight", RandomDelay );
		}
	}

	public void toggleLight()
	{
		//This turns off the main spotlight
		if ( renderer.enabled == true )
			renderer.enabled = false;
		else if( renderer.enabled == false )
			renderer.enabled = true;

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
