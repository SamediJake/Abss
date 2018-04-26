using UnityEngine;
using System.Collections;

public class Lupa : Skill 
{
	public float speed = 3.5f;
	public float delay = 0.5f;

	int killedByLupa = 0;

	bool canKill = false;

	public bool CanKill
	{
		get{return canKill;}
		set{canKill = true; 
			if(canKill)
			{				
				GetComponent<SpriteRenderer>().enabled = true;
				GetComponent<BoxCollider2D>().enabled = true;
			}
		}
	}

	public override void Start()
	{
		base.Start ();
		product.skillItemSpawned = true;
		StartCoroutine (followMouse ());
		product.CurDuration = 1f;
	}

	IEnumerator followMouse()
	{
		while(true)
		{
			if(!CanKill)
			{
				Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				v = new Vector3(v.x, v.y, 0f);
				transform.position = v;
				yield return new WaitForSeconds(0.02f);
			}
			else
				break;
		}
	}

	public override void execute ()
	{
		base.execute ();

		for (int i = 0; i < transform.childCount; i++)
			Destroy (transform.GetChild (i).gameObject);

		CanKill = true;
	}

	void Update()
	{
		if(CanKill)
			transform.position = transform.position + new Vector3(0f, speed * Time.deltaTime, 0f);
	}

	void OnTriggerEnter2D(Collider2D coll2d)
	{
		Bean b = coll2d.gameObject.GetComponent<Bean> ();

		if (CanKill)
			if (b != null && coll2d.GetComponent<Boss> () == null)
				if (!b.IsDead) 
				{
					b.doDamage (444);

					killedByLupa++;

					if(killedByLupa > 4)
						GameManager.Instance.setAchievementStatus(9, true);
				}
	}
}
