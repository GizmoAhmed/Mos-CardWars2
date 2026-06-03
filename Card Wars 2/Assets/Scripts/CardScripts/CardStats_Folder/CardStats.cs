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

        private CardDisplay _display;

        [SyncVar(hook = nameof(Hook_UpdateSoulUI_OnBothClients))]
        public int soulUse;

        [SyncVar(hook = nameof(UpdateBurnCost))]
        public int burnCost = 2;

        [SyncVar(hook = nameof(ToggleBurnability))]
        public bool canBeBurned = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="networked"></param>
        public void SetCardData(CardDataSO data, bool networked)
        {
            MasterDeck masterDeckDb = FindObjectOfType<MasterDeck>();

            if (masterDeckDb == null)
            {
                Debug.LogError($"MasterDeckDb is Null on when data set on {gameObject.name}");
                return;
            }

            cardData = data;

            if (networked) // called from server based func
            {
                // tell other guy about it to
                Rpc_SetCardData(data.cardID); 
            }
            
            // todo now do what Initialize card did, here
            
            // _cardDataIndex = masterDeckDb.masterDeckList.IndexOf(data);
        }

        /// <summary>
        /// Set card data for both clients from above command
        /// I have to do it this way because I need a card data to be known for both host and join,
        /// but I can't use the trust sync var so I have to do it the old fashion way
        /// </summary>
        /// <param name="data"></param>
        [ClientRpc]
        private void Rpc_SetCardData(string cardID)
        {
            // Host already set it in SetCardData above
            if (isServer) return;

            MasterDeck masterDeckDb = FindObjectOfType<MasterDeck>();

            if (masterDeckDb == null)
            {
                Debug.LogError($"MasterDeck not found on client!");
                return;
            }

            CardDataSO data = masterDeckDb.GetCardByID(cardID);

            if (data == null)
            {
                Debug.LogError($"Card not found on client: {cardID}");
                return;
            }

            cardData = data;
        }

        /// <summary>
        /// Sets all the stats and display information of a card
        /// </summary>
        public virtual void InitializeCard()
        {
            if (cardData == null)
            {
                Debug.LogError($"cardData on {gameObject.name} is null");
                return;
            }

            gameObject.name = cardData.cardName + "CardObj";

            _display = GetComponent<CardDisplay>();
            _display.SetDisplayElements_UsingData(this);

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

                // on the server, these two variables would be set via hook
                // since this if block says there is no server (Offer cards client side)...
                // ... you have to set the display manually for this client ↓
            }
            else
            {
                CmdRefreshCardStats();
            }

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