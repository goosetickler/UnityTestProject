using UnityEngine;
using System.Collections;

public class AdditiveColorSwap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space))
			renderer.material.color = Color.white;
	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		Color pColor = renderer.material.GetColor("_Color");
		Color oColor = col.renderer.material.GetColor("_Color");
		if( pColor != oColor && col.tag != "Switch" )
		{
			print( "Combine Colors" );

			if(pColor.r == 1 && pColor.g == 1 && pColor.b == 1 )
			{
				renderer.material.color = oColor;
			}
			else
			{
				float r3 = (pColor.r + oColor.r) *.5f;
				float g3 = (pColor.g + oColor.g) *.5f;
				float b3 = (pColor.b + oColor.b) *.5f;
				Color mergedColor = new Color( r3, g3, b3, 255 );
				print( mergedColor);
				renderer.material.color = mergedColor;
			}
		}
	}
}
