using UnityEngine;
using System.Collections;

public class MainMenuButton : MonoBehaviour 
{
	public float rotationDegree = 0;
	public string functionName = "";

	public virtual void OnMouseDown()
	{
		MainMenu.Instance.Invoke (functionName, 0f);
	}
}
