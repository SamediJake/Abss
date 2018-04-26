using UnityEngine;
using System.Collections;
using System.Linq;

public class Necromancer : Bean 
{
	public int resAmount = 3;
	public float resDelay = 3.5f, timeToNextRes = 3.5f, castTime = 2f, timeLeftToCast = 0f, resRadius = 3f;

	public GameObject zombiePrefab, singleFormation;

	public Bean patient;

	public override IEnumerator moving()
	{
		while(true)
		{
			if(!IsDead)
			{
				timeToNextRes -= 0.5f;
				
				if(resAmount > 0)
					if(timeToNextRes <= 0)
				{
					Bean[] beans; 
					
					if(patient == null)
					{
						beans = FindObjectsOfType<Bean>().Where(b=>b.IsDead)
							.Where(b=>b.CanBeRessurected)
								.Where(b=>Vector2.Distance(b.gameObject.transform.position, transform.position) <= resRadius).ToArray() as Bean[];
						
						if(beans.Length > 0)
						{
							patient = beans[0];

							resAmount--;
							timeLeftToCast = castTime;

							GameObject form = Instantiate(singleFormation, patient.transform.position, Quaternion.identity) as GameObject;

							form.transform.SetParent(BeansSpawner.Instance.transform);

							form.name = form.name.Replace("(Clone)","");

							GameObject zomb = Instantiate(zombiePrefab, form.transform.GetChild(0).transform.position, Quaternion.identity) as GameObject;

							Destroy (form.transform.GetChild(0).gameObject);

							zomb.name = zomb.name.Replace("(Clone)","");

							Bean zombBean = zomb.GetComponentInChildren<Bean>();

							zombBean.Owner = form.GetComponent<Formation>();

							zomb.transform.SetParent(form.transform);

							zombBean.reviver = this;

							Destroy(patient.gameObject);

							patient = zombBean;

							StartCoroutine(ressurecting());
							break;
						}
					}
				}

				animator.SetTrigger("jump");
				yield return new WaitForSeconds (0.5f);
			}
			else 
				break;
		}
	}

	IEnumerator ressurecting()
	{
		patient.animator.SetTrigger ("ressurect");
		patient.StartCoroutine ("reviving");

		while(true)
		{
			if(IsDead)
			{
				StopCoroutine(ressurecting());
				break;
			}
			else
				if(timeLeftToCast > 0)
				{
					timeLeftToCast--;
					yield return new WaitForSeconds(1f);
				}
				else
				{
					if(patient != null)
					{
						BeansSpawner.Instance.spawnFormation(singleFormation, zombiePrefab, patient.transform.position);

						Destroy(patient.gameObject);

						timeToNextRes = resDelay;
				
						StartCoroutine(moving());
						break;
					}
				}
		}
	}
	public override void getdata (BeanSave bs)
	{
		base.getdata (bs);
		NecromancerSave cs = bs as NecromancerSave;
		
		resAmount = cs.resAmount;
		resDelay = cs.resDelay;
		timeToNextRes = cs.timeToNextRes;
		castTime = cs.castTime;
		timeLeftToCast = cs.timeLeftToCast;
		resRadius = cs.resRadius;
	}

	public override BeanSave makeSaveData()
	{
		NecromancerSave bs = new NecromancerSave ();
		bs.getdata (this);
		return bs;
	}
}

[System.Serializable]
public class NecromancerSave: BeanSave
{
	public int resAmount = 0;
	public float resDelay = 0f, timeToNextRes = 0f, castTime = 0f, timeLeftToCast = 0f, resRadius = 0f;
	
	//public Bean patient;

	public override void getdata (Bean bean)
	{
		base.getdata (bean);
		Necromancer cent = bean as Necromancer;
		
		resAmount = cent.resAmount;
		resDelay = cent.resDelay;
		timeToNextRes = cent.timeToNextRes;
		castTime = cent.castTime;
		timeLeftToCast = cent.timeLeftToCast;
		resRadius = cent.resRadius;
		//patient = cent.patient;
	}
}
