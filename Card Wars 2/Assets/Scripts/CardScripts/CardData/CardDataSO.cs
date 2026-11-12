using CardScripts.Abilities;
using UnityEngine;

namespace CardScripts.CardData
{
    public class CardDataSO : ScriptableObject
    {
        public string cardID => cardName.ToLower().Replace(" ", "_");

        [Header("General Card Data")]
        public string cardName;
        public Sprite mainImage;
        public int soul;
        
        public int burnCost = 2;

        public int burnReturn = 3;

        public CardAbilitySO ability;

        public enum Rarity
        {
            Common = 0, 
            Uncommon = 1,
            Rare = 2,
            Epic = 3,
        }
        
        public Rarity cardRarity = Rarity.Common;
    }
}