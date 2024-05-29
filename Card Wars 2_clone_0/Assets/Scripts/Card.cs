using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
	public PlayerManager playerManager;

	public bool Grabbed;
    public bool Movable = true;

    [Tooltip("true = front, visible, false = back, logo")]
    public bool Flipped; 

    public GameObject   StartParent;
    public Vector2      StartPos;
    public GameObject   NewDropZone;
    public bool         isOverDropZone;

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

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Land"))
		{
			NewDropZone = other.gameObject;
			isOverDropZone = true;
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
            transform.SetParent(NewDropZone.transform, true);
			transform.localPosition = Vector2.zero;
			Movable = false;
            

			NetworkIdentity networkIdentity = NetworkClient.connection.identity;
			playerManager = networkIdentity.GetComponent<PlayerManager>();

            SetState(CardState.Placed);
            playerManager.DropCard(gameObject, currentState);
        }
        else 
        {
            transform.position = StartPos;
            transform.SetParent(StartParent.transform, false);
        }
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
        
    }

}
