using UnityEngine;
using System.Collections;

public class Legionnaire : Bean 
{
	[SerializeField]
	float curShieldHp = 3, maxShieldHp = 3;

	public GameObject shield;

	public float MaxShieldHp{get{return maxShieldHp;}}

	public float CurShieldHp
	{
		get{return curShieldHp;}
		set
		{
			curShieldHp = value;

			shield.GetComponent<Animator>().SetInteger("shieldHP", (int)curShieldHp);

			if(curShieldHp <= 0)
			{
				curShieldHp = 0;
			}
		}
	}

	public override void doDamage (float damage)
	{
		if(CurShieldHp > 0)
		{
			float temp = damage;

			damage -= CurShieldHp;

			CurShieldHp -= temp;
		}

		if(damage > 0)
			base.doDamage (damage);
	}

	public override void Start ()
	{
		base.Start ();
		curShieldHp = maxShieldHp;
	}

	public override void die ()
	{
		base.die ();

		if (CurShieldHp > 0)
			CurShieldHp = 0;
	}

	public override void getdata (BeanSave bs)
	{
		base.getdata (bs);
		LeggionaireSave cs = bs as LeggionaireSave;
		
		CurShieldHp = cs.curShieldHp;
		maxShieldHp = cs.maxShieldHp;
	}

	public override BeanSave makeSaveData()
	{
		LeggionaireSave bs = new LeggionaireSave ();
		bs.getdata (this);
		return bs;
	}
}

[System.Serializable]
public class LeggionaireSave: BeanSave
{
	public float curShieldHp = 0f, maxShieldHp = 0f;

	public override void getdata (Bean bean)
	{
		Legionnaire cent = bean as Legionnaire;
		curShieldHp = cent.CurShieldHp;
		maxShieldHp = cent.MaxShieldHp;
		base.getdata (bean);
	}
}
