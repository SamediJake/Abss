using UnityEngine;
using System.Collections;

public class Ctulhu : Boss 
{
	public override IEnumerator moving ()
	{
		while(true)
		{
			if(!isDead)
			{
				if(canMove())
				{
					transform.parent.transform.position = new Vector2 (transform.parent.transform.position.x, transform.parent.transform.position.y - speed/12f);
				}
				yield return new WaitForSeconds(0.05f);
			}
			else break;
		}
	}

	public override void die ()
	{
		base.die ();

		GameManager.Instance.setAchievementStatus (19, true);

		bool allBossesKilled = true;

		for(int i = 0; i < GameManager.Instance.bossKiled.Count; i++)
			allBossesKilled = GameManager.Instance.bossKiled[i] > 0;

		if(allBossesKilled)
			GameManager.Instance.setAchievementStatus(20, true);
	}
}
