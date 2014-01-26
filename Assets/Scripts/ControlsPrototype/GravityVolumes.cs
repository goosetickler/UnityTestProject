using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityVolumes : MonoBehaviour {

	private GameObject playerGameObject;

	public Vector2 gravityVolumeVelocity;
	public bool Horizontal;
	public bool Vertical;

	// Use this for initialization
	void Start () {
		playerGameObject = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D (Collider2D col) 
	{ 
		playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Add(gameObject);
		playerGameObject.GetComponent<SpritePhysics>().isInGravityVolume = true;
		playerGameObject.GetComponent<SpritePhysics>().gravityVolumeVelocityOverride = gravityVolumeVelocity;
		Debug.Log ("enter");
	}
	void OnTriggerExit2D (Collider2D col) 
	{ 
		//remove what we just exited
		playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Remove(gameObject);

		//copy current contents
		List<GameObject> tempList = new List<GameObject>();

		if(playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Count>0)
		{
			for(int i=0;i<playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Count;i++)
			{
				if(playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes[i]!=null)
					tempList.Add(playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes[i]);
			}
		}
	
		//clear the previous contents
		playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes = new List<GameObject>();

		if(tempList.Count>0)
		{
			//add back in valid objects
			for(int i=0;i<tempList.Count;i++)
			{
				playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Add(tempList[i]);
			}
		}
	
		int currentCount = playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Count;

		if(playerGameObject.GetComponent<SpritePhysics>().touchedGravityVolumes.Count==0)
			playerGameObject.GetComponent<SpritePhysics>().isInGravityVolume = false;

		Debug.Log ("exit");
	}
}
