using UnityEngine;
using System.Collections.Generic;

public class Formation : MonoBehaviour 
{
	public List<Bean> beans = new List<Bean>();
	public bool midOnly = false, freeToSpawn = true;

	public void checkBeans()
	{
		if (beans.Count < 1)
			Destroy (gameObject);
	}

	public int getFormationSlots()
	{
		int res = 0;

		for(int i = 0; i < transform.childCount; i++)
		{
			GameObject go = transform.GetChild(i).gameObject;

			if(go.tag == "formationSlot")
				res++;
		}	

		return res++;
	}
}

[System.Serializable]
public class FormationSave
{
	public string name = "";
	public List<BeanSave> beans = new List<BeanSave>();
	public bool midOnly = false;
}
