using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GrandNecromancer : Boss 
{
	public float spawnInterval = 0.3f;
	public GameObject singleFormation, bigFormation, zombie;
	bool isShielded = false;

	public override void Start ()
	{
		base.Start ();
		StartCoroutine (spawningZombies ());
	}

	IEnumerator spawningZombies()
	{
		while(true)
		{
			if(!IsDead)
			{
				if(!isShielded)
					BeansSpawner.Instance.spawnFormation(singleFormation, zombie);

				yield return new WaitForSeconds(spawnInterval);
			}
			else break;
		}
	}

	public override bool canMove ()
	{
		return base.canMove () & !isShielded;
	}

	public override void OnMouseDown ()
	{
		base.OnMouseDown ();

		if (!IsDead && !isShielded)
			placeShield (true);
	}

	void placeShield(bool on)
	{
		if(on)
		{
			animator.SetTrigger("inShield");
			BeansSpawner.Instance.spawnFormation (bigFormation, zombie);
			StartCoroutine(checkIfGuardiansAlive());
		}
		else animator.SetTrigger("outShield");
	}

	public void setIsShielded(int one)
	{
		isShielded = (one > 0);
	}

	IEnumerator checkIfGuardiansAlive()
	{
		while(true)
		{
			Bean[] beans = GameObject.FindObjectsOfType<Bean>().Where (b=>!b.IsDead).Where(g=>g.GetComponent<Boss>() == null).ToArray() as Bean[];

			if(beans.Length == 0)
			{
				placeShield(false);
				break;
			}
			else
				yield return new WaitForSeconds(0.25f);
		}
	}

	public override bool canBeDamaged ()
	{
		return base.canBeDamaged () && !isShielded;
	}

	public override void die ()
	{
		base.die ();

		Bean[] zombie = GameObject.FindGameObjectsWithTag ("zombie").Where (b => !b.GetComponent<Bean>().IsDead).Select(b=>b.GetComponent<Bean>()).ToArray() as Bean[];

		foreach (Bean b in zombie)
			b.doDamage (1000);

		GameManager.Instance.setAchievementStatus (17, true);
	}
}
