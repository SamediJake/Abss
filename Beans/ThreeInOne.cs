using UnityEngine;
using System.Collections;

public class ThreeInOne : Bean 
{
	public bool hasChildren = true;
	public GameObject formation, beanPrefab;
	
	public override void die()
	{
		if(hasChildren)
		{
			BeansSpawner.Instance.spawnFormation(formation, beanPrefab, transform.position);
			hasChildren = false;
		}

		base.die ();
	}
	public override void getdata (BeanSave bs)
	{
		ThreeInOneSave cs = bs as ThreeInOneSave;
		hasChildren = cs.hasChildren;
		base.getdata (bs);
	}

	public override BeanSave makeSaveData()
	{
		ThreeInOneSave bs = new ThreeInOneSave ();
		bs.getdata (this);
		return bs;
	}
}

[System.Serializable]
public class ThreeInOneSave: BeanSave
{
	public bool hasChildren = true;

	public override void getdata (Bean bean)
	{
		base.getdata (bean);
		ThreeInOne cent = bean as ThreeInOne;
		hasChildren = cent.hasChildren;
	}
}
