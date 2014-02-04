using UnityEngine;
using System.Collections;

public class ForceField : MonoBehaviour {
	public string unlockColor;
	public GameObject blockingvolume;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col) 
	{ 
		if( col.tag == unlockColor)
		{
			//Destroy( blockingvolume );
			blockingvolume.collider2D.enabled = false;
		}
	}
	void OnTriggerExit2D (Collider2D col) 
	{ 

		blockingvolume.collider2D.enabled = true;
	}
}
