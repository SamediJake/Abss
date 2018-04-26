using UnityEngine;
using System.Collections;

public class TotalDestroyer : Skill 
{
	public override void Start()
	{
		Bean[] beans = FindObjectsOfType<Bean> () as Bean[];

		foreach (Bean b in beans)
			b.doDamage (555);

		Destroy (gameObject, 1f);
	}
}
