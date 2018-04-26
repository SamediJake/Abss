using UnityEngine;
using UnityEngine.UI;

public class MyTextFollower : MonoBehaviour 
{	
	TextMesh textMesh;
	Text text;

	void OnEnable()
	{
		textMesh = GetComponent<TextMesh> ();
		text = GetComponent<Text> ();
		GameManager.Instance.changeTranslation += changeTranslation;
	}

	public void changeTranslation()
	{
		/*if(textMesh != null)
			print (gameObject.name + "||" + textMesh.text);
		else if(text != null)
			print (gameObject.name + "||" + text.text);*/

		if(textMesh != null)
			textMesh.text = TranslationLoader.Instance.obj[gameObject.name];
		else if(text != null)
			text.text = TranslationLoader.Instance.obj[gameObject.name];
	}
}
