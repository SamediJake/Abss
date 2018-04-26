using UnityEngine;
using System.Collections;

public class JuniBrut : Bean 
{
	Transform target;

	public override void Start ()
	{
		base.Start ();

		target = GameObject.FindGameObjectWithTag("juniBrutSlot").transform;

		transform.position = new Vector2(target.position.x, transform.position.y);
	}
	public override void OnMouseDown ()
	{}

	void Update()
	{
		Caesar cs = GameObject.FindObjectOfType<Caesar> ();

		if(target != null)
		{
			if(Mathf.Abs(transform.position.y - target.position.y) <= 0.4f 
			   || Mathf.Abs(transform.position.y - cs.transform.position.y) <= 0.4f)
			{		print (Mathf.Abs (transform.position.y - target.position.y).ToString () + " " + Mathf.Abs (transform.position.y - cs.transform.position.y).ToString ());
				cs.doDamage(10000f);
				Destroy (gameObject);
			}
		}
		else Destroy (this.gameObject);
	}

}
