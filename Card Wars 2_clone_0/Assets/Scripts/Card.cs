using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Card : NetworkBehaviour
{
	public PlayerManager playerManager;

	public bool Grabbed;
	public bool Movable = true;

	public GameObject StartParent;
	public Vector2 StartPos;
	public GameObject NewDropZone;
	public bool isOverDropZone;

	public bool isZoomLocked;

	public Vector2 currentMousePos;
	public Vector2 clickSave;

	[Tooltip("The Land space this card is on")]
	public GameObject myLand;

	[Tooltip("Where the card exists")]
	public enum CardState
	{
		Deck,
		Hand,
		Placed
	}

	public CardState currentState = CardState.Deck;

	public void SetState(CardState newState)
	{
		currentState = newState;
	}

	void Start()
	{
		if (isOwned)
		{
			Movable = true;
		}
		else
		{
			Movable = false;
		}
	}

	public bool IsOwnedByLocalPlayer()
	{
		return isOwned;
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		Land landscript = other.GetComponent<Land>();

		if (landscript != null)
		{
			if (other.CompareTag("Land") && isOwned && landscript.Taken == false)
			{
				NewDropZone = other.gameObject;
				isOverDropZone = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		NewDropZone = null;
		isOverDropZone = false;
	}

	public void Zoom()
	{
		CardZoom zoom = GetComponent<CardZoom>();

		zoom.ZoomIn();
	}

	public void PointerDown() 
	{
		clickSave = new Vector2(Input.mousePosition.x, Input.mousePosition.y); 
	}

	public void PointerUp()
	{
		CardFlipper flip = GetComponent<CardFlipper>();

		// If the mouse position has not changed, zoom into the card
		if (clickSave == currentMousePos && flip.currentFace == CardFlipper.FaceState.FaceUp && !isZoomLocked)
		{
			Zoom();
		}
	}


	public void Grab()
	{
		if (!Movable || isZoomLocked) return;

		Grabbed = true;

		StartParent = transform.parent.gameObject;
		StartPos = transform.position;
	}

	public void LetGo()
	{
		if (!Movable || isZoomLocked) return;

		Grabbed = false;

		if (isOverDropZone)
		{
			PlaceCard(NewDropZone);
		}
		else
		{
			transform.position = StartPos;
			transform.SetParent(StartParent.transform, false);
		}
	}

	public void PlaceCard(GameObject land)
	{
		Movable = false;

		Land landscript = land.GetComponent<Land>();

		// Link them
		landscript.AttachCard(gameObject);
		myLand = land;

		transform.SetParent(land.transform, true);
		transform.localPosition = Vector2.zero;

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();

		SetState(CardState.Placed);
		playerManager.CmdDropCard(gameObject, currentState, land);
	}

	void Update()
	{
		currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		if (currentState == CardState.Placed)
		{
			Movable = false;
		}

		if (Movable && Grabbed)
		{
			transform.position = currentMousePos;
		}
	}
}
