using UnityEngine;
using System.Collections;

public class Bean : MonoBehaviour 
{	
	public int minSoul = 1, maxSoul = 2, damage = 1, desiredScore = 0;
	public float speed = 2f, timeToDestroy = 15f, leftToDestroy = 15f, curHp = 1, maxHp = 1;

	public Sprite deathSprite;

	public Necromancer reviver;

	public bool isDead = false, canBeRessurected = true, freeToSpawn = true;

	Formation owner;

	Statistics statistics;

	//[HideInInspector]
	public Animator animator;

	public bool IsDead
	{get{return isDead;}}
	
	public bool CanBeRessurected
	{get{return canBeRessurected;}
		set{canBeRessurected = value;}}

	public int Damage{get{return damage;} set{damage = value;}}

	public Formation Owner
	{
		get{return owner;}
		set{owner = value;owner.beans.Add(this);}
	}

	public float CurHp
	{
		get{return curHp;}
		set{curHp = value; if(curHp <= 0) die ();}
	}

	public virtual void doDamage(float damage)
	{
		if (CurHp > 0)
			CurHp -= damage;
	}

	public virtual void Start()
	{
		animator = GetComponent<Animator> ();

		if(!IsDead)
		{
			//curHp = maxHp;
			leftToDestroy = timeToDestroy;
			StartCoroutine (moving());
		}
	}

	public virtual void OnMouseDown()
	{
		if(!IsDead && Time.timeScale >= 1 && canBeDamaged())
		{
			doDamage (1);

			pushBack();
		}
	}

	public virtual void pushBack()
	{
		if(!IsDead)
			transform.parent.transform.position = new Vector2(transform.parent.transform.position.x, transform.parent.transform.position.y + 1);
	}

	public virtual void die()
	{
		if(!isDead)
		{
			if(animator == null)
				animator = GetComponent<Animator> ();

			animator.SetTrigger("death");

			lowestSortingOrder();

			isDead = true;

			int reward = Random.Range(minSoul, maxSoul+1);

			GameManager.Instance.Souls += reward;
			GameManager.Instance.score += reward;

			minSoul = 0; maxSoul = 0;

			foreach(Collider2D coll in GetComponentsInChildren<Collider2D>())
				Destroy(coll);

			GameManager.Instance.GetComponent<Statistics> ().addKilledBean(BeansSpawner.Instance.getBeanIndex(this), this);
		}
	}

	public void lowestSortingOrder()
	{
		foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
			s.sortingLayerName = "Corpses";
	}

	public virtual IEnumerator moving()
	{
		while(true)
		{
			if(!IsDead)
			{
				if(canMove())
					animator.SetTrigger("jump");
				yield return new WaitForSeconds (0.5f);
			}
			else
				break;
		}
	}

	public virtual bool canMove()
	{
		return true;
	}

	public virtual bool canBeDamaged()
	{
		return true;
	}

	void changePos()
	{
		transform.parent.transform.position = new Vector2 (transform.parent.transform.position.x, transform.parent.transform.position.y - speed/2);
	}

	public virtual void getdata(BeanSave bs)
	{
		minSoul = bs.minSoul;
		maxSoul = bs.maxSoul;
		Damage = bs.damage;
		speed = bs.speed;
		timeToDestroy = bs.timeToDestroy;
		leftToDestroy = bs.leftToDestroy;
		CurHp = bs.curHp;
		maxHp = bs.maxHp;
		canBeRessurected = bs.canBeRessurected;
		transform.position = new Vector3 (bs.posx, bs.posy, bs.posz);
	}

	public void getDeathAnim()
	{
		animator.enabled = false;

		Rigidbody2D rgb = GetComponent<Rigidbody2D> ();

		if (rgb != null)
			Destroy (rgb);

		GetComponent<SpriteRenderer> ().sprite = deathSprite;
		StartCoroutine(selfDestruction());
	}

	public IEnumerator reviving()
	{
		while(true)
		{
			if(!IsDead)
			{
				if(!reviver.IsDead && reviver != null)
				{
					yield return new WaitForSeconds(0.01f);
				}
				else
				{
					AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);

					if(asi.IsName("reviving"))
					{
						animator.Play("reviving_cancel", -1, asi.normalizedTime);
						die();
					}
					break;
				}
			}
			else break;
		}
	}

	IEnumerator selfDestruction()
	{
		while(true)
		{
			if(leftToDestroy > 0f)
			{
				Color oldColor = GetComponent<SpriteRenderer>().color;
				float alpha = leftToDestroy/timeToDestroy;
				GetComponent<SpriteRenderer>().color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
				leftToDestroy--;
				yield return new WaitForSeconds(1f);
			}
			else
			{
				Destroy(gameObject);
				break;
			}
		}
	}

	public virtual BeanSave makeSaveData()
	{
		BeanSave bs = new BeanSave ();
		bs.getdata (this);
		return bs;
	}
}

[System.Serializable]
public class BeanSave
{
	public int minSoul = 1, maxSoul = 2, damage = 1;
	public float speed = 2f, timeToDestroy = 15f, leftToDestroy = 15f, curHp = 0, maxHp = 0, 
	posx = 0f, posy = 0f, posz = 0f, colorr = 0f, colorg = 0f, colorb = 0f, colora = 0f;

	public bool canBeRessurected = true;
	public string name = "", owner = "";

	public virtual void getdata(Bean bean)
	{
		name = bean.name;
		minSoul = bean.minSoul;
		maxSoul = bean.maxSoul;
		damage = bean.Damage;
		speed = bean.speed;
		timeToDestroy = bean.timeToDestroy;
		leftToDestroy = bean.leftToDestroy;
		curHp = bean.CurHp;
		maxHp = bean.maxHp;
		canBeRessurected = bean.CanBeRessurected;
		owner = bean.name;
		posx = bean.transform.position.x;
		posy = bean.transform.position.y;
		posz = bean.transform.position.z;
		Color col = bean.GetComponent<SpriteRenderer>().color;
		colorr = col.r; colorg = col.g; colorb = col.b; colora = col.a; 
	}
}
