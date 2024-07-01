using Mirror;
using UnityEngine;

public class ChooseLand : Phase
{
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
		base.OnExitPhase();

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
