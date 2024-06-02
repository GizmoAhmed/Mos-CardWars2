using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Card : NetworkBehaviour
{
	public PlayerManager playerManager;

	public Image image;

	public bool Grabbed;
	public bool Movable = true;

	public GameObject StartParent;
	public Vector2 StartPos;
	public GameObject NewDropZone;
	public bool isOverDropZone;

	[Tooltip("The Land space this card is on")]
	public GameObject myLand;

	[Tooltip("Whether Card is flipped or not")]
	public enum Face
	{
		FaceUp,
		FaceDown
	}

	public Face cardFace = Face.FaceUp;

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
		image = GetComponent<Image>();

		if (isOwned)
		{
			Movable = true;
		}
		else
		{
			Movable = false;
		}
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

	public void Grab()
	{
		if (!Movable) return;
		Grabbed = true;

		StartParent = transform.parent.gameObject;
		StartPos = transform.position;
	}

	public void LetGo()
	{
		if (!Movable) return;

		Grabbed = false;

		if (isOverDropZone)
		{
			HandleYourPlacement(NewDropZone);
		}
		else
		{
			transform.position = StartPos;
			transform.SetParent(StartParent.transform, false);
		}

		image.color = Color.white;
	}

	public void HandleYourPlacement(GameObject land)
	{

		Debug.Log("Handling Placement...");

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
		if (currentState == CardState.Placed)
		{
			Movable = false;
		}

		if (Movable && Grabbed)
		{
			transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}

		if (isOverDropZone)
		{
			image.color = Color.green;
		}
		else
		{
			image.color = Color.white;
		}
	}
}
