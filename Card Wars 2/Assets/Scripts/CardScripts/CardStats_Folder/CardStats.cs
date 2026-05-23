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
        
        public CardDataSO CardData => cardData;

        [SyncVar(hook = nameof(OnCardDataChanged))]
        private int _cardDataIndex = -1;

        private CardDisplay _display;

        [SyncVar(hook = nameof(Hook_UpdateSoulUI_OnBothClients))] public int soulUse;

        [SyncVar(hook = nameof(UpdateBurnCost))]
        public int burnCost = 2;
        
        [SyncVar(hook = nameof(ToggleBurnability))] public bool canBeBurned = true;

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
            
            PassiveListenerCard passiveListener = GetComponent<PassiveListenerCard>();

            if (passiveListener != null)
            {
                passiveListener.InitializePassiveListener(this, cardData.ability as PassiveAbilitySO);
            }

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
            _display.UpdateUISoul(soulUse);
            _display.UpdateUI_BurnCost(burnCost);
        }

        /// <summary>
        /// Applies the stats from the CardDataSO
        /// </summary>
        [Command]
        protected virtual void CmdRefreshCardStats()
        {
            ApplyStatsFromData();
        }

        protected virtual void LocallyRefreshCardStats()
        {
            ApplyStatsFromData();
        }

        public virtual void ApplyStatsFromData()
        {
            soulUse = cardData.magic;
            burnCost = cardData.burnCost;
        }

        /// <summary>
        /// Call to change a cards soul when it's on the field
        /// Updates the player stat 'current soul use' whenever a field card has their soul changed
        /// </summary>
        /// <param name="diff">pass negative if reducing soul of this card, positive if increasing</param>
        public void UpdateSyncSoulToPlayer(int diff) 
        {
            // one is += and the other is -= lol it's confusing
            // cards become more expensive in soul when increased
            // current soul for the player on the other hand is how much is used, some reducing that actually means adding
            soulUse += diff;

            // only change players stat for field cards
            if (TryGetComponent(out CardMovement move) &&
                move.cardState == CardMovement.CardState.Field)
            {
                move.thisCardOwnerPlayerStats.currentSoul -= diff;
            }
        }

        public void Hook_UpdateSoulUI_OnBothClients(int oldMagic, int newSoul)
        {
            _display.UpdateUISoul(newSoul); // todo also change the players max soulUse
        }

        public void UpdateBurnCost(int oldCost, int newCost)
        {
            _display.UpdateUI_BurnCost(newCost);
        }
        
        public void ToggleBurnability(bool oldValue, bool newValue)
        {
            
        }
    }
}