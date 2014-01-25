using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor( typeof(TintSpriteColor))]
public class UpdateSpritesColorInEditor : Editor 
{
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var gameObjects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Unfiltered);
		
		foreach (GameObject gameObject in gameObjects)
		{
			var COLORS = gameObject.GetComponent<TintSpriteColor>().currentColor;
			bool renderObject = gameObject.GetComponent<TintSpriteColor>().isOn;
			var spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); 
			
			if( spriteRenderer != null)
			{
				if(!renderObject)
				{
					spriteRenderer.renderer.enabled = false;
				}
				else
					spriteRenderer.renderer.enabled = true;

				switch( COLORS )
				{
				case TintSpriteColor.COLORS.Red:
					spriteRenderer.color = Color.red;
					break;
					
				case TintSpriteColor.COLORS.Yellow:
					spriteRenderer.color = Color.yellow;
					break;
					
				case TintSpriteColor.COLORS.Blue:
					spriteRenderer.color = Color.blue;
					break;
					
				case TintSpriteColor.COLORS.Green:
					spriteRenderer.color = Color.green;
					break;
					
				case TintSpriteColor.COLORS.White:
					spriteRenderer.color = Color.white;
					break;
				}

				gameObject.name = COLORS + " Light";
			}
		}
	}
}