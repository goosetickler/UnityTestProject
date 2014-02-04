using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour {
	public string LevelName = "";
	public float OrthoZoomOutLevel = 10f;
	public GameObject UI;	

	[HideInInspector]
	public Transform cameraStart;	
	[HideInInspector]
	public Transform playerStart;	
	// Use this for initialization
	void Start () {
		cameraStart = transform.Find("CameraStart");
		playerStart = transform.Find("PlayerStart");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
