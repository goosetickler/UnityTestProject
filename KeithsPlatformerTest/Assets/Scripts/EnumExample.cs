using UnityEngine;
using System.Collections;

public class EnumExample : MonoBehaviour {
	public enum directions
	{
		Up,
		Down,
		Left,
		Right
	}
	public directions direction;

	// Use this for initialization
	void Start () {
		switch( direction )
		{
		case directions.Up:
			print( "Up!" );
			break;

		case directions.Down:
			print( "Down!" );
			break;

		case directions.Left:
			print( "Left!" );
			break;

		case directions.Right:
			print( "Right!" );
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
