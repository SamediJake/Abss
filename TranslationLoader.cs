using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class TranslationLoader : MonoBehaviour 
{	
	public Dictionary<string,string> obj = new Dictionary<string, string> ();
	
	public bool isLoading = false;
	
	public RectTransform langList;
	
	public GameObject langButtonPrefab;
	
	TextAsset trans;
	
	public IEnumerator getLangsList()
	{
		isLoading = true;
		
		WWW www = new WWW("file://"+Application.dataPath.Replace("/Assets", "")+"/AssetBundles/langs.unity3d");
		yield return www;
		
		string[] assetnames = www.assetBundle.GetAllAssetNames ();
		
		for(int i = 0; i < assetnames.Length; i++)
		{
			trans = www.assetBundle.LoadAsset<TextAsset>(assetnames[i]);
			
			XmlDocument xmlDoc = new XmlDocument();
			
			xmlDoc.LoadXml(trans.text);
			
			XmlNode xmlnode = xmlDoc.GetElementsByTagName("level").Item(0).FirstChild;
			
			GameObject go = Instantiate(langButtonPrefab) as GameObject;
			go.transform.SetParent(langList);
			go.transform.localScale = Vector3.one;
			go.name = xmlnode.Attributes["name"].Value;
			go.GetComponentInChildren<Text>().text = xmlnode.ChildNodes[0].InnerText;
			
			getShortFromLang(go.GetComponent<Button>(), go.name);
		}
		
		isLoading = false;
		www.assetBundle.Unload(false);
		www.Dispose();
	}
	
	void getShortFromLang(Button b, string shor)
	{
		b.onClick.AddListener (delegate {GameManager.Instance.transPostfix = shor; StartCoroutine(GetTranslations());});;
	}
	
	public IEnumerator GetTranslations()
	{
		while(true)
		{
			if(!isLoading)
			{
				obj.Clear ();
				isLoading = true;
				
				WWW www = new WWW("file://"+Application.dataPath.Replace("Assets", "")+"/AssetBundles/langs.unity3d");
				yield return www;
				
				trans = www.assetBundle.LoadAsset<TextAsset>("lines_" + GameManager.Instance.TransPostfix+".xml");
				
				XmlDocument xmlDoc = new XmlDocument();
				
				xmlDoc.LoadXml(trans.text);
				
				XmlNodeList levelsList = xmlDoc.GetElementsByTagName("level");
				
				foreach (XmlNode levelInfo in levelsList)
				{
					XmlNodeList levelcontent = levelInfo.ChildNodes;
					
					for(int i = 1; i < levelcontent.Count; i++)
					{
						string toadd = levelcontent[i].InnerText;
						toadd = toadd.Replace("[br]","\n");
						obj.Add(levelcontent[i].Attributes["name"].Value,toadd);
						
					}
				}
				isLoading = false;
				www.assetBundle.Unload(false);
				www.Dispose();
				//to do refresh
				GameManager.Instance.TransPostfix = GameManager.Instance.TransPostfix;
				
				break;
			}
			else
				yield return new WaitForSeconds(0.2f);
		}
	}
	
	private  static TranslationLoader instance;  
	
	private TranslationLoader() {}   
	
	public static TranslationLoader Instance   
	{    
		get    
		{      
			if (instance ==  null)
				instance = GameObject.FindObjectOfType(typeof(TranslationLoader)) as  TranslationLoader;     
			return instance;   
		}  
	}
}