using UnityEngine;
using System.Collections;

public class Cat : Skill 
{
	public float period = 5f;
	public GameObject catClaws;
	
	public override void Start()
	{
		base.Start ();
		StartCoroutine (durationDecrease ());
		product.skillItemSpawned = true;
		product.StartCoroutine ("handleUsebar");
		StartCoroutine (doSkills ());
		base.execute ();
	}
	
	IEnumerator doSkills()
	{
		while(true)
		{
			GameObject g = Instantiate(catClaws) as GameObject;

			g.transform.SetParent(transform);

			yield return new WaitForSeconds(period);
		}
	}
}
