using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

public class GameManager : MonoBehaviour 
{
	//[HideInInspector]
	public int lifeRemains = 10, maxLifes = 10, souls = 0, score = 0;

	int boughtSkillsAmount = 0;//buy 50 items achievement (id=25)

 	int killedByExplosionAmount = 0;//25 dead beans because of kamikadze explosion

	int getDamageAmount = 0; // amount of damage received, achievement for getting 50 damage

	public int GetDamageAmount
	{
		get{ return getDamageAmount;}
		set
		{
			getDamageAmount = value;
			
			if(getDamageAmount > 50)
				setAchievementStatus(5, true);
		}
	}

	public int BoughtSkillsAmount
	{
		get{ return boughtSkillsAmount;}
		set
		{
			boughtSkillsAmount = value;

			if(boughtSkillsAmount > 49)
				setAchievementStatus(25, true);
		}
	}

	public int KilledByExplosionAmount
	{
		get{ return killedByExplosionAmount;}
		set
		{
			killedByFlyKiller = value;

			if(killedByFlyKiller > 24)
				setAchievementStatus(10, true);
		}
	}

	public GameObject singleFormation;

	public AudioClip deathMelody;

	public AudioSource musicChannel, soundChannel;

	public Text soulsTx;

	public Image hpFill;

	public string transPostfix = "eng";

	public bool isChangingTable = false;

	int killedByFlyKiller = 0;

	public int KilledByFlyKiller
	{
		get{return killedByFlyKiller;}
		set
		{
			if(killedByFlyKiller > 99)
				setAchievementStatus(8, true);
		}
	}

	public Achievement[] achievements;

	public delegate void OnTranslationChangedEvent();
	public event OnTranslationChangedEvent changeTranslation;
	
	public string TransPostfix
	{
		set{;transPostfix = value;changeTranslation(); SaveLoad.savePersistent();}
		get{return transPostfix;}
	}

	public List<int> bossKiled = new List<int>();

	public List<Product> products = new List<Product> ();

	public int Souls
	{
		get{return souls;}
		set
		{
			souls = value;

			if(souls > 4999)
				setAchievementStatus(11, true);

			print ("Score: " + score);

			if(souls >= getCheapestSkillValue())
				if(MainMenu.Instance.TutorialIndex == 2)
					MainMenu.Instance.showTutorial();

			StringBuilder strBld = new StringBuilder(); 
			strBld.Append(souls); 
			soulsTx.text = strBld.ToString();

			if(bossKiled.Count>0 && Camera.main.transform.position.x == 0f)
			{
				Boss boss = BeansSpawner.Instance.getBossByDesiredScore(score);

				if(boss != null)
				{
					Bean[] beans = FindObjectsOfType<Bean>().Where(b=>!b.IsDead).ToArray();
					BeansSpawner.Instance.canSpawn = false;
					if(beans.Length == 0 && !isChangingTable)
					{
						Transform targetTable = GameObject.Find(boss.gameObject.name + "_table").transform;

						StartCoroutine(
						changeTable(targetTable, boss.gameObject, singleFormation));
					}
				}
			}
		}
	}

	public int LifeRemains
	{
		get{return lifeRemains;}
		set
		{
			if(value < lifeRemains)
			{
				halfHourAch = 0.0f;
				getDamageAmount += lifeRemains - value;
			}

			lifeRemains = value;

			if(lifeRemains < 1)
			{
				gameOver();
				lifeRemains = 0;
			}

			if(lifeRemains > maxLifes)
				lifeRemains = maxLifes;

			if(lifeRemains < maxLifes/3)
				StartCoroutine(lifeAppearing());

			refreshLifesText();
		}
	}

	IEnumerator lifeAppearing()
	{
		while(true)
		{
			if(lifeRemains < maxLifes/3)
			{
				hpFill.color = new Color(1f,1f,1f, hpFill.color.a == 1f ? 0f : 1f);
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				hpFill.color = new Color(1f,1f,1f,1f);
				break;
			}
		}
	}

