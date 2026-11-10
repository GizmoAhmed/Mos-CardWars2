#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Data;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using Modal;
using PlayerStuff;
using Tiles;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        // For preview cards only (client-side reference)
        private DeckCollection _owningDeck;

        [Header("Logical Position (Server Authority - Game Logic)")] [SyncVar]
        public int logicalRow = -1;

        [SyncVar] public int logicalColumn = -1;
        [SyncVar] public int logicalPlayerSide = -1;

        [Header("Visual Reference (Client-side - Rendering Only)")]
        public Tile thisCardsVisualTile = null;

        private bool _grabbed;
        private GameObject _startParent;
        private Vector3 _startPos;
        private GameObject _newDropZone;

        private Tile _hoveredTile;

        private bool _outsideDrawModal; // for previewed cards

        private Transform _dragLayer;
        [Tooltip("Seconds for snapping back")] public float snapBackDuration = 0.25f;

        protected CanvasGroup _canvasGroup;

        protected CardDisplay _cardDisplay;

        private CardInfoHandler _cardInfoHandler;

        private const int CLICKABLE_RECT_WIDTH = 147;

        protected CardAnimator CardAnimator;

        private Player _cardsPlayer;

        public bool CanMoveCard
        {
            get
            {
                if (_cardsPlayer == null)
                    _cardsPlayer = thisCardOwnerPlayerStats.GetComponent<Player>();

                return _cardsPlayer == null || _cardsPlayer.myTurn;
            }
        }

        private bool CanBeGrabbed(out string reason)
        {
            reason = null;

            if (cardState == CardState.Preview)    return true;
            if (_grabbed)                           { reason = "already grabbed"; return false; }
            if (thisCardsVisualTile != null)    { reason = "already on a tile"; return false; }
            if (!isOwned)                          { reason = "not owned by this player"; return false; }
            if (!CanMoveCard)                      { reason = "not your turn"; return false; }
            if (cardState == CardState.Discard)    { reason = "in discard"; return false; }

            return true;
        }

        [Header("Discard Effects")] public float discardFadeOutDuration = 1f;

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

            CardAnimator = GetComponent<CardAnimator>();
        }

        [Server]
        public void SetOwningPlayer(PlayerStats stats)
        {
            thisCardOwnerPlayerStats = stats;

            // set logical side to whoever placed the card
            Player thisPlayer = thisCardOwnerPlayerStats.GetComponent<Player>();
            logicalPlayerSide = thisPlayer.playerSide;
        }

        /// <summary>
        /// Set which deck this preview card came from (client-side only)
        /// </summary>
        public void SetOwningDeck(DeckCollection deck)
        {
            _owningDeck = deck;
        }

        public void BeginDrag()
        {
            if (!CanBeGrabbed(out string reason))
            {
                Debug.Log($"<color=orange>Drag denied</color> on {gameObject.name}: {reason}");
                return;
            }

            _grabbed = true;

            _startParent = transform.parent.gameObject;
            _startPos = transform.position;

            transform.SetParent(_dragLayer, true); // move out of LayoutGroup
            transform.SetAsLastSibling(); // render on top
            _canvasGroup.blocksRaycasts = false; // allow dragging through raycast

            _cardInfoHandler.CloseAnyOpenInfo();

            // tell card manager that this card is being dragged
            CardDragManager.Instance?.RegisterDraggedCard(this);
        }

        public virtual void EndDrag()
        {
            if (!_grabbed) return;

            _grabbed = false;
            _canvasGroup.blocksRaycasts = true;

            // tell drag manager this card isn't being dragged anymore
            CardDragManager.Instance?.UnregisterDraggedCard(this);

            if (cardState == CardState.Preview && _outsideDrawModal)
            {
                if (CanPickFromPreview())
                {
                    SpawnThisCardFromPreview();
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

            // Clear hovered tile
            _hoveredTile = null;
        }

        /// <summary>
        /// Called by TileHoverDetector when mouse enters tile
        /// </summary>
        public void OnTileHoverEnter(Tile tile)
        {
            _hoveredTile = tile;

            // Validate ONCE when entering
            if (ValidPlacement(tile))
            {
                _newDropZone = tile.gameObject;
            }
            else
            {
                _newDropZone = null;
            }
        }

        /// <summary>
        /// Called by TileHoverDetector when mouse exits tile
        /// </summary>
        public void OnTileHoverExit(Tile tile)
        {
            if (_hoveredTile == tile)
            {
                _hoveredTile = null;
                _newDropZone = null;

                // Optional: Visual feedback
                // tile.HidePlacementIndicator();
            }
        }

        private void SpawnThisCardFromPreview()
        {
            CardStats stats = GetComponent<CardStats>();

            if (stats != null && stats.cardData != null && _owningDeck != null)
            {
                // Tell the deck to spawn this card
                _owningDeck.CmdDrawCardByID(stats.cardData.cardID);

                // Destroy the preview card
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Missing card data or owning deck!");
            }
        }

        private bool CanPickFromPreview()
        {
            // Debug.Log($"Player choosing {gameObject.name} to draw");
            DrawModal drawModal = FindObjectOfType<DrawModal>();

            if (drawModal == null)
            {
                Debug.LogError($"{gameObject.name} couldn't find the drawModal");
                return false;
            }

            if (drawModal.picksLeft != 0) // there are picks left
            {
                // Debug.Log($"Player picks {gameObject.name} to draw, as there are enough picks");
                drawModal.UpdatePicksLeft(drawModal.picksLeft -= 1);
                return true;
            }

            // Debug.LogWarning($"Player could NOT pick {gameObject.name} as they have no picks left");
            return false;
        }

        [Command] // placing this card (this gameObject) on the tile (_newDropZone) it's over 
        protected virtual void CmdPlaceCardOnTile(GameObject tile)
        {
            Tile tileScript = tile.GetComponent<Tile>();

            // SERVER: Store LOGICAL position (authoritative game state)
            logicalRow = logicalPlayerSide; // tileScript.row; 
            logicalColumn = tileScript.column;

            // SERVER: Set logical reference on tile
            SetLogicalReferenceOnTile(tileScript);

            RpcPlaceCardOnTile(tile);

            cardState = CardState.Field;

            RpcFlipBurn(); // since now on field, use return instead of cost

            // remove from hand counter
            // All others call base, so this will get called there
            // Spell doesn't call base.CmdPlace but it's ok cause it discards itself when that happens which calls this
            // Rune needs it though, so this line will go there
            thisCardOwnerPlayerStats.GetComponent<PlayerCardTracker>().Server_RemoveFromHand(gameObject);

            // you tell the global instance that a card placed, which lets EVERYONE know to trigger their abilities if they care
            GlobalBroadcast_AnyCardPlacement();

            // you tell the tile manager of this creature about how you placed a card on it, not everyone
            LocalWhisperCardPlacement(GetLogicalTile());
        }

        /// <summary>
        /// Tell CardDisplay to show burn return instead of burn cost
        /// </summary>
        [ClientRpc]
        private void RpcFlipBurn()
        {
            _cardDisplay.FlipBurn_ToCostFromReturn(cardStats.burnCost);
        }

        [Server]
        protected virtual void SetLogicalReferenceOnTile(Tile tileScript)
        {
            // Override in child classes (CreatureMovement, BuildingMovement, etc.)
            Debug.LogWarning("SetLogicalReferenceOnTile not overridden!");
        }

        // you tell the global instance that a card placed, which lets EVERYONE know to trigger their abilities if they care
        protected virtual void GlobalBroadcast_AnyCardPlacement()
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData cardPlaceData = new AbilityEventData(
                    AbilityEventType.AnyFieldCardPlaced,
                    gameObject
                );

                // tell event manager to tell everyone (that cares) that this card was placed
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.TriggerEvents_ForAllSubscribersOfType(
                    cardPlaceData);
            }
            else
            {
                Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
            }
        }

        // you tell the tile manager of this creature about how you placed a card on it, not everyone. then tile, will execute abilities of its subscribers
        private void LocalWhisperCardPlacement(Tile tile)
        {
            TileEventManager tileEventManager = tile.gameObject.GetComponent<TileEventManager>();

            // tileManager handles creating and broadcasting the AbilityEventData
            tileEventManager.OnCardPlacedOnTile(gameObject);
        }

        /// <summary>
        /// Add a LayoutElement with Ignore Layout checked to the card when it lands on a tile.
        /// Layout groups skip ignored children entirely, so they stop driving the anchors, position, and size.
        /// You can set them yourself. The hand keeps its layout behavior, and the card only opts out once it’s placed.
        /// </summary>
        /// <param name="newParent">Tile being placed on</param>
        /// <param name="anchoredPos">New position set to when placed</param>
        protected void PlaceOnTileOutsideLayout(Transform newParent, Vector2 anchoredPos)
        {
            RectTransform rt = (RectTransform)transform;

            // Opt out of the layout group BEFORE reparenting, so it never drives this card
            LayoutElement le = GetComponent<LayoutElement>();
            if (le == null) le = gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;

            transform.SetParent(newParent, false);

            // The layout group left the anchors at top-left, so set them explicitly
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            transform.SetAsFirstSibling();
        }

        /// <summary>
        /// Opposite of PlaceOnTileOutsideLayout: hands the card back to a layout group
        /// (hand, discard board) by clearing Ignore Layout and restoring top-left anchors.
        /// </summary>
        private void ReturnToLayout()
        {
            RectTransform rt = (RectTransform)transform;

            if (TryGetComponent(out LayoutElement le))
                le.ignoreLayout = false;

            // match the anchors the layout group expects (top left, as on unbugged cards)
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }

        [ClientRpc]
        protected virtual void RpcPlaceCardOnTile(GameObject tileObj)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            rectTransform.sizeDelta =
                new Vector2(CLICKABLE_RECT_WIDTH,
                    rectTransform.sizeDelta.y); // consistent clickable area size, when dropped on field

            _cardDisplay.FlipCard(true); // now on field, show to both players

            // todo Store visual tile reference (for animations, UI, etc.)
            thisCardsVisualTile = tileObj.GetComponent<Tile>();
        }

        /// <summary>
        /// Base call that broadcasts AnyCardBurned, overrides in the child card types (creature, building, etc) need to call this as well as broadcast their own type
        /// </summary>
        [Server]
        public virtual void GlobalBroadcastCardBurned()
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData burnData = new AbilityEventData(
                    type: AbilityEventType.AnyCardBurned,
                    targ: gameObject
                );

                Debug.Log("<<< <color=cyan>Base: Any Card Burn Broadcast</color> >>>");

                // tell event manager to tell everyone (that cares) that a card was burned
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.TriggerEvents_ForAllSubscribersOfType(
                    burnData);
            }
            else
            {
                Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
            }
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

                // Handle preview card logic (not over tiles)
                if (cardState == CardState.Preview)
                {
                    DrawModal? drawModal = GetUIElementUnderPointer<DrawModal>();
                    _outsideDrawModal = (drawModal == null);
                }

                // Get the land under the mouse, no need for colliders anymore
                /*Tile landComponent = GetUIElementUnderPointer<Tile>();

                // Only assign if it's a valid place for this card
                if (landComponent != null && ValidPlacement(landComponent))
                {
                    _newDropZone = landComponent.gameObject;
                }
                else
                {
                    _newDropZone = null; // snap back if invalid
                }*/
            }
        }

        protected virtual bool ValidPlacement(Tile tile)
        {
            if (cardState == CardState.Preview)
            {
                // Debug.LogWarning($"Preview Card {gameObject.name} is asking about valid placement on {tile.gameObject.name}");
                return false;
            }

            if (!CanMoveCard)
            {
                Debug.LogWarning($"Not this player's ({thisCardOwnerPlayerStats.name.ToLower()}) turn");
                return false;
            }

            // if player has negative soulUse and this card cost something, can't place
            if (cardStats.soulUse > 0 & thisCardOwnerPlayerStats.currentSoul <= 0)
            {
                Debug.LogWarning(
                    $"Not enough soul ({thisCardOwnerPlayerStats.currentSoul}) to place this card ({gameObject.name} [Soul Use = {cardStats.soulUse}])");
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

        protected IEnumerator SnapBackToHand()
        {
            transform.DOKill(); // make sure animation doesn't mess with snap back movement

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

        [Server]
        public virtual void BurnCard()
        {
            //Debug.LogWarning($"BurnCard not overridden on {gameObject.name}! Discarding anyway...");

            ServerDiscard();
        }

        [Server]
        public virtual void ServerDiscard()
        {
            // bother unsubscribing if you are on field (as opposed to hand and preview)
            if (cardState == CardState.Field)
            {
                // this (unsub) goes first (before detach) so that when data is sent from unsubscribe, the tile is still set in custom data in PassiveListenerCard.cs 

                // if listener found, card is passive, unsubscribe its ability
                PassiveListenerCard listen = GetComponent<PassiveListenerCard>();
                if (listen != null) listen.UnsubscribeThisCardFromListening();

                DetachFromTile(); // if on a tile, detach from that tile
            }

            // set state to discard
            cardState = CardState.Discard;

            // resets stats to base, to show on discard board
            cardStats.SetStats_FromData();

            Player player = thisCardOwnerPlayerStats?.GetComponent<Player>();

            // Track discard on server
            player.playerCardTracker.Server_TrackDiscard(gameObject);

            // also remove from hand if discarding from there
            player.playerCardTracker.Server_RemoveFromHand(gameObject);

            LocalWhisperCardDiscard(GetLogicalTile());

            // visually move card to discard board for each respective client
            RpcMoveDiscardedCard_ToBoard();
        }

        protected void LocalWhisperCardDiscard(Tile tile)
        {
            TileEventManager tileEventManager = tile.gameObject.GetComponent<TileEventManager>();

            // tileManager handles creating and broadcasting the AbilityEventData
            tileEventManager.OnCardDiscardFromTile(gameObject);
        }

        protected virtual void DetachFromTile()
        {
            thisCardsVisualTile = null;

            ClearLogicalPositionServer();
        }

        [ClientRpc]
        private void RpcMoveDiscardedCard_ToBoard()
        {
            _cardInfoHandler.CloseAnyOpenInfo();

            CardAnimator.FadeOutThen(_canvasGroup, () =>
            {
                // code in here is ran once the card is completely faded:
                thisCardOwnerPlayerStats.GetComponent<CardPlacement>().MoveCardToDiscard(gameObject);
                ReturnToLayout();
            }, duration: discardFadeOutDuration);
        }

        [Server]
        private void ClearLogicalPositionServer()
        {
            // Clear tile's reference to this card (creature and building, not rune, tile don't care about runes)
            // a rune's tile is actually a creature if you think about it
            Tile logicalTile = GetLogicalTile();

            if (logicalTile != null)
            {
                // overridden in child card types
                ClearLogicalReference_OnTile(logicalTile);
            }
            else
            {
                Debug.LogWarning($"Can't find logical tile for {gameObject.name}\n" +
                                 "Either this was a card in hand, or the tile couldn't be found while on the field, which is an issue");
            }

            // Reset to "not placed" values
            logicalRow = -1;
            logicalColumn = -1;
            logicalPlayerSide = -1;
        }

        [Server]
        protected virtual void ClearLogicalReference_OnTile(Tile tile)
        {
            // Override in child classes
            Debug.LogError($"ClearLogicalReferenceOnTile not overridden on {gameObject.name}!");
        }

        /// <summary>
        /// Get the logical tile based on logical position (logical pos is the same between clients)
        /// Logical Position = row, col, player-side, these are class variables in CardMovement.
        /// </summary>
        public Tile GetLogicalTile() // maybe have this function take row, col, and p as parameters
        {
            if (TileManager.Instance != null)
            {
                Tile thisCardsTile = TileManager.Instance.GetTile(logicalRow, logicalColumn, logicalPlayerSide);

                return thisCardsTile;
            }

            Debug.LogError("TileManager Instance is null");
            return null;
        }

        /// <summary>
        /// Call to get the correct tile when in running in client
        /// Like Get logical tile, expect this one is called from a client, like in ValidPlacement()
        /// </summary>
        /// <returns></returns>
        protected Tile GetServerTileForClient(Tile midTile)
        {
            Tile logTile = null;

            // if client is validating, check the other side since server saves client side placements on the other side
            if (logicalPlayerSide == 1)
            {
                logTile = midTile.across.GetComponent<Tile>(); // return across
            }
            else
            {
                logTile = midTile; // just return the tile you passed
            }

            return logTile;
        }
    }
}