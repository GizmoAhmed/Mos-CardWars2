using Mirror;
using UnityEngine;

public class ChooseLand : Phase
{
	[Server]
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[ClientRpc]
	public override void OnEnterPhase()
	{
		GameObject[] lands = GameObject.FindGameObjectsWithTag("CreatureLand");

		foreach (GameObject land in lands)
		{
			foreach (Transform child in land.transform)
			{
				child.gameObject.SetActive(true);
			}
		}
	}

	[ClientRpc]
	public override void OnExitPhase()
	{
		GameObject[] lands = GameObject.FindGameObjectsWithTag("CreatureLand");

		foreach (GameObject land in lands)
		{
			foreach (Transform child in land.transform)
			{
				child.gameObject.SetActive(false);
			}
		}
	}
}
