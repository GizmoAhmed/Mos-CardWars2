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