using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	public string unlockColor;
	public GameObject colorLight;
	public GameObject[] toggleLights;
	private Animator colorLightAnim;
	public bool isStillInsideTrigger = false;
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
				audio.Play();
				colorLight.GetComponent<TintLight>().toggleLight();

				if( toggleLights.Length > 0 )
				{
					foreach (var light in toggleLights) 
					{
						light.GetComponent<TintLight>().toggleLight();
					}
				}
			}
			isStillInsideTrigger = true;
		}
	}

	void OnTriggerExit2D (Collider2D col) 
	{ 
		isStillInsideTrigger = false;
		collider2D.enabled = false;
		Invoke( "ResetSwitch", .25f);
	}

	void ResetSwitch()
	{
		collider2D.enabled = true;
	}
}
