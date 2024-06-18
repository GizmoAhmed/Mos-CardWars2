using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	// [SyncVar]
	public int CurrentTurn;

	public bool isYourTurn;

	public void Ready() 
	{
		Debug.Log("You're Ready...");
	}
}