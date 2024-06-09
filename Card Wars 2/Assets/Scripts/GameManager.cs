using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public int CurrentTurn;
	public enum TurnTable 
	{
		GameInactive,
		BothSetUp,
		ThisPlayerSetUp,
		OtherPlayerSetUp,
		
		ThisPlayerAttack,
		OtherPlayerAttack,
	}
}