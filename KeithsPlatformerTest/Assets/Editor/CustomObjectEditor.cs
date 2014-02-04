using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor( typeof(CustomObject))]
public class CustomObjectEditor : Editor 
{
	GameObject closestGameObject;
	Quaternion rotateAngles;
	Vector3 snapObject;
	Vector3 scale;
	//int moveDistance;
	//float rotationSnap;

	static float rotationSnap
	{
		get { return EditorPrefs.GetFloat("CustomObject_rotationSnap", 22.5f); }
		set { EditorPrefs.SetFloat("CustomObject_rotationSnap", value); }
	}
	static float scaleSize
	{
		get { return EditorPrefs.GetFloat("CustomObject_scaleSize", .25f); }
		set { EditorPrefs.SetFloat("CustomObject_scaleSize", value); }
	}
	static float moveDistance
	{
		get { return EditorPrefs.GetFloat("CustomObject_moveDistance", 1f); }
		set { EditorPrefs.SetFloat("CustomObject_moveDistance", value); }
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var gameObjects = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
		Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);

/*
		var spriteObject = target as CustomObject;
		var spriteRenderer = spriteObject.GetComponent<SpriteRenderer>(); 
		var COLORS = spriteObject.GetComponent<CustomObject>().currentColor;
		bool renderObject = spriteObject.GetComponent<CustomObject>().isOn;
		//Toggles the Object renderer On/Off from a bool that is set on the gameobject
		if(!renderObject)
		{
			spriteRenderer.renderer.enabled = false;
		}
		else
			spriteRenderer.renderer.enabled = true;

		//Use Enum dropdown menu to set the color of the object
		switch( COLORS )
		{
		case CustomObject.COLORS.Red:
			spriteRenderer.color = Color.red;
			break;

		case CustomObject.COLORS.Yellow:
			spriteRenderer.color = Color.yellow;
			break;
			
		case CustomObject.COLORS.Blue:
			spriteRenderer.color = Color.blue;
			break;

		case CustomObject.COLORS.Green:
			spriteRenderer.color = Color.green;
			break;

		case CustomObject.COLORS.White:
			spriteRenderer.color = Color.white;
			break;
		}
*/


