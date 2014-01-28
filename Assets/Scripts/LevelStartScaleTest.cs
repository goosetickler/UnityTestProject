using UnityEngine;
using System.Collections;

public class LevelStartScaleTest : MonoBehaviour {

	private GameObject[] levelObjects;
	public float scaleDelayMin;
	public float scaleDelayMax;
	// Use this for initialization
	void Start () 
	{
		levelObjects = GameObject.FindObjectsOfType(typeof (GameObject)) as GameObject[];
		Debug.Log ( levelObjects.Length );
		ScaleGameObjects( levelObjects );
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void ScaleGameObjects( GameObject[] Objects )
	{
		//Save the current scaele and then reset the scale of all gameObjects down to 0
		foreach (GameObject obj in Objects) 
		{
			Vector3 currentScale = obj.transform.localScale;
			obj.transform.localScale = new Vector3(0,0,0);
			StartCoroutine(ResetObjectScale(obj, currentScale));
		}
	}

	private IEnumerator ResetObjectScale( GameObject obj, Vector3 scale )
	{
		yield return new WaitForSeconds(Random.Range(scaleDelayMin, scaleDelayMax));
		iTween.ScaleTo( obj, scale, 2f);
	}
}
