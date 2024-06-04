using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;


public class FloopExit : NetworkBehaviour
{
	public PlayerManager playerManager;

	public GameObject card;
    public CardZoom zoom;

	public GameObject floopButton;

	public bool Owned;
	public bool placed;

	private void Start()
	{
		zoom = card.GetComponent<CardZoom>();

		Card cardScript = card.GetComponent<Card>();

		// Check if floopButton is not assigned
		if (floopButton == null)
		{
			Debug.LogWarning("FloopButton is not assigned! Please assign it in the Inspector.");
			return; 
		}

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();

		Owned = cardScript.IsOwnedByLocalPlayer();

		placed = cardScript.currentState == Card.CardState.Placed;

		// get the button component
		Button fB = floopButton.GetComponent<Button>();
		
		fB.interactable = Owned && placed;
	}
	public void floop() 
    {
        Debug.Log("To Be overwritten by different creatures");

		exit();
    }

    public void exit() 
    {
		Debug.Log("exiting zoom...");

		zoom.ZoomOut();
	}
}
