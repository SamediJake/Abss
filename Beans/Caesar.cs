using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Caesar : Boss 
{
	public GameObject brut, single;

	public GameObject formation, guardian;

	public List<Bean> guardians = new List<Bean> ();

	public override void Start ()
	{
		base.Start ();

		BeansSpawner.Instance.spawnFormation (formation, guardian, transform.position);

		guardians = GameObject.FindGameObjectsWithTag ("caesarGuardian").Select (b => b.transform.GetChild (0).GetComponent<Bean> ()).ToList();

		StartCoroutine (checkIfGuardiansAlive ());
	}

	IEnumerator checkIfGuardiansAlive()
	{
		while(true)
		{
			Bean[] remains = guardians.Where(b => b != null && !b.IsDead).ToArray() as Bean[];

			if(remains.Length > 0)
				yield return new WaitForSeconds(0.05f);
			else
			{
				spawnJuniBrut();
				break;
			}
		}
	}

	void spawnJuniBrut()
	{
		BeansSpawner.Instance.spawnFormation (single, brut);
		
		JuniBrut jb = FindObjectOfType<JuniBrut> ();
		
		if(jb != null)
			jb.transform.position = new Vector2(transform.position.x, jb.transform.position.y);
		
		speed = 0f;
	}

	public override void OnMouseDown ()
	{

	}

	public override void die ()
	{
		base.die ();
		GameManager.Instance.setAchievementStatus (16, true);
	}
}
