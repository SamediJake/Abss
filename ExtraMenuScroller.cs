using UnityEngine;
using System.Collections;

public class ExtraMenuScroller : MonoBehaviour 
{
	bool isMousePressed = false;

	float step = 0.01f;

	float mouseY = 0f;

	public float granMin = 0.0f, granMax = 0.0f;

	void OnMouseDown()
	{
		if(MainMenu.Instance.canScrollExtraMenu)
			isMousePressed = true;
	}

	void Update()
	{
		if(isMousePressed)
		{
			if(Input.mousePosition.y != mouseY)
			{
				int multiplier = 1;

				if(Input.mousePosition.y < mouseY)
					multiplier = -1;

				mouseY = Input.mousePosition.y;

				Vector2 curOffset = GetComponent<MeshRenderer> ().material.mainTextureOffset;

				if(!(curOffset.y >= granMax && multiplier == 1) && !(curOffset.y <= granMin && multiplier == -1))
				GetComponent<MeshRenderer> ().material.mainTextureOffset = new Vector2(curOffset.x, curOffset.y + step* multiplier);
			}
		}
	}

	void OnMouseUp()
	{
		isMousePressed = false;
	}
}
