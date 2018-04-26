using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour 
{
	public float duration = 10f;
	public Product product;
	bool durationDecreasing = false;

	public float Duration
	{
		get{ return duration;}
		set
		{
			duration = value;

			if(product.maxDuration > 0)
				if(duration <= 0f)
					Destroy(gameObject);
		}
	}

	public virtual void execute()
	{
		GameManager.Instance.GetComponent<Statistics> ().addSkillUsed (product.index);
	}

	public IEnumerator durationDecrease()
	{
		if(!durationDecreasing)
		{
			durationDecreasing = true;

			while(true)
			{
				Duration--;
				yield return new WaitForSeconds(1f);
			}
		}
	}

	public void OnDestroy(){product.skillItemSpawned = false;}

	public virtual void Start(){product.CurDuration = duration;}
}
