using UnityEngine;
using System.Collections;

public class RandomPlayRate : MonoBehaviour {
	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		anim.speed =.25f;
	}
	
	// Update is called once per frame
	void Update () {
		float RandomDelay = Random.Range(.25f, 1.85f );
		if( !IsInvoking("adjustAnimPlayRate"))
			Invoke( "adjustAnimPlayRate", RandomDelay );
	}

	void adjustAnimPlayRate()
	{
		float playRate = Random.Range( .1f, .75f );
			anim.speed = playRate;
	}
}
