using UnityEngine;
using System.Collections;

public class EndTrigger : MonoBehaviour {

	private GameObject BodyCheck;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name=="Player")
		{
			BodyCheck = GameObject.Find("Body");
			if(gameObject.tag == BodyCheck.tag)
			{
				GameObject tempCamera = GameObject.FindGameObjectWithTag("MainCamera");
				other.GetComponent<PlayerController>().CurrentLevelName++;
				tempCamera.GetComponent<CameraController>().ProgressIntro = false;
				tempCamera.GetComponent<CameraController>().InitTransition = false;
				tempCamera.GetComponent<CameraController>().levelInit = true;
			}
		}
	}
}
