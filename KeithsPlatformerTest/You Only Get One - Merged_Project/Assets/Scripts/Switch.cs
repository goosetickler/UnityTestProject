using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	public string unlockColor;
	public GameObject colorLight;
	private Animator colorLightAnim;
	private bool isStillInsideTrigger = false;
	// Use this for initialization
	void Start () {
		colorLightAnim = colorLight.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col) 
	{ 

		if( isStillInsideTrigger == false )
		{
			if( col.tag == unlockColor)
			{
				colorLight.GetComponent<TintLight>().toggleLight();
			}
			isStillInsideTrigger = true;
		}
	}

	void OnTriggerExit2D (Collider2D col) 
	{ 
		isStillInsideTrigger = false;
	}
}
