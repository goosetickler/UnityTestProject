using UnityEngine;
using System.Collections;

public class RandomPlayRate : MonoBehaviour {
	private Animator anim;
	public float minPlayRate;
	public float maxPlayRate;

	void Start () 
	{
		anim = GetComponent<Animator>();
		anim.speed =.25f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float RandomDelay = Random.Range(.25f, .85f );
		if( !IsInvoking("adjustAnimPlayRate"))
			Invoke( "adjustAnimPlayRate", RandomDelay );
	}

	void adjustAnimPlayRate()
	{
		float playRate = Random.Range( minPlayRate, maxPlayRate );
			anim.speed = playRate;
	}
}
