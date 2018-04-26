using UnityEngine;
using System.Collections;

public class MainMenuCheckbox : MainMenuButton 
{
	bool on = true;
	public Sprite sprOn, sprOff;
	public SpriteRenderer tickRenderer;

	public bool On
	{
		get{return on;}
		set{on = value; 
			if(on)tickRenderer.sprite = sprOn; 
			else tickRenderer.sprite = sprOff;}
	}

	public override void OnMouseDown ()
	{
		base.OnMouseDown ();
		On = !On;
	}
}
