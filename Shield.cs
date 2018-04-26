using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
	public void showShieldPart()
	{
		if(transform.Find("shieldPart") != null)
		{
			GameObject shieldPart = transform.Find("shieldPart").gameObject;
			shieldPart.GetComponent<SpriteRenderer> ().enabled = true;
			shieldPart.transform.SetParent (BeansSpawner.Instance.transform);
			Destroy (shieldPart, 8f);
		}
	}

	public void detach()
	{
		if(transform.Find("shield_trash") != null)
		{
			GameObject shieldPart = transform.Find("shield_trash").gameObject;
			shieldPart.GetComponent<SpriteRenderer> ().enabled = true;
			shieldPart.transform.SetParent (BeansSpawner.Instance.transform);
			Destroy (shieldPart, 8f);
		}
		Destroy (this);
	}
}
