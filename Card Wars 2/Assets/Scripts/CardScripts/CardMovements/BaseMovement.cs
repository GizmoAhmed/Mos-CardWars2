using System.Collections;
using System.Collections.Generic;
using CardScripts.CardDisplays;
using CardScripts.CardStatss;
using Lands;
using Mirror;
using PlayerStuff;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardScripts.CardMovements
{
    public class BaseMovement : NetworkBehaviour
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

        protected CardStats cardStats;
        public CardStats CardStats => cardStats;

        public PlayerStats thisPlayersStats;

        private Player cardOwner;

        [HideInInspector] public MiddleLand currentLand = null;

        private bool _grabbed = false;
        private GameObject _startParent;
        private Vector3 _startPos;
        private GameObject _newDropZone;

        private Transform _dragLayer;
        [Tooltip("Seconds for snapping back")] public float snapBackDuration = 0.25f;

        private CanvasGroup _canvasGroup;

        private CardDisplay _cardDisplay;

        private const int CLICKABLE_RECT_WIDTH = 147;

        protected virtual void Start()
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            player = networkIdentity.GetComponent<Player>();

            cardStats = GetComponent<CardStats>();

            cardOwner = cardStats.thisCardOwner.gameObject.GetComponent<Player>();

            thisPlayersStats = cardStats.thisCardOwner.gameObject.GetComponent<PlayerStats>();

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

            if (_cardDisplay == null)
                Debug.LogError($"_cardDisplay is null ({gameObject.name})");
        }

        public void BeginDrag()
        {
            if (_grabbed || currentLand != null || !isOwned) return;

            _grabbed = true;

            _startParent = transform.parent.gameObject;
            _startPos = transform.position;

            transform.SetParent(_dragLayer, true); // move out of LayoutGroup
            transform.SetAsLastSibling(); // render on top
            _canvasGroup.blocksRaycasts = false; // allow dragging through raycast

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
                // player.cardHandler.CmdDropCard(gameObject, _newDropZone);
                CmdPlaceCardOnTile(_newDropZone);
            }
            else
            {
                StartCoroutine(SnapBackToHand());
            }
        }

        [Command] // placing this card (this gameObject) on the tile (_newDropZone) it's over 
        private void CmdPlaceCardOnTile(GameObject tile)
        {
            RpcPlaceCardOnTile(tile);
        }

        [ClientRpc]
        protected virtual void RpcPlaceCardOnTile(GameObject tileObj)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            rectTransform.sizeDelta =
                new Vector2(CLICKABLE_RECT_WIDTH,
                    rectTransform.sizeDelta.y); // consistent clickable area size, when dropped on field

            _cardDisplay.FlipCard(true); // now on field, show to both players
            cardState = CardState.Field;
        }

        public void OnClick()
        {
            Debug.Log($"Clicked on: {gameObject.name}");

            _cardDisplay.ToggleInfoSlide();
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
                if (landComponent != null && ValidPlacement(landComponent))
                {
                    _newDropZone = landComponent.gameObject;
                }
                else
                {
                    _newDropZone = null; // snap back if invalid
                }
            }
        }

        protected virtual bool ValidPlacement(MiddleLand land)
        {
            // if not your turn, you can't place a card anywhere
            if (cardOwner != null &&
                cardOwner.myTurn == false)
            {
                return false;
            }

            // if player has negative magicUse and this card cost something, can't place
            if (cardStats.magicUse > 0 & thisPlayersStats.currentMagic <= 0)
            {
                return false;
            }

            return true;
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

        [ClientRpc] // use when cards themselves call to discard, requiring server-issued call, (ie burn)
        public void RpcDiscard()
        {
            Discard();
        }

        // use if separate rpc function needs a discard (ie CardHandler)
        protected virtual void Discard()
        {
            gameObject.GetComponent<CardStats>().thisCardOwner.GetComponent<CardHandler>().MoveToDiscard(gameObject);

            _cardDisplay.ToggleInfoSlide(false);
            
            if (cardState == CardState.Field) DetachFromTile();

            cardStats.RefreshCardStats();

            cardState = CardState.Discard;
        }

        protected virtual void DetachFromTile()
        {
            currentLand = null;
        }
    }
}