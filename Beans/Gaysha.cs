using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Gaysha : Boss 
{
	public List<Suriken> surikens = new List<Suriken> ();

	public float vulnerablePeriod;

	public GameObject suriken, formation, beanPrefab;

	public Transform surikenParent;

	public enum GayshaPhase
	{
		Walk, Protected
	}

	public GayshaPhase phase;

	public GayshaPhase Phase
	{
		get{ return phase;}
		set
		{ 
			phase = value;

			if(phase == GayshaPhase.Protected)
			{
				animator.SetTrigger("gayshaInCover");
			}
			else
			{
				animator.SetTrigger("gayshaOutCover");
				StartCoroutine(moving());
			}
		}
	}

	void spawnSurikens()
	{
		surikens.Clear ();
		for (int i = 0; i < surikenParent.transform.childCount; i++)
		{
			GameObject surik = Instantiate(suriken, 
			                               surikenParent.transform.GetChild(i).transform.position, 
			                               surikenParent.transform.GetChild(i).transform.rotation) as GameObject;

			surik.GetComponent<Suriken>().gaysha = this;
			surikens.Add(surik.GetComponent<Suriken>());
		}
	}

	public void removeSuriken(Suriken surik)
	{
		surikens.Remove (surik);

		if (surikens.Count < 1)
			spawnGuardians ();
	}

	public override void OnMouseDown ()
	{
		if(Phase == GayshaPhase.Walk)
		{
			base.OnMouseDown();

			if(CurHp > 0)
			{
				spawnSurikens();
				Phase = GayshaPhase.Protected;
			}
		}
	}

	void spawnGuardians()
	{
		//GetComponent<SpriteRenderer> ().enabled = false;
		BeansSpawner.Instance.spawnFormation (formation, beanPrefab, transform.parent.transform.position);
		StartCoroutine (checkIfGuardiansAlive ());
	}

	IEnumerator checkIfGuardiansAlive()
	{
		while(true)
		{
			Ninja[] beans = GameObject.FindObjectsOfType<Ninja>().Where(b=>!b.IsDead).ToArray() as Ninja[];

			if(beans.Length > 0)
				yield return new WaitForSeconds(0.05f);
			else
			{
				Phase = GayshaPhase.Walk;
				break;
			}
		}
	}

	public override bool canMove ()
	{
		return (phase == GayshaPhase.Walk);
	}

	public override void die ()
	{	
		foreach (Suriken su in surikens)
			Destroy (su.gameObject);

		surikens.Clear ();

		base.die ();

		GameManager.Instance.setAchievementStatus (15, true);
	}
}
