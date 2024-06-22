using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseLand : Phase
{
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[ClientRpc]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering ChooseLand phase...");
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

		Debug.Log("Exiting ChooseLand phase...");

		GameObject[] lands = GameObject.FindGameObjectsWithTag("CreatureLand");

		foreach (GameObject land in lands)
		{
			foreach (Transform child in land.transform)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		base.HandlePhaseLogic();
		Debug.Log("Handling logic for ChooseLand phase on server...");
	}

	/*[ClientRpc]
	public void RpcReady()
	{
		if (isServer)
		{
			foreach (var pair in selectedElements)
			{
				var land = pair.Key.GetComponent<CreatureLand>();
				land.AssignElement(pair.Value);
			}
			selectedElements.Clear();
		}
		else
		{
			// Call the server command to notify the server the player is ready
			CmdPlayerReady();
		}
	}

	[Command]
	private void CmdPlayerReady()
	{
		RpcReady();
	}*/
}
