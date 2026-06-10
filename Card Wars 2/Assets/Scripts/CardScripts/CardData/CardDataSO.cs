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
        public int magic;
    
        [TextArea] 
        public string abilityDescription;
        
        public int burnCost = 2;

        public CardAbilitySO ability;
    }
}