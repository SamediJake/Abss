using UnityEngine;
using System.Collections;
using System.Linq;

public class Ninja : Bean 
{
	public float dissapearFrequencymin = 2f, dissapearFrequencymax = 3f, toDissapear = 0f, minDistance = 1.5f, maxDistance = 2.4f;

	public bool isDissapearing = false;
	
	public IEnumerator teleport ()
	{
		while(true)
		{
			if(!IsDead)
			{
				if(toDissapear <= 0f)
				{
					animator.Play("ninjaDissapear");
					float tt = Random.Range(dissapearFrequencymin, dissapearFrequencymax);
					toDissapear = tt;
				}
				else toDissapear -= 0.1f;

				yield return new WaitForSeconds(0.1f);
			}
			else
				break;
		}
	}

	public override void Start ()
	{
		base.Start ();
		toDissapear = Random.Range(dissapearFrequencymin, dissapearFrequencymax);
		animator.Play ("ninjaAppear");
		StartCoroutine (teleport ());
	}

	public override IEnumerator moving()
	{
		while(true)
		{
			if(!IsDead)
			{
				//if(!isDissapearing)
					animator.SetTrigger("jump");

				yield return new WaitForSeconds (0.8f);
			}
			else
				break;
		}
	}

	public void switchIsDissapearing()
	{
		isDissapearing = !isDissapearing;
	}

	public void teleportMovement()
	{
		Transform tr = GameObject.FindObjectsOfType<DeadLine> ().Where (b => !b.GetComponent<DeadLine> ().isUpperBorder).ToArray () [0].transform;

		if(tr != null)
			if(Mathf.Abs(transform.parent.position.y - tr.position.y) <= maxDistance)
			{
				transform.parent.position = new Vector2(transform.position.x, tr.position.y);
			}
			else
			{
				transform.parent.position = new Vector2 (transform.parent.position.x, transform.parent.position.y - Random.Range (minDistance, maxDistance));
			}
	}

	public override void getdata (BeanSave bs)
	{
		base.getdata (bs);
		NinjaSave cs = bs as NinjaSave;
		
		dissapearFrequencymin = cs.dissapearFrequencymin;
		dissapearFrequencymax = cs.dissapearFrequencymax;
		minDistance = cs.minDistance;
		maxDistance = cs.maxDistance;
		isDissapearing = cs.isDissapearing;
	}

	public override BeanSave makeSaveData()
	{
		NinjaSave bs = new NinjaSave ();
		bs.getdata (this);
		return bs;
	}
}

[System.Serializable]
public class NinjaSave: BeanSave
{
	public float dissapearFrequencymin = 2f, dissapearFrequencymax = 3f, minDistance = 0f, maxDistance = 0f;
	
	public bool isDissapearing = false;

	public override void getdata (Bean bean)
	{
		base.getdata (bean);
		Ninja cent = bean as Ninja;
		dissapearFrequencymin = cent.dissapearFrequencymin;
		dissapearFrequencymax = cent.dissapearFrequencymax;
		minDistance = cent.minDistance;
		maxDistance = cent.maxDistance;
		isDissapearing = cent.isDissapearing;
	}
}