using System;
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

        protected CardDisplay Display;

        [SyncVar(hook = nameof(Hook_UpdateSoulUI_OnBothClients))]
        public int soulUse;

        [SyncVar(hook = nameof(Hook_UpdateUIBurnCost))]
        public int burnCost;

        [SyncVar(hook = nameof(ToggleBurnability))]
        public bool canBeBurned = true;

        protected virtual void Awake()
        {
            Display = GetComponent<CardDisplay>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serverCall"></param>
        public virtual void SetCardData(CardDataSO data, bool serverCall)
        {
            MasterDeck masterDeckDb = FindObjectOfType<MasterDeck>();

            if (masterDeckDb == null)
            {
                Debug.LogError($"MasterDeckDb is Null on when data set on {gameObject.name}");
                return;
            }

            // set for self, server or otherwise
            cardData = data;
            
            // ...set the name
            gameObject.name = cardData.cardName + "CardObj";

            if (serverCall) // if called from server based func, set data on other guy 
            {
                // then set for the other guy
                Rpc_SetCardData(data.cardID); // name also set in here
            }

            Display = GetComponent<CardDisplay>();
            
            if (serverCall)
            {
                // since server call, will use hooks to propagate to both clients
                SetStats_FromData(); 
            }
            else
            {
                // since client, you'll have to manually call the ui update functions that are inside the hooks
                SetStats_FromData(); 
                Display.UpdateUISoul(soulUse);
                Display.UpdateUI_BurnCost(burnCost);
            }
            
            if (serverCall) // called to spawn card on both clients
            {
                Rpc_RenderCardUI(); // render ui for both clients since spawning
            }
            else
            {
                // since only client calling, render for them only
                Display.SetDisplayElements_UsingData(this);
            }

            if (serverCall) // on server only since abilities are server based anyway (I think that's why it works like this)
            {
                // ...register passive listener if it has one
                if (TryGetComponent<PassiveListenerCard>(out var listen))
                {
                    listen.InitializePassiveListener(this, cardData.ability as PassiveAbilitySO);
                }
            }
        }

        /// <summary>
        /// Set card data for both clients from above command
        /// I have to do it this way because I need a card data to be known for both host and join,
        /// but I can't use the trusty sync var, so I have to do it the old fashion way via client rpc
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
                Debug.LogError("MasterDeck not found on this client!");
                return;
            }

            CardDataSO data = masterDeckDb.GetCardByID(cardID);

            if (data == null)
            {
                Debug.LogError($"Card ({cardID}) not found on this client");
                return;
            }

            // set on the other guy 
            cardData = data;
            // ...set the name
            gameObject.name = cardData.cardName + "CardObj";
        }

        [ClientRpc]
        private void Rpc_RenderCardUI()
        {
            // render ui for both clients
            Display.SetDisplayElements_UsingData(this);
        }

        public virtual void SetStats_FromData()
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
            Display.UpdateUISoul(newSoul); // todo also change the players max soulUse
        }

        public void Hook_UpdateUIBurnCost(int oldCost, int newCost)
        {
            Display.UpdateUI_BurnCost(newCost);
        }

        public void ToggleBurnability(bool oldValue, bool newValue)
        {
        }
    }
}