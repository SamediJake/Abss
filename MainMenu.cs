using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;

public class MainMenu : MonoBehaviour 
{
	public List<GameObject> buttons = new List<GameObject>();
	public List<BoxCollider> ExtraMenuButtons = new List<BoxCollider>();
	public List<GameObject> tutorialPanels = new List<GameObject>();

	public GameObject can, canCamera, inGameUI, shareButtons; 
	public Animator canCameraAnimator;
	public GameObject preventScreen, gameOverPanel, achievementsPanel, teamListPanel, recordsPanel, deadBeansPanel, UsedSkills, achievementsParent, extraMenuPunkts;
	public ExtraMenuScroller extramenuscroller;
	public Text gameWillStart, beansWasKilledGeneralText, skillsWasUsedGeneralText, timeSpentInGame;
	public float continueIn = 3, startMoment = 0;
	string gameWillStartSaved = "";
	public bool gameIsRunning = false, canScrollExtraMenu = false;

	bool needToLoad = false;

	[SerializeField]
	float rotationTime = 0.1f, timeInSession = 0.0f, leftRotate = 0f, degree = 0;

	public float TimeInSession
	{ get { return timeInSession; } }

	int tutorialIndex = 0;

	public int TutorialIndex
	{ get { return tutorialIndex; } set { tutorialIndex = value; } }

	GameManager GM;

	public void showTutorial()
	{
		tutorialPanels [tutorialIndex].SetActive (true);
		GameManager.Instance.stopTime (true);
		tutorialIndex++;
	}

	public void hideTutorials()
	{
		foreach (GameObject go in tutorialPanels)
			go.SetActive (false);

		GameManager.Instance.stopTime (false);

		if (tutorialIndex == 1)
			showTutorial ();
	}

