using UnityEngine;
using System.Collections;
using System.Linq;

public class FlyKiller : Skill 
{
	public override void Start()
	{
		base.Start ();
		product.CurDuration = 1f;
		StartCoroutine (followMouse ());
	}

	IEnumerator followMouse()
	{
		while(true)
		{
			Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			v = new Vector3(v.x, v.y, 0f);
			transform.position = v;
			yield return new WaitForSeconds(0.02f);
		}
	}

	public override void execute()
	{
		base.execute ();

		Bean[] beans = FindObjectsOfType<Bean>().Where (b=>b.GetComponent<Boss>() == null).Where(b=>!b.IsDead).Where (b=>b.gameObject.GetComponent<BoxCollider2D>().bounds.Intersects(GetComponent<BoxCollider2D>().bounds)).ToArray() as Bean[];
		
		print (beans.Length);
		
		foreach(Bean b in beans)
			b.doDamage (333);

		product.CurDuration = 0f;

		Destroy (gameObject);
	}
}
