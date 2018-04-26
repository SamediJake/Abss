using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Devil : Boss 
{
	bool hasAppeared = false;

	public GameObject formation, single;

	public GameObject fiend;

	public List<Bean> fiends = new List<Bean>();

	public override void Start ()
	{
		base.Start ();

		List<GameObject> beans = new List<GameObject>();

		for(int i = 0; i < formation.transform.childCount; i++)
			beans.Add(fiend);

		BeansSpawner.Instance.spawnFormation (formation, beans, transform.position);

		findAllFiends ();

		StartCoroutine (checkIfFiendsAreDead ());
	}

	void findAllFiends()
	{
		GameObject[] fien = GameObject.FindGameObjectsWithTag ("Fiend").Where (b => b.transform.GetChild (0).GetComponent<Bean>().IsDead == false).ToArray ()  as GameObject[];

		fiends.Clear ();

		foreach (GameObject go in fien)
				fiends.Add (go.transform.GetChild (0).GetComponent<Bean> ());
	}

	IEnumerator checkIfFiendsAreDead()
	{
		while(true)
		{
			if(!IsDead)
			{
				for(int i = 0; i < fiends.Count; i++)
				    if(fiends[i] != null)
					    if(fiends[i].IsDead)
						{
							BeansSpawner.Instance.spawnFormation(single, fiend.gameObject, fiends[i].transform.position);
							findAllFiends();
						}

				yield return new WaitForSeconds(0.2f);
			}
			else break;
		}
	}

	public override IEnumerator moving ()
	{
		while(true)
		{
			if(!isDead)
			{
				if(canMove())
				{
					transform.parent.transform.position = new Vector2 (transform.parent.transform.position.x, transform.parent.transform.position.y - speed/12f);
				}
				yield return new WaitForSeconds(0.05f);
			}
			else break;
		}
	}

	public override void OnMouseDown ()
	{
		base.OnMouseDown ();

		if(!IsDead)
		{
			Bean[] f = fiends.Where (b => !b.IsDead && b != null && Vector2.Distance(b.transform.position, GameObject.FindGameObjectWithTag("lowerBorder").transform.position) > 1f).ToArray () as Bean[];
		
			if(f.Length > 0)
			{
				int index = Random.Range(0, f.Length+1);

				Vector3 devilTemp = this.transform.position;

				if(fiends[index] != null)
	            {
					transform.position = fiends[index].transform.position;

					fiends[index].transform.position = devilTemp;
		        }
			}
		}
	}

	public override bool canMove ()
	{
		return base.canMove() && hasAppeared;
	}

	public void Appeared()
	{
		hasAppeared = true;
	}

	public override void die ()
	{
		base.die ();

		foreach (Bean b in fiends)
			b.doDamage (1000f);

		GameManager.Instance.setAchievementStatus (18, true);
	}
}
