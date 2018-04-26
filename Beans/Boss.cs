using UnityEngine;
using System.Collections;

public class Boss : Bean 
{
	public override void die ()
	{
		GameManager Gm = GameManager.Instance;
		Gm.bossKiled [BeansSpawner.Instance.getIndexOfBoss(gameObject)]++;
		BeansSpawner.Instance.canSpawn = false;

		Invoke("changeTable", 4f);

		base.die ();
	}

	public void changeTable()
	{
		Boss boss = BeansSpawner.Instance.getBossByDesiredScore(GameManager.Instance.score);

		if (boss != null)
			GameManager.Instance.StartCoroutine (GameManager.Instance.changeTable (GameObject.Find(boss.gameObject.name+"_table").transform, boss.gameObject, GameManager.Instance.singleFormation));
		else
			GameManager.Instance.StartCoroutine (GameManager.Instance.changeTable (GameObject.FindGameObjectWithTag("mainTable").transform, null, null));
	}
}
