using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMovement : NetworkBehaviour
{
    [HideInInspector] public Player player;

    private bool Grabbed = false;
    private GameObject StartParent;
    private Vector3 StartPos;
    public GameObject NewDropZone;
    public bool onLand = false;

    public Transform dragLayer; 
    [Tooltip("Seconds for snapping back")] public float snapBackDuration = 0.25f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        player = networkIdentity.GetComponent<Player>();

        if (player == null)
            Debug.LogError("player is null");

        if (dragLayer == null)
        {
            GameObject dragObj = GameObject.FindWithTag("Drag");
            if (dragObj != null) dragLayer = dragObj.transform;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void BeginDrag()
    {
        if (Grabbed || onLand || !isOwned) return;

        Grabbed = true;

        StartParent = transform.parent.gameObject;
        StartPos = transform.position;

        transform.SetParent(dragLayer, true); // move out of LayoutGroup
        transform.SetAsLastSibling();         // render on top
        canvasGroup.blocksRaycasts = false;   // allow dragging through raycast
    }

    public void EndDrag()
    {
        if (!Grabbed) return;

        Grabbed = false;
        canvasGroup.blocksRaycasts = true;

        if (NewDropZone != null)
        {
            PlaceCard(NewDropZone);
        }
        else
        {
            StartCoroutine(SnapBackToHand());
        }
    }

    private void PlaceCard(GameObject land)
    {
        player.cardHandler.CmdDropCard(gameObject, land);
    }

    private void Update()
    {
        if (Grabbed)
        {
            Vector2 mousePos = Input.mousePosition;
            transform.position = Vector3.Lerp(transform.position, mousePos, Time.deltaTime * 12f);

            // Get the land under the mouse
            MiddleLand landComponent = GetUIElementUnderPointer<MiddleLand>();

            // Only assign if it's a valid place for this card
            if (landComponent != null && landComponent.ValidPlace(this))
            {
                NewDropZone = landComponent.gameObject;
            }
            else
            {
                NewDropZone = null; // snap back if invalid
            }
        }
    }



    private T GetUIElementUnderPointer<T>() where T : MonoBehaviour
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            T component = result.gameObject.GetComponent<T>();
            if (component != null)
                return component;
        }
        return null;
    }

    private IEnumerator SnapBackToHand()
    {
        Vector3 start = transform.position;
        Vector3 end = StartPos;
        float t = 0f;
        
        // scuffed lerp
        while (t < 1f)
        {
            t += Time.deltaTime / snapBackDuration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        // Reparent back to hand LayoutGroup
        transform.SetParent(StartParent.transform, false);
        transform.localPosition = Vector3.zero;
    }
}
