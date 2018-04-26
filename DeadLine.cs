using UnityEngine;

public class DeadLine : MonoBehaviour 
{
	public bool isUpperBorder = false;

	void OnTriggerEnter2D(Collider2D collider2d)
	{
		if(!isUpperBorder)
		{
			Bean b = collider2d.gameObject.GetComponent<Bean>();

			if (b != null)
			{
				GameManager.Instance.LifeRemains -= b.Damage;
				Destroy(b.gameObject, 3.5f);
			}
			else
			{
				Suriken surk = collider2d.gameObject.GetComponent<Suriken>();

				if(surk != null)
				{
					GameManager.Instance.LifeRemains -= surk.damage;
					surk.gaysha.removeSuriken(surk);
					Destroy(surk.gameObject);
				}
			}
		}
		else
		{
			Lupa l = collider2d.gameObject.GetComponent<Lupa>();
			
			if (l != null)
			{
				l.product.CurDuration = 0f;
				Destroy(l.gameObject);
			}
		}
	}
}