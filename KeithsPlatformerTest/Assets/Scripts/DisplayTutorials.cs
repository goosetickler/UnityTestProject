using UnityEngine;
using System.Collections;

public class DisplayTutorials : MonoBehaviour {
	public GameObject movementTutorial;
	public GameObject jumpTutorial;
	// Use this for initialization
	void Start () {
		movementTutorial.renderer.enabled = false;
		jumpTutorial.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void displayTutorials()
	{
		Invoke( "showText", 2.75f);
	}

	void showText()
	{
		movementTutorial.renderer.enabled = true;
		jumpTutorial.renderer.enabled = true;
	}
}
