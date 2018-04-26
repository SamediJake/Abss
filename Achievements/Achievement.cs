using UnityEngine;
using System.Collections;

public class Achievement : MonoBehaviour 
{
	public int achId = 0;

	[SerializeField]
	bool isUnlocked = false, wasShown = false;
	public string nameKey, descKey;

	public bool IsUnlocked 
	{
		get{ return isUnlocked;}
		set
		{ 
			isUnlocked = value;

			if(!wasShown)
			{
				//показати текст з появою ачівки
				wasShown = true;
			}
		}
	}
}
