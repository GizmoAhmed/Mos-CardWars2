using System.Collections;
using System.Collections.Generic;
using CardScripts.CardDisplays;
using CardScripts.CardStatss;
using Mirror;
using Modal;
using PlayerStuff;
using Tiles;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardScripts.CardMovements
{
    public class CardMovement : NetworkBehaviour
    {
        public enum CardState
        {
            Deck,
            Preview,
            Hand,
            Field,
            Discard
        }

        [SyncVar] public CardState cardState;

        protected CardStats cardStats;
        public CardStats CardStats => cardStats;

        [SyncVar] public PlayerStats thisCardOwnerPlayerStats;

        [HideInInspector] public Tile currentLand = null;

        private bool _grabbed;
        private GameObject _startParent;
        private Vector3 _startPos;
        private GameObject _newDropZone;

        private bool _outsideDrawModal; // for previewed cards

        private Transform _dragLayer;
        [Tooltip("Seconds for snapping back")] public float snapBackDuration = 0.25f;

        private CanvasGroup _canvasGroup;

        private CardDisplay _cardDisplay;

        private CardInfoHandler _cardInfoHandler;

        private const int CLICKABLE_RECT_WIDTH = 147;

        protected virtual void Start()
        {
            cardStats = GetComponent<CardStats>();

            if (_dragLayer == null)
            {
                GameObject dragObj = GameObject.FindWithTag("Drag");
                if (dragObj != null) _dragLayer = dragObj.transform;
            }

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _cardDisplay = GetComponentInParent<CardDisplay>();

            if (_cardDisplay == null)
                Debug.LogError($"_cardDisplay is null ({gameObject.name})");

            _cardInfoHandler = FindObjectOfType<CardInfoHandler>();
        }

        [Server]
        public void SetOwningPlayer(PlayerStats stats)
        {
            thisCardOwnerPlayerStats = stats;
        }

        [Command]
        public void CmdSetCardState(CardState newState)
        {
            SetCardState(newState);
        }

        private void SetCardState(CardState newState)
        {
            cardState = newState;
        }

        public void BeginDrag()
        {
            if (cardState == CardState.Preview)
            {
                // cards in the draw modal preview are draggable
            }
            else
            {
                // DON'T allow to be grabbed if:
                // already being grabbed, is already on a land, or is not your card to grab 
                if (_grabbed || currentLand != null || !isOwned) return;
            }

            _grabbed = true;

            _startParent = transform.parent.gameObject;
            _startPos = transform.position;

            transform.SetParent(_dragLayer, true); // move out of LayoutGroup
            transform.SetAsLastSibling(); // render on top
            _canvasGroup.blocksRaycasts = false; // allow dragging through raycast

            _cardInfoHandler.CloseAnyOpenInfo();
        }

        public void EndDrag()
        {
            if (!_grabbed) return;

            _grabbed = false;
            _canvasGroup.blocksRaycasts = true;


            if (cardState == CardState.Preview && _outsideDrawModal)
            {
                if (CanPickFromPreview()) 
                {
                    // CanPickFromPreview, if true, subtracts from the picks left in draw modal
                    
                }
            }

            if (_newDropZone != null)
            {
                CmdPlaceCardOnTile(_newDropZone);
            }
            else
            {
                StartCoroutine(SnapBackToHand());
            }
        }

        private bool CanPickFromPreview()
        {
            Debug.Log($"Player choosing {gameObject.name} to draw");
            DrawModal drawModal = FindObjectOfType<DrawModal>();

            if (drawModal == null)
            {
                Debug.LogError($"{gameObject.name} couldn't find the drawModal");
                return false;
            }

            if (drawModal.picksLeft != 0) // there are picks left
            {
                Debug.Log($"Player picks {gameObject.name} to draw, as there are enough picks");
                drawModal.UpdatePicksLeft(drawModal.picksLeft -= 1);
                return true;
            }
            
            Debug.LogWarning($"Player could NOT pick {gameObject.name} as they have no picks left");
            return false;
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
            CmdSetCardState(CardState.Field);
        }

        public void OnClick()
        {
            // Debug.Log($"Clicked on: {gameObject.name}");

            _cardDisplay.OnCardClicked();
        }

        private void Update()
        {
            if (_grabbed)
            {
                // follow mouse
                Vector2 mousePos = Input.mousePosition;
                transform.position = Vector3.Lerp(transform.position, mousePos, Time.deltaTime * 12f);

                if (cardState == CardState.Preview)
                {
                    DrawModal? drawModal = GetUIElementUnderPointer<DrawModal>();

                    if (drawModal != null)
                    {
                        // Debug.LogWarning($"{gameObject.name} is above {drawModal}");
                        _outsideDrawModal = false;
                    }
                    else
                    {
                        // Debug.Log($"{gameObject.name} is not above the draw modal");
                        _outsideDrawModal = true;
                    }

                    return;
                }

                // Get the land under the mouse, no need for colliders anymore
                Tile landComponent = GetUIElementUnderPointer<Tile>();

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

        protected virtual bool ValidPlacement(Tile tile)
        {
            if (cardState == CardState.Preview)
            {
                // Debug.LogWarning($"Preview Card {gameObject.name} is asking about valid placement on {tile.gameObject.name}");
                return false;
            }

            Player cardsPlayer = thisCardOwnerPlayerStats.GetComponent<Player>();

            // if not your turn, you can't place a card anywhere
            if (cardsPlayer != null &&
                cardsPlayer.myTurn == false)
            {
                return false;
            }

            // if player has negative magicUse and this card cost something, can't place
            if (cardStats.magicUse > 0 & thisCardOwnerPlayerStats.currentMagic <= 0)
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
            thisCardOwnerPlayerStats.GetComponent<CardHandler>().MoveToDiscard(gameObject);

            // _cardDisplay.ToggleInfoSlide(false);

            if (cardState == CardState.Field) DetachFromTile();

            cardStats.CmdRefreshCardStats();

            CmdSetCardState(CardState.Discard);
        }

        protected virtual void DetachFromTile()
        {
            currentLand = null;
        }
    }
}