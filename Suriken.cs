using UnityEngine;
using System.Collections;

public class Suriken : MonoBehaviour 
{
	public float speedDivider = 6.3f;

	public int damage = 1;

	public Gaysha gaysha;

	GameObject sprite;

	void Start()
	{
		sprite = transform.GetComponentInChildren<SpriteRenderer> ().gameObject;
		StartCoroutine (moving ());
		StartCoroutine (spinning ());
	}

	IEnumerator spinning()
	{
		while(true)
		{
			sprite.transform.RotateAround(sprite.transform.position, -Vector3.back, 16f);
			yield return new WaitForSeconds(0.03f);
		}
	}

	IEnumerator moving()
	{
		while (true) 
		{
			transform.Translate (Vector3.down/speedDivider);
			yield return new WaitForSeconds(0.05f);
		}
	}

	void OnMouseDown()
	{
		if(Time.timeScale != 0f)
		{
			gaysha.removeSuriken (this);
			Destroy (gameObject);
		}
	}
}
