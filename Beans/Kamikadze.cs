using UnityEngine;
using System.Collections;
using System.Linq;

public class Kamikadze : Bean 
{
	public int ExplosionDamage = 3;
	public float explosionRadius = 2f;

	public override void die ()
	{
		base.die ();

		Bean[] beans = FindObjectsOfType<Bean> ().Where (b => Vector2.Distance (transform.position, b.transform.position) <= explosionRadius)
			.Where (b => !b.IsDead).ToArray () as Bean[];

		foreach (Bean b in beans)
			b.doDamage (Damage);

		foreach (Bean b in beans)
			if (b != null)
				if (b.IsDead)
					GameManager.Instance.KilledByExplosionAmount ++;

		Damage = 0;
	}

	public override void getdata (BeanSave bs)
	{
		base.getdata (bs);
		KamikadzeSave cs = bs as KamikadzeSave;
		
		ExplosionDamage = cs.ExplosionDamage;
		explosionRadius = cs.explosionRadius;
	}

	public override BeanSave makeSaveData()
	{
		KamikadzeSave bs = new KamikadzeSave ();
		bs.getdata (this);
		return bs;
	}
}

[System.Serializable]
public class KamikadzeSave: BeanSave
{
	public int ExplosionDamage = 0; 
	public float explosionRadius = 0f;

	public override void getdata (Bean bean)
	{
		base.getdata (bean);
		Kamikadze cent = bean as Kamikadze;
		
		ExplosionDamage = cent.ExplosionDamage;
		explosionRadius = cent.explosionRadius;
	}
}
