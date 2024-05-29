using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Card : NetworkBehaviour
{
	public PlayerManager playerManager;

    public UnityEngine.UI.Image image;

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
        image = GetComponent<UnityEngine.UI.Image>();

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
        if (other.CompareTag("Player1Lands") && isOwned)
        {
            NewDropZone = other.gameObject;
            isOverDropZone = true;
        }
        /*else if (other.CompareTag("Player2Lands") && !isOwned) 
        {
			isOverDropZone = true;
		}*/
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

        if (isOverDropZone) // && newDropZone is accepting
        {
            transform.SetParent(NewDropZone.transform, true);
			transform.localPosition = Vector2.zero;
			Movable = false;
            
			NetworkIdentity networkIdentity = NetworkClient.connection.identity;
			playerManager = networkIdentity.GetComponent<PlayerManager>();

            SetState(CardState.Placed);
            playerManager.CmdDropCard(gameObject, currentState);
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
