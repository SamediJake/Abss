using UnityEngine;

public class DeadZone : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D coll2d)
	{
		Bean b = coll2d.GetComponent<Bean> ();

		if (b != null)
			b.doDamage (111);
	}
}
