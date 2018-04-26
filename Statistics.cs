using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Statistics : MonoBehaviour 
{
	List<int> killingStart = new List<int>();
	List<int> skillUsed = new List<int> ();

	public float timeSpentInGame = 0.0f;

	public List<int> KillingStart
	{get{ return killingStart; }}

	public List<int> SkillUsed
	{get{ return skillUsed; }}

	void Awake()
	{
		foreach (GameObject go in BeansSpawner.Instance.beansPrefabs)
			killingStart.Add (0);

		foreach (Product go in GameManager.Instance.products)
			skillUsed.Add (0);
	}
	
	public void addKilledBean(int index, Bean bean)
	{
		if(index > -1 && index < killingStart.Count)
			killingStart [index]++;

		if (bean.gameObject.tag == "zombie") 
		{
			if(killingStart [index] > 99)
				GameManager.Instance.setAchievementStatus(12, true);
		} 
		else if (bean.gameObject.tag == "leggionaire") 
		{
			if(killingStart [index] > 49)
				GameManager.Instance.setAchievementStatus(13, true);

		} 
		else if (bean.gameObject.tag == "ninja") 
		{
			if(killingStart [index] > 49)
				GameManager.Instance.setAchievementStatus(14, true);
		}
	}

	public int getKilledIndexByName(string name)
	{
		for (int i = 0; i < BeansSpawner.Instance.beansPrefabs.Count; i++)
			if (BeansSpawner.Instance.beansPrefabs [i].name == name)
				return i;

		return -1;
	}

	public void addSkillUsed(int index)
	{
		skillUsed [index]++;

		if (skillUsed [2] > 9)
			GameManager.Instance.setAchievementStatus (7, true);
	}

	public void setKillingStart(List<int> counts)
	{
		for(int i = 0; i < counts.Count; i++)
			killingStart[i] = counts[i];
	}

	public void setUsedSkillls(List<int> counts)
	{
		for(int i = 0; i < counts.Count; i++)
			skillUsed[i] = counts[i];
	}
}
