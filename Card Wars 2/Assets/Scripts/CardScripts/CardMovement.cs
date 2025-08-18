using System;
using Mirror;
using UnityEngine;

public class CardMovement : NetworkBehaviour
{
    [HideInInspector] public Player player;

    private bool Grabbed;
    private bool returningToParent = false;

    private GameObject StartParent;
    private Vector2 StartPos;
    public GameObject NewDropZone;

    public bool onLand;

    [Tooltip("ideally should be 12f")] public float cardSnapSpeed = 12f;
    
    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        player = networkIdentity.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("player is null");
        }

        onLand = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Land1"))
        {
            // make new drop zone, if ok to place on said drop zone
            MiddleLand landScript = other.gameObject.GetComponent<MiddleLand>();
        
            if (landScript == null)
            {
                Debug.LogError($"landScript on {NewDropZone.name} is null");
                return;
            }

            if (landScript.ValidPlace(this))
            {
                NewDropZone = other.gameObject;
            }
            else
            {
                NewDropZone = null;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        NewDropZone = null;
    }

    public void BeginDrag()
    {
        if (!isOwned) // can only move your own cards
        {
            return;
        }

        if (onLand) // can't move cards you already placed
        {
            return;
        }

        Grabbed = true;

        StartParent = transform.parent.gameObject;
        StartPos = transform.position;
    }

    public void EndDrag()
    {
        if (!isOwned) // can only move your own cards
        {
            return;
        }
        
        if (onLand) // can't move cards you already placed
        {
            return;
        }

        Grabbed = false;
        
        if (NewDropZone) // because not null, means it passed valid place check, would be null otherwise
        {
            PlaceCard(NewDropZone);
        }
        else
        {
            // snap back
            transform.position = StartPos;
            transform.SetParent(StartParent.transform, false);
        }
    }

    private void PlaceCard(GameObject land)
    {
        player.cardHandler.CmdDropCard(gameObject, land);
    }
    
    void Update()
    {
        Vector2 currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Grabbed)
        {
            transform.position = Vector3.Lerp(transform.position, currentMousePos, Time.deltaTime * cardSnapSpeed);
        }
        else if (returningToParent)
        {
            transform.position = Vector3.Lerp(transform.position, StartPos, Time.deltaTime * cardSnapSpeed);

            // stop lerping when close to parent
            if (Vector3.Distance(transform.position, StartPos) < 0.1f)
            {
                transform.position = StartPos;
                transform.SetParent(StartParent.transform, false);
                returningToParent = false;
            }
        }
    }
}
