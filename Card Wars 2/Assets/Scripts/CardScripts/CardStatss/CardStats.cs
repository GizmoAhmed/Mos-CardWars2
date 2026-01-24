using CardScripts.CardData;
using CardScripts.CardDisplays;
using Mirror;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.CardStatss
{
    public class CardStats : NetworkBehaviour
    {
        [SerializeField]
        protected CardDataSO cardData;

        public CardDataSO CardData => cardData;

        [SyncVar] public PlayerStats thisCardOwner;

        private CardDisplay _display;

        [SyncVar(hook = nameof(UpdateMagic))] public int magicUse;
        
        [SyncVar(hook = nameof(UpdateBurnCost))] public int burnCost = 2;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
    
            _display = GetComponent<CardDisplay>();
            _display.InitDisplay(this);
        
            RefreshCardStats();
        
            // these updates not called via hook (change in stat). 
            // that way, the stat can be zero if so desired from the CardDataSO
            _display.UpdateUIMagic(magicUse);

            _display.UpdateUI_BurnCost(burnCost);
        } 

        /// <summary>
        /// Applies the stats from the CardDataSO
        /// </summary>
        [Command]
        public virtual void RefreshCardStats()
        {
            magicUse = cardData.magic;
            burnCost = cardData.burnCost;
        }
    
        public void UpdateMagic(int oldMagic, int newMagic)
        {
            _display.UpdateUIMagic(newMagic); // todo also change the players max magicUse
        }

        public void UpdateBurnCost(int oldCost, int newCost)
        {
            _display.UpdateUI_BurnCost(newCost);
        }
    }
}
