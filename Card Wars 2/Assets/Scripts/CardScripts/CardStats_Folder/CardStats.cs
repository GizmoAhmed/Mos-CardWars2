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
        
        protected CardAnimator anim;
        
        protected CardMovement Movement;

        private MasterDeck _masterDeck;

        [SyncVar(hook = nameof(Hook_UpdateSoulUI_OnBothClients))]
        public int soulUse;

        [SyncVar(hook = nameof(Hook_UpdateUIBurnCost))]
        public int burnCost;

        [SyncVar(hook = nameof(Hook_UpdateUIBurnReturn))]
        public int burnInHandReturn;

        [SyncVar(hook = nameof(Hook_ToggleBurnability))]
        public bool canBeBurned = true;

        [SyncVar]
        public bool canBeHandSold = true;

        [SyncVar(hook=nameof(Hook_UpdateLuck))] 
        public int cardBonusLuck = -1;
        
        public enum AnimTarget
        {
            Strength,
            Defense,
            Soul,
            AbilityCost,
            BurnCost,
            Uses
        }

        protected virtual void Awake()
        {
            Display = GetComponent<CardDisplay>();
            anim = GetComponent<CardAnimator>();
            Movement = GetComponent<CardMovement>();
            _masterDeck = FindObjectOfType<MasterDeck>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serverCall"></param>
        public virtual void SetAndApplyCardData(CardDataSO data, bool serverCall)
        {
            if (_masterDeck == null)
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
            
            if (serverCall) // drawing from server
            {
                // since server call, will use hooks to propagate to both clients
                SetStats_FromData(); 
            }
            else // set card data called client side, so draw modal
            {
                // since client, you'll have to manually call the ui update functions that are inside the hooks
                SetStats_FromData(); 
                Display.UpdateUI_Soul(soulUse);
                
                Display.UpdateUI_BurnReturn(burnInHandReturn); // set both return... 
                Display.UpdateUI_BurnCost(burnCost); // ...and cost
                Display.UpdateUI_Luck(cardBonusLuck);
                
                // Display.ToggleBurnCost(false); // turn off burn cost since init-ing
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
            soulUse = cardData.soul;
            burnCost = cardData.burnCost;
            burnInHandReturn = _masterDeck.GetBurnReturn(cardData); // get burn return for this rarity
            cardBonusLuck = 0;
        }
        
        [ClientRpc]
        protected void RpcPopTextAnimate(AnimTarget target, bool isBuff)
        {
            Display?.PopTextAnimate(target, isBuff);
        }
        
        protected void CallAnim_ToFloatStatText(int amount, bool buff, Color color)
        {
            string textToFloat = amount.ToString();
            if (buff) // positive
            {
                textToFloat = "+ " + textToFloat;
            }
            else // nerf? then negative
            {
                textToFloat = "- " + textToFloat;
            }

            anim.RpcFloatText(textToFloat, color);
        }
        
        [ClientRpc]
        protected void RpcDrawLineToTarget(Transform from)
        {
            anim.ShowTargetLine(from: from, to: transform);
        }

        /// <summary>
        /// Call to change a cards soul when it's on the field
        /// Updates the player stat 'current soul use' whenever a field card has their soul changed
        /// </summary>
        /// <param name="diff">pass negative if reducing soul of this card, positive if increasing</param>
        public void UpdateSyncSoulToPlayer(int diff, Transform source = null)
        {
            // one is += and the other is -= lol it's confusing
            // cards become more expensive in soul when increased
            // current soul for the player on the other hand is how much is used, some reducing that actually means adding
            soulUse += diff;

            // only change players stat for field cards
            if (TryGetComponent(out CardMovement move) &&
                move.cardState == CardMovement.CardState.Field)
            {
                move.thisCardOwnerPlayerStats.AdjustPlayerCurrentSoul_ViaPlacement(usingSoul: true, amount: diff);
                // move.thisCardOwnerPlayerStats.currentSoul -= diff;
            }
            
            RpcPopTextAnimate(AnimTarget.Soul, true); // pop soul text 
            
            bool buff = diff >= 0 ? true : false;
            CallAnim_ToFloatStatText(Math.Abs(diff), buff,new Color32(87, 169, 108, 255));
            
            // absolute here since diff can be negative, which is not the same for strength and defense
            
            if (source != null) // source was passed, use it to draw line
            {
                RpcDrawLineToTarget(from: source);
            }
            
            anim.Server_JiggleCard();
        }

        [Server]
        public void UpdateBurnCost(int amount, bool increase, Transform source = null)
        {
            if (increase)
            {
                burnCost += amount;
            }
            else
            {
                burnCost -= amount;
            }
            
            if (source != null) // source was passed, use it to draw line
            {
                RpcDrawLineToTarget(from: source);
            }
            
            anim.Server_JiggleCard();
        }

        [Server]
        public void UpdateBonusLuck(int amount, bool buff)
        {
            if (buff)
            {
                cardBonusLuck += amount;
            }
            else
            {
                cardBonusLuck -= amount;
            }
            
            anim.Server_JiggleCard();
        }

        public void Hook_UpdateSoulUI_OnBothClients(int oldMagic, int newSoul)
        {
            Display.UpdateUI_Soul(newSoul); 
        }

        public void Hook_UpdateUIBurnCost(int oldCost, int newCost)
        {
            Display.UpdateUI_BurnCost(newCost);
        }
        
        public void Hook_UpdateUIBurnReturn(int oldCost, int newReturn)
        {
            Display.UpdateUI_BurnReturn(newReturn);
        }

        public void Hook_ToggleBurnability(bool oldValue, bool newValue)
        {
            Display.UpdateUI_BurnButtonActivity(newValue);
        }

        public void Hook_UpdateLuck(int oldValue, int newValue)
        {
            Display.UpdateUI_Luck(newValue);
        }
    }
}