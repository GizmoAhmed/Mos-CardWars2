using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public int CurrentTurn;

	private void Awake()
	{
		CurrentTurn = 0;
	}

	public void NextTurn() 
	{
		CurrentTurn++;
	}

	/*public enum GameState 
	{
		GameInactive,
		BothSetUp,
		ThisPlayerSetUp,
		OtherPlayerSetUp,
		
		ThisPlayerAttack,
		OtherPlayerAttack,
	}*/
}