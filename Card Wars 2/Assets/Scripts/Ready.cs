using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ready : NetworkBehaviour
{
	public GameManager gameManager;

	public void OnClick() 
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		gameManager.Ready();
	}
}
