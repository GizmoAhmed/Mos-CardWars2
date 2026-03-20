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

        // For tile events
        private Dictionary<AbilityEventType, System.Action<AbilityEventData>> _tileCallbacks =
            new Dictionary<AbilityEventType, System.Action<AbilityEventData>>();

        private Tile _subscribedTile;

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
                AbilityEventData data = PrepareDataForExecution();

                passiveAbility.ExecuteAbility(gameObject, data);
            }

            AbilityEventType[] events = passiveAbility.eventsThatTriggerThisAbility;

            // Register based on scope (global vs tile)
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

            // Get the tile this card is on
            CardMovement movement = GetComponent<CardMovement>();

            if (movement == null)
            {
                Debug.LogError("CardMovement not found - can't register tile listener!");
                return;
            }

            Tile tile = movement.GetLogicalTile();
            TileEventManager tileEventManager = tile.GetComponent<TileEventManager>();

            if (tile == null)
            {
                Debug.LogWarning($"{gameObject.name} not on a tile yet - can't register tile listener");
                return;
            }

            if (tileEventManager == null)
            {
                Debug.LogError($"This tile ({tile.gameObject.name}) doesn't have a tile listener)");
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

                _subscribedTile = tile;
            }
        }

        [Command]
        public void CmdUnsubscribeThisCardFromListening()
        {
            UnsubscribeThisCardFromListening();
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

            // Unsubscribe from tile events
            if (_subscribedTile != null)
            {
                TileEventManager tileEventManager = _subscribedTile.GetComponent<TileEventManager>();

                foreach (var kvp in _tileCallbacks)
                {
                    tileEventManager.UnsubscribeFromTileEvent(kvp.Key, kvp.Value);
                }
            }

            _globalCallbacks.Clear();
            _tileCallbacks.Clear();

            AbilityEventData data = PrepareDataForExecution();

            passiveAbility.UndoExecution(gameObject, data);
        }

        private AbilityEventData PrepareDataForExecution()
        {
            AbilityEventData data = new AbilityEventData(
                AbilityEventType.CardPlacedOnTile,
                gameObject,
                customData: new Dictionary<string, object>()
            );

            CardMovement cardMove = GetComponent<CardMovement>();

            data.CustomData["tile"] = cardMove.GetLogicalTile(); // {tile: Tile}

            return data;
        }
    }
}