		rotationSnap = EditorGUILayout.FloatField( "Rotation Snap", rotationSnap );
		scaleSize = EditorGUILayout.FloatField( "Scale", scaleSize );
		moveDistance = EditorGUILayout.FloatField( "Move Distance", moveDistance );

		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "(0,0,0)"))
		{
			foreach (Transform transform in transforms)
			{
				Undo.RecordObject( transform, "Reset to Origin");
				transform.position = new Vector3(0, 0, 0);
			}
		}
		if(GUILayout.Button( "Nudge X"))
		{
			foreach (Transform transform in transforms)
			{
				Undo.RecordObject( transform, "Nudge X");
				if( Event.current.shift)
					transform.position = new Vector3(transform.position.x - moveDistance, transform.position.y, transform.position.z);
				else
					transform.position = new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z);
			}
		}
		if(GUILayout.Button( "Nudge Y"))
		{
			foreach (Transform transform in transforms)
			{
				Undo.RecordObject( transform, "Nudge Y");
				if( Event.current.shift)
					transform.position = new Vector3(transform.position.x, transform.position.y - moveDistance, transform.position.z);
				else
					transform.position = new Vector3(transform.position.x, transform.position.y + moveDistance, transform.position.z);
			}
		}
		if(GUILayout.Button( "Set Z Depth"))
		{
			foreach (Transform transform in transforms)
			{
				Undo.RecordObject( transform, "Nudge Z");
				if( Event.current.shift)
					transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z - moveDistance);
				else
					transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z + moveDistance);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "Rotate"))
		{
			if( Event.current.shift )
				rotateAngles = Quaternion.Euler(0, 0, -rotationSnap);
			else
				rotateAngles = Quaternion.Euler(0, 0, rotationSnap);
			foreach (Transform transform in transforms)
			{
				Quaternion currentAngles = transform.rotation;
				Quaternion updatedAngles = (rotateAngles * currentAngles);
				Undo.RecordObject( transform, "Rotate Object");
				transform.rotation = updatedAngles;
			}
		}

		if(GUILayout.Button( "Reset Rotation"))
		{
			foreach (Transform transform in transforms)
			{
				Undo.RecordObject( transform, "Reset Rotation");
				transform.rotation = Quaternion.identity;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "Mirror X Axis"))
		{
			Vector3 xScale = new Vector3(-1, 1, 1);
			foreach (Transform transform in transforms)
			{
				Vector3 currentScale = transform.localScale;
				Vector3 mirrorScale = Vector3.Scale(currentScale,xScale);
				Undo.RecordObject( transform, "Mirror X");
				transform.localScale = mirrorScale;
			}
		}
		if(GUILayout.Button( "Mirror Y Axis"))
		{
			Vector3 yScale = new Vector3(1, -1, 1);
			foreach (Transform transform in transforms)
			{
				Vector3 currentScale = transform.localScale;
				Vector3 mirrorScale = Vector3.Scale(currentScale,yScale);
				Undo.RecordObject( transform, "Mirror Y");
				transform.localScale = mirrorScale;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "Uniform Scale"))
		{
			foreach (Transform transform in transforms)
			{
				Vector3 currentScale = transform.localScale;
				scale = new Vector3(scaleSize, scaleSize, 0);
				if( Event.current.shift )
				{
					scale = currentScale -= scale;
				}
				else
				{
					scale = currentScale += scale;
				}
				Undo.RecordObject( transform, "Scale Object");
				transform.localScale = scale;
			}
		}
		if(GUILayout.Button( "Scale X"))
		{
			foreach (Transform transform in transforms)
			{
				Vector3 currentScale = transform.localScale;
				scale = new Vector3(scaleSize, 0, 0);
				if( Event.current.shift )
				{
					scale = currentScale -= scale;
				}
				else
				{
					scale = currentScale += scale;
				}
				Undo.RecordObject( transform, "Scale Object");
				transform.localScale = scale;
			}
		}
		if(GUILayout.Button( "Scale Y"))
		{
			foreach (Transform transform in transforms)
			{
				Vector3 currentScale = transform.localScale;
				scale = new Vector3(0, scaleSize, 0);
				if( Event.current.shift )
				{
					scale = currentScale -= scale;
				}
				else
				{
					scale = currentScale += scale;
				}
				Undo.RecordObject( transform, "Scale Object");
				transform.localScale = scale;
			}
		}
		if(GUILayout.Button( "Reset Scale"))
		{
			foreach (Transform transform in transforms)
			{
				Undo.RecordObject( transform, "Reset Scale");
				transform.localScale = Vector3.one;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "Snap to Closest GameObject"))
		{
			//TODO Let people enter in the tag they want to snap to 
			foreach (GameObject gameObject in gameObjects)
			{
				GameObject[] nearbyGameObjects = GameObject.FindGameObjectsWithTag("Ground");
				float distance = Mathf.Infinity;
				Vector3 position = gameObject.transform.position;
				foreach (GameObject go in nearbyGameObjects) 
				{
					if ( gameObject != go) 
					{
						if((go.transform.IsChildOf(gameObject.transform)))
							continue;
						else
						{
							Vector3 diff = go.transform.position - position;
							float curDistance = diff.sqrMagnitude;
							if (curDistance < distance) 
							{
								closestGameObject = go;
								distance = curDistance;
							}
						}
					}
				}
				Undo.RecordObject( gameObject.transform, "Snap to Nearby Object");
				gameObject.transform.position = closestGameObject.transform.position;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "Snap to Grid"))
		{
			float gridSize = 1.0f;
			foreach (Transform transform in transforms)
			{
				Vector3 gridPosition = transform.position;
				gridPosition.x = Mathf.Round(gridPosition.x / gridSize) * gridSize;
				gridPosition.y = Mathf.Round(gridPosition.y / gridSize) * gridSize;
				gridPosition.z = Mathf.Round(gridPosition.z / gridSize) * gridSize;
				Undo.RecordObject( transform, "Snap to Grid");
				transform.position = gridPosition;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button( "Snap to Ground"))
		{
			foreach (Transform transform in transforms)
			{
				RaycastHit2D hit = new RaycastHit2D();
				hit = Physics2D.Raycast( transform.position, -Vector2.up);
				if( hit )
				{
					Undo.RecordObject( transform, "Snap to Ground");
					transform.position = new Vector3( transform.position.x, hit.point.y, transform.position.z );
				}
			}
		}
		GUILayout.EndHorizontal();
	}
}
