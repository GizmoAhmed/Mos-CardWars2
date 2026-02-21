using CardScripts.CardData;
using CardScripts.CardDisplays;
using GameManagement;
using Mirror;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.CardStatss
{
    public class CardStats : NetworkBehaviour
    {
        public CardDataSO cardData;

        public CardDataSO CardData => cardData;
        
        [SyncVar(hook = nameof(OnCardDataChanged))]
        private int _cardDataIndex = -1;
        
        private CardDisplay _display;

        [SyncVar(hook = nameof(UpdateMagic))] public int magicUse;
        
        [SyncVar(hook = nameof(UpdateBurnCost))] public int burnCost = 2;

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
        
            RefreshCardStats(); 
            
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
