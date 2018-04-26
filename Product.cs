using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class Product : MonoBehaviour 
{
	public bool isDragable;
	public int amount = 0, price = 10;
	public string product_name = "", description = "";
	public Text amountTx;
	public Skill skillItemPrefab;
	public bool skillItemSpawned = false;

	public GameObject souls_panel;

	public Sprite active, not_active;

	public float curDuration = 0f, maxDuration = 10f;

	private Image productImage;

	public int index;

	public float CurDuration
	{
		get{ return curDuration;}
		set{ curDuration = value; productImage.sprite = (curDuration > 0f) ? active : not_active;}
	}

	public int Amount
	{
		get{return amount;}
		set
		{
			if(value < 1)
			{
				GetComponent<Button>().onClick.RemoveAllListeners();
				GetComponent<Button>().onClick.AddListener(delegate {GameManager.Instance.Buy(index);});
			}
			else
			{
				GetComponent<Button>().onClick.RemoveAllListeners();
			}

			amount = value;
			refresh();
		}
	}
	void Awake()
	{
		productImage = GetComponent<Button> ().targetGraphic.GetComponent<Image> ();

		refresh();
	}

	public void refresh()
	{
		if(Amount > 0)
		{
			if(!isDragable)
			{
				GetComponent<Button>().onClick.RemoveAllListeners();
				GetComponent<Button>().onClick.AddListener(delegate {GameManager.Instance.removeProduct(this); spawnSkillItem();});
			}
			amountTx.text = amount.ToString();
		}
		else
		{
			GetComponent<Button>().onClick.RemoveAllListeners();
			GetComponent<Button>().onClick.AddListener(delegate {GameManager.Instance.Buy(index);});

			amountTx.text = "";
		}

		souls_panel.SetActive (Amount < 1);

		if(souls_panel.activeSelf)
		{
			Text tx = souls_panel.GetComponentInChildren<Text>();
			tx.text = price.ToString();
		}
	}
	
	public GameObject spawnSkillItem()
	{
		if(Time.timeScale != 0f)
		{
			GameObject go = Instantiate (skillItemPrefab.gameObject) as GameObject;

			go.GetComponent<Skill> ().product = this;

			return go;
		}
		return null;
	}

	public void executeSkillPrefab()
	{
		if(Time.timeScale != 0f)
		{
			Skill[] sks = FindObjectsOfType<Skill> ().Where (b => b.GetType () == skillItemPrefab.GetType ()).ToArray() as Skill[];

			if(sks.Length > 0)
				sks[0].execute ();
		}
	}

	public void spawnCopyOfImage()
	{
		if(Amount > 0 && Time.timeScale != 0f && CurDuration < 1f)
		{
			GameObject g = spawnSkillItem ();

			if (g != null)
			{
				GameObject go = new GameObject ();
				go.AddComponent<SpriteRenderer> ().sprite = GetComponent<Button> ().targetGraphic.gameObject.GetComponent<Image> ().sprite;
				g.tag = "CopyOfItem";
				go.transform.SetParent (g.transform);
				go.transform.position = Vector3.zero;
				GameManager.Instance.removeProduct(this);
			}
		}
	}

	IEnumerator handleUsebar()
	{
		if(maxDuration > 0)
		{
			while(true)
			{
				CurDuration--;

				if(CurDuration > 0)
					yield return new WaitForSeconds(1f);
				else
				{
					break;
				}
			}
		}
	}
}

[System.Serializable]
public class ProductSave
{
	public int amount = 0, price = 0;
	public float curDuration = 0f, maxDuration = 0f;
	public string name = "";
	public bool itemWasSpawned = false;

	//for windmaker
	public float posx = 0f, posy = 0f, posz = 0f, rotx = 0f, roty = 0f, rotz = 0f, rotw = 0f;
	public bool windmakerWasPlaced = false;	

	public ProductSave(Product pr)
	{
		this.name = pr.name;
		this.amount = pr.Amount;
		this.price = pr.price;
		this.curDuration = pr.curDuration;
		this.maxDuration = pr.maxDuration;
		this.itemWasSpawned = pr.skillItemSpawned;

		if(pr.skillItemPrefab != null)
		{
			if(pr.skillItemPrefab.GetComponent<Lupa>() != null)
			{
				Lupa lp = GameObject.FindObjectOfType<Lupa>();
				if(lp != null)
				{
					this.posx = lp.transform.position.x;
					this.posy = lp.transform.position.y;
					this.posz = lp.transform.position.z;
				}
			}
		}
	}
}