using UnityEngine;
using System.Collections;

public class ForceField : MonoBehaviour {
	public string unlockColor;
	public GameObject blockingvolume;
	private bool isLocked = true;
	public AudioClip LockedClip;
	public AudioClip UnlockedClip;
	private AudioSource LockedSource;
	private AudioSource UnlockedSource;

	// Use this for initialization
	void Start () {
	}

	void Awake()
	{
		LockedSource = AddAudio(LockedClip,false,false,1.0f);
		UnlockedSource = AddAudio(UnlockedClip,false,false,1.0f);
	}

	AudioSource AddAudio(AudioClip NewClip, bool IsLoop, bool OnAwake, float Volume)
	{
		
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = NewClip;
		newAudio.loop = IsLoop;
		newAudio.playOnAwake = OnAwake;
		newAudio.volume = Volume;
		
		return newAudio;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col) 
	{ 
		if( col.tag == unlockColor)
		{
			if( isLocked == true )
			{
				//Destroy( blockingvolume );
				UnlockedSource.Play();
				blockingvolume.collider2D.enabled = false;
				isLocked = false;
			}
		}
	}
	void OnTriggerExit2D (Collider2D col) 
	{ 
		if( isLocked == true )
		{
			LockedSource.Play();
		}
		else if( isLocked == false )
		{
			isLocked = true;
		}
		blockingvolume.collider2D.enabled = true;
	}
}

