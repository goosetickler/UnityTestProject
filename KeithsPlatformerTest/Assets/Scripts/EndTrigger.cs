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
				other.GetComponent<PlayerController>().StartTeleportOutSequence();
				gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}
		}
	}

	public void ResetTrigger2D()
	{
		gameObject.GetComponent<BoxCollider2D>().enabled = true;
	}
}
