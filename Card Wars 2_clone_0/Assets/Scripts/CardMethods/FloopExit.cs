using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;


public class FloopExit : NetworkBehaviour
{
	public Player playerManager;

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
			Debug.Log("Probably a building...");
			return; 
		}

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<Player>();

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
		zoom.ZoomOut();
	}
}
