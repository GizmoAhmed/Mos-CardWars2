using System.Collections;
using System.Collections.Generic;
using CardScripts;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMovement : NetworkBehaviour
{
    [HideInInspector] public Player player;

    public enum CardState
    {
        Deck,
        Hand, 
        Field,
        Discard
    }
    
    public CardState cardState;

    public CardStats cardStats;

    [HideInInspector] public MiddleLand currentLand = null;
    
    private bool _grabbed = false;
    private GameObject _startParent;
    private Vector3 _startPos;
    private GameObject _newDropZone;

    private Transform _dragLayer; 
    [Tooltip("Seconds for snapping back")] public float snapBackDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    
    private CardDisplay _cardDisplay;

    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        player = networkIdentity.GetComponent<Player>();
        
        cardStats = GetComponent<CardStats>();

        if (player == null)
            Debug.LogError("player is null");

        if (_dragLayer == null)
        {
            GameObject dragObj = GameObject.FindWithTag("Drag");
            if (dragObj != null) _dragLayer = dragObj.transform;
        }

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        cardState = CardState.Deck;
        
        _cardDisplay = GetComponentInParent<CardDisplay>();
        
        if  (_cardDisplay == null)
            Debug.LogError($"_cardDisplay is null ({gameObject.name})");
    }

    public void BeginDrag()
    {
        if (_grabbed || currentLand != null || !isOwned) return;

        _grabbed = true;

        _startParent = transform.parent.gameObject;
        _startPos = transform.position;

        transform.SetParent(_dragLayer, true); // move out of LayoutGroup
        transform.SetAsLastSibling();         // render on top
        _canvasGroup.blocksRaycasts = false;   // allow dragging through raycast
        
        _cardDisplay.ToggleInfoSlide(false);
    }

    public void EndDrag()
    {
        if (!_grabbed) return;

        _grabbed = false;
        _canvasGroup.blocksRaycasts = true;
        
        _cardDisplay.ToggleInfoSlide(false);

        if (_newDropZone != null)
        {
            PlaceCard(_newDropZone);
        }
        else
        {
            StartCoroutine(SnapBackToHand());
        }
    }

    public void OnClick()
    {
        Debug.Log($"Clicked on: {gameObject.name}");
        
        _cardDisplay.ToggleInfoSlide();
    }

    private void PlaceCard(GameObject land)
    {
        player.cardHandler.CmdDropCard(gameObject, land);
    }

    private void Update()
    {
        if (_grabbed)
        {
            Vector2 mousePos = Input.mousePosition;
            transform.position = Vector3.Lerp(transform.position, mousePos, Time.deltaTime * 12f);

            // Get the land under the mouse, no need for colliders anymore
            MiddleLand landComponent = GetUIElementUnderPointer<MiddleLand>();

            // Only assign if it's a valid place for this card
            if (landComponent != null && landComponent.ValidPlace(this))
            {
                _newDropZone = landComponent.gameObject;
            }
            else
            {
                _newDropZone = null; // snap back if invalid
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
        Vector3 end = _startPos;
        float t = 0f;
        
        // scuffed lerp
        while (t < 1f)
        {
            t += Time.deltaTime / snapBackDuration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        // Reparent back to hand LayoutGroup
        transform.SetParent(_startParent.transform, false);
        transform.localPosition = Vector3.zero;
    }
    
    // use if separate rpc function needs a discard (ie CardHandler)
    public void Discard()
    {
        gameObject.GetComponent<CardStats>().thisCardOwner.GetComponent<CardHandler>().MoveToDiscard(gameObject);
        
        if (cardState == CardState.Field) currentLand.DetachCard(gameObject); // detach from its land if on the field
        
        cardState = CardState.Discard;
    }

    // use when cards themselves call to discard, requiring server-issued call, (ie burn)
    [ClientRpc]
    public void RpcDiscard()
    {
        Discard();
    }
}
