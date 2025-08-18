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

    [Tooltip("ideally should be 12f")] public float cardSnapSpeed = 12f;
    
    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        player = networkIdentity.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("player is null");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Land"))
        {
            NewDropZone = other.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        NewDropZone = null;
    }

    public void BeginDrag()
    {
        Grabbed = true;

        StartParent = transform.parent.gameObject;
        StartPos = transform.position;
    }

    public void EndDrag()
    {
        // if (!Movable) return;

        Grabbed = false;

        if (NewDropZone)
        {
            PlaceCard(NewDropZone);
        }
        else
        {
            transform.position = StartPos;
            transform.SetParent(StartParent.transform, false);
        }
    }

    private void PlaceCard(GameObject land)
    {
        transform.SetParent(land.transform, true);
        transform.localPosition = Vector2.zero;
        
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
