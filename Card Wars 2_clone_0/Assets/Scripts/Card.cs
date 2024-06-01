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
		Land landscript = other.GetComponent<Land>();

        if (landscript != null) 
        {
            if (other.CompareTag("Land") && isOwned && landscript.Taken == false)
            {
                NewDropZone = other.gameObject;
			    isOverDropZone = true;
            }

            /*else if (other.CompareTag("Player2Lands") && !isOwned) 
            {
			    isOverDropZone = true;
		    }*/
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

        if (isOverDropZone) // && newDropZone is accepting
        {
            HandlePlacement();
		}
        else 
        {
            transform.position = StartPos;
            transform.SetParent(StartParent.transform, false);
        }

		image.color = Color.white;
	}

    public void HandlePlacement()
    {
        Movable = false;

        /// link them
        Land landscript = NewDropZone.GetComponent<Land>();
        landscript.AttachCard(gameObject);
        myLand = NewDropZone;

        transform.SetParent(NewDropZone.transform, true);
        transform.localPosition = Vector2.zero;


        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();

		SetState(CardState.Placed);
		playerManager.CmdDropCard(gameObject, currentState);
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
