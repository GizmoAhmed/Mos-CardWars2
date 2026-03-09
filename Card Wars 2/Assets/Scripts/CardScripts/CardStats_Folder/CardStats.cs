using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using GameManagement;
using Mirror;
using UnityEngine;

namespace CardScripts.CardStats_Folder
{
    public class CardStats : NetworkBehaviour
    {
        public CardDataSO cardData;

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

        // called from command in CardMovement.cs
        public void RegisterPassiveAbility()
        {
            if (AbilityEventManager.AbilityManagerInstance == null)
            {
                Debug.LogError("AbilityEventManager not found in scene!");
                return;
            }

            if (cardData.ability == null)
            {
                Debug.LogWarning($"{gameObject.name} doesn't have an ability on it");
                return;
            }

            if (!cardData.ability.isPassive) // if not passive then we don't care
            {
                return;
            }

            AbilityEventType[] events = cardData.ability.triggeringEvents;
            
            foreach (AbilityEventType eventType in events)
            {
                // Create a callback for this event type
                void Callback(AbilityEventData eventData)
                {
                    // Only execute on server
                    if (!isServer) return;

                    // add execution to callback, this function is called when event manager broadcasts the type
                    cardData.ability.ExecuteAbility(gameObject, eventData);
                }

                AbilityEventManager.AbilityManagerInstance.Subscribe(eventType, Callback);
                
                Debug.Log($"{cardData.cardName} subscribed to {eventType} events");
            }
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