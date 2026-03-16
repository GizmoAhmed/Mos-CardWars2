using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using GameManagement;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardStats_Folder
{
    public class CardStats : NetworkBehaviour
    {
        public CardDataSO cardData;

        // For global events
        private Dictionary<AbilityEventType, System.Action<AbilityEventData>> _globalCallbacks = 
            new Dictionary<AbilityEventType, System.Action<AbilityEventData>>();
    
        // For tile events
        private Dictionary<AbilityEventType, System.Action<AbilityEventData>> _tileCallbacks = 
            new Dictionary<AbilityEventType, System.Action<AbilityEventData>>();
        
        private Tile _subscribedTile;
        
        public CardDataSO CardData => cardData;

        [SyncVar(hook = nameof(OnCardDataChanged))]
        private int _cardDataIndex = -1;

        private CardDisplay _display;

        [SyncVar(hook = nameof(UpdateMagic))] public int soulUse;

        [SyncVar(hook = nameof(UpdateBurnCost))]
        public int burnCost = 2;

        public void SetCardData(CardDataSO data)
        {
            cardData = data;
            GameManager gm = FindObjectOfType<GameManager>();
            _cardDataIndex = gm.masterDeck.IndexOf(data);
        }

        private void OnCardDataChanged(int oldIndex, int newIndex)
        {
            if (newIndex < 0) return;

            GameManager gm = FindObjectOfType<GameManager>();
            if (newIndex < gm.masterDeck.Count && cardData == null)
            {
                cardData = gm.masterDeck[newIndex];
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            InitializeCard();
        }

        public virtual void InitializeCard()
        {
            if (cardData == null)
            {
                Debug.LogError($"cardData on {gameObject.name} is null");
                return;
            }

            gameObject.name = cardData.cardName + "CardObj";

            _display = GetComponent<CardDisplay>();
            _display.InitDisplayWithData(this);

            // Check if this is a networked object or client-only
            if (!netIdentity || netIdentity.netId == 0)
            {
                // non-networked: set directly
                LocallyRefreshCardStats();
            }
            else
            {
                CmdRefreshCardStats();
            }

            // CmdRefreshCardStats(); 
            _display.UpdateUIMagic(soulUse);
            _display.UpdateUI_BurnCost(burnCost);
        }

        /// <summary>
        /// Applies the stats from the CardDataSO
        /// </summary>
        [Command]
        public virtual void CmdRefreshCardStats()
        {
            ApplyStatsFromData();
        }

        protected virtual void LocallyRefreshCardStats()
        {
            ApplyStatsFromData();
        }

        protected virtual void ApplyStatsFromData()
        {
            soulUse = cardData.magic;
            burnCost = cardData.burnCost;
        }

        // called from command in CardMovement.cs // todo a function that creature and spell will never use, yet it has the function
        public void RegisterPassiveAbility()
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance == null)
            {
                Debug.LogError("GlobalAbilityEventManager not found in scene!");
                return;
            }

            if (cardData.ability == null)
            {
                Debug.LogWarning($"{gameObject.name} doesn't have an ability on it");
                return;
            }

            if (cardData.ability is PassiveAbilitySO passiveAbility)
            {
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
                
                _globalCallbacks[eventType] = Callback; // cards owns this call back, once discarded, remove it from manager

                // this card says: hey event manager, let me know (subscription) when this event (event data)...
                // ...happens (happens ==> TriggerEvents_ForAllSubscribersOfType), so I can run my execution
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.Subscribe(eventType, Callback);
            
                Debug.Log($"{cardData.cardName} subscribed GLOBALLY to {eventType}");
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
                Debug.LogWarning($"{cardData.cardName} not on a tile yet - can't register tile listener");
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
        }

        [Command] 
        public void CmdChangeSoulUse(int amount, bool buff)
        {
            if (buff)
            {
                soulUse -= amount;
            }
            else
            {
                soulUse += amount;
            }
        }
        
        [Command] 
        public void CmdChangeBurnCost(int amount, bool buff)
        {
            if (buff)
            {
                burnCost -= amount;
            }
            else
            {
                burnCost += amount;
            }
        }

        public void UpdateMagic(int oldMagic, int newMagic)
        {
            _display.UpdateUIMagic(newMagic); // todo also change the players max soulUse
        }

        public void UpdateBurnCost(int oldCost, int newCost)
        {
            _display.UpdateUI_BurnCost(newCost);
        }
    }
}