using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardData
{
    public class PassiveListenerCard : NetworkBehaviour
    {
        public CardStats cardStats;
        public PassiveAbilitySO passiveAbility;
        public bool isExecutedOnPlace = false;

        // For global events
        private Dictionary<AbilityEventType, System.Action<AbilityEventData>> _globalCallbacks =
            new Dictionary<AbilityEventType, System.Action<AbilityEventData>>();

        // For middleTile events
        private Dictionary<AbilityEventType, System.Action<AbilityEventData>> _tileCallbacks =
            new Dictionary<AbilityEventType, System.Action<AbilityEventData>>();

        private Tile _subscribedMiddleTile;

        public void InitializePassiveListener(CardStats stats, PassiveAbilitySO p)
        {
            cardStats = stats;

            if (p == null)
            {
                Debug.LogError(
                    $"{gameObject.name} attempted to initialize passive listener with an ability that was either null or not a passive ability");
                return;
            }

            passiveAbility = p;
            isExecutedOnPlace = p.isExecutableOnPlaced;
        }

        public void RegisterPassiveAbility()
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance == null)
            {
                Debug.LogError("GlobalAbilityEventManager not found in scene!");
                return;
            }

            if (isExecutedOnPlace)
            {
                AbilityEventData data = PrepareData_ForPlacementExecution();

                passiveAbility.ExecuteAbility(gameObject, data);
            }

            AbilityEventType[] events = passiveAbility.eventsThatTriggerThisAbility;

            if (events.Length == 0) return; // don't register anything if it has no triggering events
            
            // Register based on scope (global vs middleTile)
            if (passiveAbility.isGlobalListener)
            {
                RegisterGlobalListener(passiveAbility, events);
            }
            else
            {
                RegisterTileListener(passiveAbility, events);
            }
        }

        private void RegisterGlobalListener(PassiveAbilitySO ability, AbilityEventType[] events)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance == null)
            {
                Debug.LogError("AbilityEventManager not found!");
                return;
            }

            foreach (AbilityEventType eventType in events)
            {
                void Callback
                    (AbilityEventData eventData)
                {
                    if (!isServer) return;
                    ability.ExecuteAbility(gameObject, eventData);
                }

                _globalCallbacks[eventType] =
                    Callback; // cards owns this call back, once discarded, remove it from manager

                // this card says: hey event manager, let me know (subscription) when this event (event data)...
                // ...happens (happens ==> TriggerEvents_ForAllSubscribersOfType), so I can run my execution
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.Subscribe(eventType, Callback);
            }
        }

        private void RegisterTileListener(PassiveAbilitySO ability, AbilityEventType[] events)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance == null)
            {
                Debug.LogError("AbilityEventManager not found!");
            }

            // Get the middleTile this card is on
            CardMovement movement = GetComponent<CardMovement>();

            if (movement == null)
            {
                Debug.LogError("CardMovement not found - can't register middleTile listener!");
                return;
            }

            Tile middleTile = movement.GetLogicalTile();
            TileEventManager tileEventManager = middleTile.GetComponent<TileEventManager>();

            if (middleTile == null)
            {
                Debug.LogWarning($"{gameObject.name} not on a middleTile yet - can't register middleTile listener");
                return;
            }

            if (tileEventManager == null)
            {
                Debug.LogError($"This middleTile ({middleTile.gameObject.name}) doesn't have a middleTile listener)");
                return;
            }

            foreach (AbilityEventType eventType in events)
            {
                void Callback(AbilityEventData eventData)
                {
                    if (!isServer) return;
                    ability.ExecuteAbility(gameObject, eventData);
                }

                _tileCallbacks[eventType] = Callback;

                tileEventManager.SubscribeToTileEvent(eventType, Callback);

                _subscribedMiddleTile = middleTile;
            }
        }

        [Server]
        public void UnsubscribeThisCardFromListening()
        {
            // Unsubscribe from global events
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                foreach (var kvp in _globalCallbacks)
                {
                    GlobalAbilityEventManager.GlobalAbilityManagerInstance.GlobalUnsubscribe(kvp.Key, kvp.Value);
                }
            }

            // Unsubscribe from middleTile events
            if (_subscribedMiddleTile != null)
            {
                TileEventManager tileEventManager = _subscribedMiddleTile.GetComponent<TileEventManager>();

                foreach (var kvp in _tileCallbacks)
                {
                    tileEventManager.UnsubscribeFromTileEvent(kvp.Key, kvp.Value);
                }
            }

            _globalCallbacks.Clear();
            _tileCallbacks.Clear();

            AbilityEventData data = PrepareData_ForPlacementExecution();

            // Debug.LogWarning($"Unsubscribing {gameObject.name}: << UNDO EXECUTION >>");
            passiveAbility.UndoExecution(gameObject, data);
        }

        private AbilityEventData PrepareData_ForPlacementExecution()
        {
            AbilityEventData data = new AbilityEventData(
                AbilityEventType.CardPlacedOnTile,
                gameObject,
                customData: new Dictionary<string, object>()
            );

            CardMovement cardMove = GetComponent<CardMovement>();

            data.CustomData["middleTile"] = cardMove.GetLogicalTile(); // {middleTile: MiddleTile}

            return data;
        }
    }
}