	void Update()
	{
		timeInSession += Time.unscaledDeltaTime;

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			if(Time.timeScale != 0f)
			{
				GameManager.Instance.stopTime(true);
				showMainMenu(true);
			}
		}
	}

	public void rotateCan(float degree)
	{
		if(leftRotate == 0f)
		{
			enableButtonColliders (false);
			leftRotate = rotationTime;
			this.degree = degree - transform.rotation.y;
			StartCoroutine (rotatingCan ());
		}
	}

	public void hideAllExtraMenus()
	{
		if (achievementsPanel.activeSelf || recordsPanel.activeSelf || teamListPanel.activeSelf) 
		{
			if (achievementsPanel.activeSelf)
				show_achievementsPanel ();
			if (recordsPanel.activeSelf)
				show_statisticsPanel ();
			if (teamListPanel.activeSelf)
				show_teamListPanel ();
		}
		else
			rotateCan (180f);

		Vector2 offset = extramenuscroller.GetComponent<MeshRenderer> ().material.mainTextureOffset;
		
		extramenuscroller.GetComponent<MeshRenderer> ().material.mainTextureOffset = new Vector2 (offset.x, 0f);
	}

	public void showGameOverPanel(bool show)
	{
		gameOverPanel.SetActive(show);
	}

	void Awake()
	{
		Screen.SetResolution (750, 1334, false);
		GM = GameManager.Instance;
		preventScreen.SetActive (false);
	}

	void setDefaults()
	{
		GameManager GM = GameManager.Instance;

		GM.LifeRemains = GM.MaxLifes; GM.score = 0; GM.Souls = 0;

		foreach(Product pr in GM.products)
		{
			pr.Amount = 1;
			pr.CurDuration = 0f;
			pr.skillItemSpawned = false;
		}

		BeansSpawner bs = BeansSpawner.Instance;

		GM.bossKiled.Clear ();

		for(int i = 0; i < bs.beansPrefabs.Count; i++)
			if(bs.beansPrefabs[i].transform.GetChild(0).GetComponent<Boss>() != null)
		{
			print (bs.beansPrefabs[i].transform.GetChild(0).GetComponent<Boss>());
				GM.bossKiled.Add(0);
		}

		Skill[] go = GameObject.FindObjectsOfType<Skill>() as Skill[];

		foreach (Skill g in go)
			Destroy (g.gameObject);

		int childCount = BeansSpawner.Instance.transform.childCount;

		for(int i = 0; i < childCount; i++)
			Destroy(BeansSpawner.Instance.transform.GetChild(i).gameObject);

		Camera.main.transform.position = new Vector3 (0f, 0f, -10f);
	}

	public void newGame()
	{
		StartGame (false);
		GameManager.Instance.setAchievementStatus (1, true);
	}
	public void continueGame()
	{
		StartGame (true);
	}

	public void StartGame(bool needToLoad)
	{
		this.needToLoad = needToLoad;
		can.GetComponent<Animation> ().Play ("Take 001");
		canCameraAnimator.GetComponent<Animator> ().SetTrigger ("Start");
	}

	public void Continue()
	{
		showMainMenu (false);
		GameManager.Instance.musicChannel.loop = true;
		BeansSpawner.Instance.canSpawn = true;
		Time.timeScale = 1f;

		if(needToLoad)
		{
			if(!gameIsRunning)
				if(System.IO.File.Exists("Saves/savedData.gd")) 
					SaveLoad.Load();

			gameWillStartSaved = TranslationLoader.Instance.obj["gameWillStartText"];
			Time.timeScale = 0f;
			startMoment = continueIn;
			StartCoroutine (gameWillStartFunc());
			preventScreen.SetActive (true);
			needToLoad = false;
		}
		else
			setDefaults ();

		gameIsRunning = true;
	}

	IEnumerator gameWillStartFunc()
	{
		while(true)
		{
			if(startMoment >= 0f)
			{
				gameWillStart.text = gameWillStartSaved + startMoment;
				startMoment --;
				yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));
			}
			else
			{
				Time.timeScale = 1f;
				gameWillStart.text = gameWillStartSaved;
				preventScreen.SetActive(false);
				break;
			}
		}
	}

	public void continueAfterDefeat(bool killBeans)
	{
		GameManager.Instance.LifeRemains = GameManager.Instance.MaxLifes;

		if(killBeans)
		{
			Bean[] beans = GameObject.FindObjectsOfType<Bean>() as Bean[];

			foreach(Bean b in beans)
				b.CurHp = 0;
		}

		GameManager.Instance.stopTime (false);
		showGameOverPanel (false);

		GameManager.Instance.setAchievementStatus (6, true);
	}

	public void showMainMenu (bool on)
	{
		if (on)
			Time.timeScale = 0f;

		can.SetActive (on);
		canCamera.SetActive (on);
		inGameUI.SetActive (!on);
		shareButtons.SetActive (on);
		TranslationLoader.Instance.langList.gameObject.SetActive (on);
		enableButtonColliders (on);
	}

	void enableButtonColliders(bool enable)
	{
		foreach (GameObject go in buttons)
			if (go.GetComponent<BoxCollider> () != null)
				go.GetComponent<BoxCollider> ().enabled = enable;
			else if (go.GetComponent<BoxCollider2D> () != null)
				go.GetComponent<BoxCollider2D> ().enabled = enable;
	}

	IEnumerator rotatingCan()
	{
		while(true)
		{
			if(leftRotate >= 0f)
			{
				can.transform.Rotate (Vector3.up, degree/(rotationTime/0.01f));
				yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.01f));
			}
			else
			{
				degree = 0;
				enableButtonColliders(true);
				leftRotate = 0f;
				break;
			}
			leftRotate -= 0.01f;
		}
	}

	void showExtraMenu(bool on)
	{
		extraMenuPunkts.SetActive (on);

		foreach (BoxCollider col in ExtraMenuButtons)
			col.enabled = on;
	}

	public void show_achievementsPanel()
	{
		achievementsPanel.SetActive (!achievementsPanel.activeSelf);

		canScrollExtraMenu = achievementsPanel.activeSelf;

		showExtraMenu(!achievementsPanel.activeSelf);

		if(achievementsPanel.activeSelf)
		{
			int childCount = achievementsParent.transform.childCount;

			for(int i = 0; i < childCount; i++)
			{
				GameObject ach = achievementsParent.transform.GetChild(i).gameObject;

				ach.transform.Find("ach_name").GetComponent<Text>().text = TranslationLoader.Instance.obj[GameManager.Instance.achievements[i].nameKey];
				ach.transform.Find("ach_name").GetComponent<Text>().text += (GameManager.Instance.achievements[i].IsUnlocked) ? TranslationLoader.Instance.obj["Unlocked"] : "";
				ach.transform.Find("ach_description").GetComponent<Text>().text = TranslationLoader.Instance.obj[GameManager.Instance.achievements[i].descKey];
			}
		}
	}

	public void show_teamListPanel()
	{
		teamListPanel.SetActive (!teamListPanel.activeSelf);
		showExtraMenu(!teamListPanel.activeSelf);
		GameManager.Instance.setAchievementStatus (3, true);
	}

	public void show_statisticsPanel()
	{
		recordsPanel.SetActive (!recordsPanel.activeSelf);

		canScrollExtraMenu = recordsPanel.activeSelf;

		showExtraMenu(!recordsPanel.activeSelf);

		int childCount = deadBeansPanel.transform.childCount;

		if (recordsPanel.activeSelf) 
		{
			int res = 0;

			for (int i = 0; i <childCount; i++) 
			{
				int pos = BeansSpawner.Instance.beansPrefabs.IndexOf (BeansSpawner.Instance.beansPrefabs.Where (b => b.name == deadBeansPanel.transform.GetChild (i).name).ToArray () [0]);

				int count = GM.GetComponent<Statistics> ().KillingStart [pos];

				res += count;

				deadBeansPanel.transform.GetChild (i).transform.Find ("killedBeanValue").GetComponent<Text> ().text = count.ToString ();
			}

			beansWasKilledGeneralText.text = TranslationLoader.Instance.obj["beansWasKilledText"]+" "+res.ToString();

			childCount = UsedSkills.transform.childCount;

			res = 0;

			for(int i = 0; i < childCount; i++)
			{
				int pos = GameManager.Instance.products.IndexOf (GameManager.Instance.products.Where (b => b.name == UsedSkills.transform.GetChild (i).name).ToArray () [0]);
				
				int count = GM.GetComponent<Statistics> ().SkillUsed [pos];
				
				res += count;
				
				UsedSkills.transform.GetChild (i).transform.Find ("usedSkillValue").GetComponent<Text> ().text = count.ToString ();
			}

			skillsWasUsedGeneralText.text = TranslationLoader.Instance.obj["skillsWasUsedGeneralText"]+" "+res.ToString();

			GM.GetComponent<Statistics>().timeSpentInGame += timeInSession;
			timeInSession = 0.0f;

			TimeSpan t = TimeSpan.FromSeconds(GM.GetComponent<Statistics>().timeSpentInGame);

			string resultTime = String.Format ("{0:00}:{1:00}:{2:00}",t.Hours, t.Minutes, t.Seconds); 

			timeSpentInGame.text = TranslationLoader.Instance.obj["timeSpentInGameText"]+" "+resultTime;
		}
	}

	public float getSpentTime()
	{
		return GM.GetComponent<Statistics> ().timeSpentInGame + timeInSession;
	}

	public void setMusicOnOff()
	{
		GM.musicChannel.volume = (GM.musicChannel.volume != 0f) ? 0f : 1f;
	}
	
	public void setSoundOnOff()
	{
		GM.soundChannel.volume = (GM.soundChannel.volume != 0f) ? 0f : 1f;
	}

	private  static MainMenu instance;  
	
	private MainMenu() {}   
	
	public static MainMenu Instance   
	{    
		get    
		{      
			if (instance ==  null)
				instance = GameObject.FindObjectOfType(typeof(MainMenu)) as  MainMenu;     
			return instance;   
		}  
	}
}
