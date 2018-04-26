using UnityEngine;
using System.Collections;
using System.Linq;

public class CatClaws : MonoBehaviour 
{
	public Sprite withClaws, noClaws;

	void Start()
	{
		Bean[] beans = FindObjectsOfType<Bean> ().Where(b=>b.IsDead == false).ToArray();
		int rand = Random.Range (0, beans.Length);

		float y = 0f;
		float x = 0f;

		if (beans.Length > 0)
			x = beans [0].transform.position.x;

		for(int i = 0; i < beans.Length; i++)
			if(beans[i].transform.position.x < x)
				x = beans[i].transform.position.x;

		if (beans.Length > 0)
			y = beans [rand].transform.position.y;

		transform.position = new Vector2(x,y);
		StartCoroutine(move (5));
	}
	IEnumerator move(float dur)
	{
		while(true)
		{
			dur -= 0.05f;

			if(GetComponent<SpriteRenderer>().sprite != withClaws)
			{
				if(dur <= 4.9f)
				{
					GetComponent<SpriteRenderer> ().sprite = withClaws;
				}
			}
			else
				transform.position = new Vector2(transform.position.x + 3f, transform.position.y);

			if(dur > 0)
				yield return new WaitForSeconds(0.05f);
			else
			{
				Destroy(gameObject);
				break;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D coll2d)
	{
		Bean b = coll2d.GetComponent<Bean> ();
		Boss boss = coll2d.GetComponent<Boss> ();

		if (b != null && boss == null)
			if (!b.IsDead)
				b.doDamage (222);
	}
}
