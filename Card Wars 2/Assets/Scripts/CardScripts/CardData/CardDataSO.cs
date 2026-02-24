using UnityEngine;

namespace CardScripts.CardData
{
    [CreateAssetMenu(fileName = "RENAME:_____Data", menuName = "New CardDataSO")]
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
    }
}

