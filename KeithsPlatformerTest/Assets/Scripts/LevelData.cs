using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour {
	public string LevelName = "";
	public string LevelNameSub = "";
	public string LevelNameSub2 = "";
	public float OrthoZoomOutLevel = 10f;
	public GameObject UI;	
	public GameObject UISub;	
	public GameObject UISub2;	
	
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