	public IEnumerator changeTable(Transform tr, GameObject bean, GameObject form)
	{
		Bean[] beans = FindObjectsOfType<Bean> ().ToArray();

		Transform beansAfterBossDeath = GameObject.FindGameObjectWithTag("beansAfterBossDeath").transform;

		GameObject[] shields = GameObject.FindGameObjectsWithTag ("shield").ToArray ();
		
		foreach (Bean b in beans)
			b.transform.parent.SetParent (beansAfterBossDeath);

		foreach (GameObject go in shields)
			go.transform.SetParent (beansAfterBossDeath);

		float timeforStep = 0.05f;
		Vector3 step = (tr.position - Camera.main.transform.position)*timeforStep;

		step = new Vector3 (step.x, step.y, 0f);

		bool cont = true;

		while(cont)
		{
			isChangingTable = true;

			if(Mathf.Abs(Camera.main.transform.position.x - tr.position.x) > Mathf.Abs(step.x))
			{
				Camera.main.transform.position += step;
			}
			else
			{
				Camera.main.transform.position = new Vector3(tr.position.x, tr.position.y, -10f);

				if(form != null && bean != null)
				{
					BeansSpawner.Instance.spawnFormation(form, bean.transform.parent.gameObject, BeansSpawner.Instance.spawnPointCenter.position);
					form = null; bean = null;
				}
				else
					BeansSpawner.Instance.canSpawn = true;

				cont = false;

				isChangingTable = false;

				break;
			}

			yield return new WaitForSeconds(timeforStep);
		}
	}

	void gameOver()
	{
		stopTime (true);
		musicChannel.loop = false;
		musicChannel.clip = deathMelody;
		musicChannel.Play();
		MainMenu.Instance.showGameOverPanel (true);
	}

	public int getCheapestSkillValue()
	{
		int minValue = products [0].price;

		foreach (Product pr in products)
			if (pr.price < minValue)
				minValue = pr.price;

		return minValue;
	}

	public int MaxLifes
	{
		get{return maxLifes;}
		set{maxLifes = value;refreshLifesText();}
	}

	void refreshLifesText()
	{
		hpFill.fillAmount = (float)LifeRemains / (float)MaxLifes;
	}

	void Update()
	{
		handleSortingOrder ();

		if (!getAchievementStatus (21)) 
		{
			float t = GetComponent<Statistics> ().timeSpentInGame += MainMenu.Instance.TimeInSession;

			if(t > 3600f)
				setAchievementStatus(21, true);
		}
	}

	void OnApplicationQuit() 
	{
		setAchievementStatus (2, true);
		SaveLoad.Save ();
		SaveLoad.savePersistent ();
	}

	void handleSortingOrder()
	{
		Bean[] beans = FindObjectsOfType<Bean> ().Where(b=>!b.IsDead).OrderBy (o=>o.transform.position.y).ToArray() as Bean[];

		int allSpriteRenderersCount = 0;

		for(int i = 0; i < beans.Length; i++)
		{
			SpriteRenderer[] spr = beans[i].GetComponentsInChildren<SpriteRenderer>() as SpriteRenderer[];

			allSpriteRenderersCount += spr.Length;
		}

		int childCount = beans.Length;

		for(int i = 0; i < childCount; i++)
		{
			SpriteRenderer[] spr = beans[i].GetComponentsInChildren<SpriteRenderer>() as SpriteRenderer[];

			SpriteRenderer temp = spr[0];
			spr[0] = spr[spr.Length-1];
			spr[spr.Length-1] = temp;

			spr = spr.OrderByDescending(s=>s.sortingOrder).ToArray();

			foreach(SpriteRenderer s in spr)
			{
				s.sortingOrder = allSpriteRenderersCount;
				allSpriteRenderersCount--;
			}
		}
	}

	public void stopTime(bool stop)
	{
		Time.timeScale = stop ? 0f : 1f;
	}

	public void Buy(int selectedProduct)
	{
		if (Souls >= products[selectedProduct].price)
		{
			Souls -= GameManager.Instance.products[selectedProduct].price;
			products[selectedProduct].Amount++;
			boughtSkillsAmount++;

			print ("bought");
		}
		else
		{
			print("no money");
		}
	}

	public void addProduct(Product pr, int amount)
	{
		products [products.IndexOf (pr)].Amount += amount;
	}

	public void removeProduct(Product pr)
	{
		if(Time.timeScale != 0f)
			products [products.IndexOf (pr)].Amount--;
	}

	void Awake()
	{
		SaveLoad.loadPersistent ();
		GetComponent<TranslationLoader>().StartCoroutine("getLangsList");
		GetComponent<TranslationLoader>().StartCoroutine("GetTranslations");
	}

