using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class BeansSpawner : MonoBehaviour 
{
	public List<GameObject> beansPrefabs = new List<GameObject> ();
	public List<GameObject> formations = new List<GameObject> ();
	public float delayBetweenSpawn = 8.0f, leftToSpawn = 8.0f;

	public Transform spawnPointLeft, spawnPointCenter, spawnPointRight;

	public bool canSpawn = false;

	void Start()
	{
		leftToSpawn = delayBetweenSpawn;
		StartCoroutine (spawning ());
	}

	IEnumerator spawning()
	{
		while(true)
		{
			if(canSpawn)
			{
				leftToSpawn--;
				
				if(leftToSpawn <= 0f)
				{
					if(MainMenu.Instance.TutorialIndex == 0)
						MainMenu.Instance.showTutorial();
					spawnFormation(null);
					leftToSpawn = delayBetweenSpawn;
				}
			}
			yield return new WaitForSeconds(1f);
		}
	}

	GameObject formationToSpawn;
	List<GameObject> beantoSpawn = new List<GameObject>();
	Vector3 centerToSpawn;

	public int getIndexOfBoss(GameObject boss)
	{
		GameObject[] bosses = beansPrefabs.Where (b => b.transform.GetChild (0).GetComponent<Boss> () != null).ToArray ();

		for (int i = 0; i < bosses.Length; i++)
		{


			if (bosses[i].transform.GetChild(0).GetComponent<Boss>().GetType() == boss.GetComponent<Boss>().GetType())
			{print (bosses[i].transform.GetChild(0).GetComponent<Boss>() +"---" + boss.GetComponent<Boss>());
				return i;
			}
		}

		return -1;
	}

	public Boss getBossByDesiredScore(int score)
	{
		GameObject[] bosses = beansPrefabs.Where (b => b.transform.GetChild (0).GetComponent<Boss> () != null).OrderBy(b => b.transform.GetChild (0).GetComponent<Boss> ().desiredScore).ToArray();

		for (int i = 0; i < bosses.Length; i++)
		{
			if (GameManager.Instance.bossKiled[i] == 0)
			{
				int difference = bosses[i].transform.GetChild(0).GetComponent<Boss>().desiredScore;

				if(difference <= score)
					return bosses[i].transform.GetChild(0).GetComponent<Boss>();
			}
		}
		
		return null;
	}

	public void spawnFormation(GameObject form, List<GameObject> beans, Vector3 formCenter)
	{
		formationToSpawn = form;
		
		beantoSpawn.Clear ();
		
		for (int i = 0; i < beans.Count; i++)
			beantoSpawn.Add (beans [i]);
		
		centerToSpawn = formCenter;
		spawnFormation (null);
	}

	public int getBeanIndex(Bean b)
	{
		foreach (GameObject go in beansPrefabs)
			if (go.transform.GetChild (0).name == b.name)
				return beansPrefabs.IndexOf (go);

		return -1;
	}

	public void spawnFormation(GameObject form, GameObject bean, Vector3 formCenter)
	{
		formationToSpawn = form;

		beantoSpawn.Clear ();

		for (int i = 0; i < form.GetComponent<Formation> ().getFormationSlots (); i++)
			beantoSpawn.Add (bean);

		centerToSpawn = formCenter;
		spawnFormation (null);
	}

	public void spawnFormation(GameObject form, GameObject bean)
	{
		formationToSpawn = form;

		beantoSpawn.Clear ();
		
		for (int i = 0; i < form.GetComponent<Formation> ().getFormationSlots (); i++)
			beantoSpawn.Add (bean);

		spawnFormation (null);
	}

	public void spawnFormation(FormationSave savedFormation)
	{
		if(savedFormation != null)
		{
			formationToSpawn = formations.Where(f=>f.name == savedFormation.name).ToArray()[0];
			
			beantoSpawn.Clear ();

			for (int i = 0; i < savedFormation.beans.Count; i++)
			{
				beantoSpawn.Add (beansPrefabs.Where(b=>b.name == savedFormation.beans[i].name).ToArray()[0]);
			}
		}

		if (formationToSpawn == null) 
		{
			List<GameObject> filteredFormations = new List<GameObject>(formations).Where(f=>f.GetComponent<Formation>().freeToSpawn).ToList() as List<GameObject>;

			int randomFormation = Random.Range (0, filteredFormations.Count);

			formationToSpawn = filteredFormations [randomFormation];
		}

		Vector3 spawnPoint;

		if(centerToSpawn == Vector3.zero)
		{
			if (formationToSpawn.GetComponent<Formation> ().midOnly) 
				spawnPoint = spawnPointCenter.position;
			else
			{
				int randSpawn = Random.Range (0,3);

				if (randSpawn == 0)
					spawnPoint = spawnPointLeft.position;
				else if (randSpawn == 1)
					spawnPoint = spawnPointCenter.position;
				else
					spawnPoint = spawnPointRight.position;
			}
		}
		else
			spawnPoint = centerToSpawn;

		formationToSpawn = Instantiate(formationToSpawn, spawnPoint, Quaternion.identity) as GameObject;

		formationToSpawn.name = formationToSpawn.name.Replace("(Clone)", "");

		formationToSpawn.transform.SetParent (gameObject.transform);

		List<GameObject> slots = new List<GameObject>();

		for(int i = 0; i < formationToSpawn.transform.childCount; i++)
		{
			GameObject go = formationToSpawn.transform.GetChild(i).gameObject;

			if(go.tag == "formationSlot")
				slots.Add(go);
		}

		bool doubleSpeed = false;

		if(slots.Count < 4)//grants doubleSpeed
			doubleSpeed = (Random.Range(0,4) == 0);

		while(beantoSpawn.Count < slots.Count)
			beantoSpawn.Add(null);

		for(int i = 0; i < slots.Count; i++)
		{
			if(beantoSpawn[i] == null)
			{
				List<GameObject> beans = beansPrefabs.
				Where (b=>b.transform.GetChild(0).GetComponent<Bean>().desiredScore <= GameManager.Instance.score).
						Where (b=>b.transform.GetChild(0).GetComponent<Bean>().freeToSpawn).
						ToList() as List<GameObject>;

				beantoSpawn[i] = beans[Random.Range(0, beans.Count)];
			}
	
			GameObject spawnedBean = Instantiate(beantoSpawn[i], slots[i].transform.position, Quaternion.identity) as GameObject;

			spawnedBean.name = spawnedBean.name.Replace("(Clone)", "");
			Bean beanComp = spawnedBean.GetComponentInChildren<Bean>();
			beanComp.Owner = formationToSpawn.GetComponent<Formation>();

			if(doubleSpeed)
				beanComp.speed *= 2f;

			spawnedBean.transform.SetParent(formationToSpawn.transform);//формація буде власником (і батьком) квасолі

			if(savedFormation != null)
				if(i < savedFormation.beans.Count)
					beanComp.getdata(savedFormation.beans[i]);

			//handle sorting order

			for(int j = 0; j < spawnedBean.transform.childCount; j++)
			{
				SpriteRenderer[] spr = spawnedBean.transform.GetComponentsInChildren<SpriteRenderer>() as SpriteRenderer[];

				foreach(SpriteRenderer s in spr)
					s.sortingOrder += formationToSpawn.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder;
			}

			Destroy(formationToSpawn.transform.GetChild(i).gameObject);
		}

		formationToSpawn = null;
		beantoSpawn.Clear ();
		centerToSpawn = Vector3.zero;
	}

	private static BeansSpawner instance;  
	
	private BeansSpawner() {}   
	
	public static BeansSpawner Instance   
	{    
		get    
		{      
			if (instance ==  null)
				instance = GameObject.FindObjectOfType(typeof(BeansSpawner)) as  BeansSpawner;     
			return instance;   
		}  
	}
}

[System.Serializable]
public class BeansSpawnerSave
{
	public List<FormationSave> formBeans = new List<FormationSave>();
}
