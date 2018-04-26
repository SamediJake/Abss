using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public static class SaveLoad 
{
	public static void savePersistent()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create ("Saves/persistentData.gd"); 

		GameManager GM = GameManager.Instance;

		bool musicOn = (GM.musicChannel.volume != 0f);
		bool soundOn = (GM.soundChannel.volume != 0f);

		Persistent pers = new Persistent 
			(GM.GetComponent<Statistics>().KillingStart, 
			 GM.GetComponent<Statistics>().SkillUsed, 
			 GM.TransPostfix, 
			 musicOn, 
			 soundOn, 
			 MainMenu.Instance.getSpentTime(), MainMenu.Instance.TutorialIndex);

		bf.Serialize (file, pers);

		file.Close();
	}

	public static void loadPersistent()
	{
		string path = "Saves/persistentData.gd";

		GameManager GM = GameManager.Instance;

		if(File.Exists(path)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(path, FileMode.Open);

			Persistent pers = (Persistent)bf.Deserialize(file);

			GM.transPostfix = pers.transPostfix;

			GM.musicChannel.volume = (pers.musicOn) ? 1f : 0f;

			GM.soundChannel.volume = (pers.soundOn) ? 1f : 0f;

			GM.GetComponent<Statistics>().setKillingStart(pers.killedBean);

			GM.GetComponent<Statistics>().setUsedSkillls(pers.skillUsed);

			GM.GetComponent<Statistics>().timeSpentInGame = pers.timeSpentInGame;

			MainMenu.Instance.TutorialIndex = pers.tutorialIndex;

			file.Close();
		}
	}

	public static void Save() 
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create ("Saves/savedData.gd"); 
		GameManager GM = GameManager.Instance;
		BeansSpawnerSave bss = new BeansSpawnerSave ();

		Formation[] forms = GameObject.FindObjectsOfType<Formation> ().ToArray ();

		foreach(Formation f in forms)
		{
			FormationSave fs = new FormationSave();
			fs.midOnly = f.midOnly;
			fs.name = f.name;

			foreach(Bean b in f.beans)
			{
				if(b != null)
				{
					Debug.Log(b.name);

					fs.beans.Add(b.makeSaveData());
				}
			}
			bss.formBeans.Add(fs);
		}

		bf.Serialize (file, bss);

		GameManagerSave gms = new GameManagerSave (
			GM.LifeRemains, 
			GM.MaxLifes, 
			GM.Souls, 
			GM.score, 
			GM.BoughtSkillsAmount,
			GM.KilledByExplosionAmount,
			GM.GetDamageAmount,
			GM.products, 
			GM.bossKiled);		                                         

		bf.Serialize (file, gms);

		file.Close();
	}   
	
	public static void Load() 
	{
		string path = "Saves/savedData.gd";
		GameManager GM = GameManager.Instance;

		if(File.Exists(path)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(path, FileMode.Open);
			
			BeansSpawnerSave bss = (BeansSpawnerSave)bf.Deserialize(file);

			foreach(FormationSave fs in bss.formBeans)
			{
				Debug.Log(fs.name);
				if(fs.beans.Count > 0)
					BeansSpawner.Instance.spawnFormation(fs);
			}

			GameManagerSave gms = (GameManagerSave)bf.Deserialize(file);

			GM.LifeRemains = gms.lifeRemains;
			GM.MaxLifes = gms.maxLifes;
			GM.score = gms.score;
			GM.Souls = gms.souls;

			for(int i = 0; i < GM.products.Count; i++)
			{
				GM.products[i].amount = gms.prodSaves[i].amount;

				if(gms.prodSaves[i].itemWasSpawned)
					if(gms.prodSaves[i].curDuration > 0f)
						GM.addProduct(GM.products[i], 1);

				GM.products[i].refresh();
			}

			GM.bossKiled.Clear();

			foreach(int i in gms.killedBoss)
				GM.bossKiled.Add(i);

			file.Close();
		}
	}
}

[System.Serializable]
public class Persistent
{
	public string transPostfix = "eng";
	public bool musicOn = true, soundOn = true;
	public float timeSpentInGame = 0.0f;
	public List<int> killedBean = new List<int> ();
	public List<int> skillUsed = new List<int> ();
	public int tutorialIndex = 0;

	public Persistent(List<int> killedBean, List<int> skillUsed, string transPostfix, bool musicOn, bool soundOn, float timeSpentInGame, int tutorialIndex)
	{
		this.transPostfix = transPostfix;
		this.musicOn = musicOn;
		this.soundOn = soundOn;
		this.tutorialIndex = tutorialIndex;

		foreach (int i in killedBean)
			this.killedBean.Add (i);

		foreach (int i in skillUsed)
			this.skillUsed.Add (i);

		this.timeSpentInGame = timeSpentInGame;
	}
}