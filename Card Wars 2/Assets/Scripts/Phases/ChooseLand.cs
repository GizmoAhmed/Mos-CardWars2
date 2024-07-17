using Mirror;
using Unity.VisualScripting;
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
		ShowLandButtons(true);
	}

	[ClientRpc]
	public override void OnExitPhase()
	{
		ShowLandButtons(false);
	}

	public void ShowLandButtons(bool enable)
	{
		GameObject[] lands = GameObject.FindGameObjectsWithTag("CreatureLand");

		foreach (GameObject land in lands)
		{
			foreach (Transform child in land.transform)
			{
				child.gameObject.SetActive(enable);
			}
		}
	}
}