	void Start()
	{
		//to show the right texts
		LifeRemains = LifeRemains;
		Souls = Souls;

		if(!getAchievementStatus(3))
			StartCoroutine (checkHalfHourAch ());
	}

	float halfHourAch = 0.0f;

	IEnumerator checkHalfHourAch()
	{
		while (true) 
		{
			if(!getAchievementStatus(3))
			{
				if(Time.timeScale != 0f)
					halfHourAch += Time.deltaTime;

				if(halfHourAch >= 30f)
				{
					setAchievementStatus(4, true);
					break;
				}
				else
					yield return new WaitForSeconds(Time.deltaTime);

			}
			else break;
		}
	}

	public bool getAchievementStatus(int id)
	{
		for(int i = 0; i < achievements.Length; i++)
			if(achievements[i].achId == id)
				return achievements[i].IsUnlocked;

		return false;
	}

	public void setAchievementStatus(int id, bool status)
	{
		for(int i = 0; i < achievements.Length; i++)
		{
			if(achievements[i].achId == id)
			{
				achievements[i].IsUnlocked = status;
				break;
			}
		}

		bool hasAllAchievements = true;

		for (int i = 0; i < achievements.Length; i++)
			if(achievements [i].achId != 24)
				hasAllAchievements = achievements [i].IsUnlocked;

		if (hasAllAchievements)
			setAchievementStatus (24, true);

	}

    const string AppId = "546296085547392";
    const string ShareUrl = "http://www.facebook.com/dialog/feed";

    public void ShareOnFacebook()
    {

        Application.OpenURL(ShareUrl +
        "?app_id=" + AppId +
          "&link=https://www.dropbox.com/s/93xx5ctocpckyag/ABs_12_03.apk?dl=0" + WWW.EscapeURL(link) +
         "&picture=http://cs633131.vk.me/v633131031/2710e/rvO-Yf3rxDs.jpg" + WWW.EscapeURL(pictureLink) +
           "&name=Check out Angry Beans Smasher on Android, bro. Coming soon..." + WWW.EscapeURL(name) +
           "&caption=Beans are coming!" + WWW.EscapeURL(caption) +
          "&description=Only U can stop them" + WWW.EscapeURL(description) +
           "&redirect_uri=https://www.facebook.com/" + WWW.EscapeURL(redirectUri));
    }

    const string Address = "http://twitter.com/intent/tweet";

    public void ShareOnTwitter()
    {
        Application.OpenURL(Address +
            "?text=Check out Angry Beans Smasher on Android, bro" + WWW.EscapeURL(text) +
            "&url=https://www.dropbox.com/s/93xx5ctocpckyag/ABs_12_03.apk?dl=0" + WWW.EscapeURL(url) +
            "&related=@werald2" + WWW.EscapeURL(related) +
            "&lang=en" + WWW.EscapeURL(lang));
    }
	
	private  static GameManager instance;  
	
	private GameManager() {}   
	
	public static GameManager Instance   
	{    
		get    
		{      
			if (instance ==  null)
				instance = GameObject.FindObjectOfType(typeof(GameManager)) as  GameManager;     
			return instance;   
		}  
	}

    public string link { get; set; }

    public string pictureLink { get; set; }

    public string caption { get; set; }

    public string description { get; set; }

    public string redirectUri { get; set; }

    public string text { get; set; }

    public string url { get; set; }

    public string related { get; set; }

    public string lang { get; set; }
}

[System.Serializable]
public class GameManagerSave
{
	public int lifeRemains = 0, maxLifes = 0, souls = 0, score = 0, boughtSkillsAmount = 0, killedByExplosionAmount = 0, getDamageAmount = 0;
	public List<ProductSave> prodSaves = new List<ProductSave> ();
	public List<int> killedBoss = new List<int> ();

	public GameManagerSave(int lifeRemains, int maxLifes, int souls, int score, int boughtSkillsAmount, int killedByExplosionAmount, int getDamageAmount, List<Product> products, List<int> killedBoss)
	{
		this.lifeRemains = lifeRemains;
		this.maxLifes = maxLifes;
		this.souls = souls;
		this.score = score;
		this.boughtSkillsAmount = boughtSkillsAmount;
		this.killedByExplosionAmount = killedByExplosionAmount;
		this.getDamageAmount = getDamageAmount;

		foreach (Product i in products)
			this.prodSaves.Add (new ProductSave(i));

		foreach (int i in killedBoss)
			this.killedBoss.Add (i);
	}
}