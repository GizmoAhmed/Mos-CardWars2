using UnityEngine;

namespace CardScripts.CardData
{
    [CreateAssetMenu(fileName = "_____ Data", menuName = "New CardDataSO")]
    public class CardDataSO : ScriptableObject
    {
        [Header("General")]
        public string cardName;
        public Sprite mainImage;
        public int magic;
    
        [TextArea] public string abilityDescription;
        
        public int burnCost = 2;
    }
}

