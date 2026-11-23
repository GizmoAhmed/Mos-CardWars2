using UnityEngine;

namespace CardScripts.CardData
{
    [CreateAssetMenu(fileName = "-----_Creature", menuName = "New Creature")]
    public class CreatureDataSO : CardDataSO
    {
        [Header("Creature Specific Data")] 
        
        public int abilityCost;

        public int attack;
        public int defense;
        
        [Header("Element")]
        public Element element;
        public enum Element
        {
            Strength,
            Defense,
            Soul,
            Draw,
            Money,
            // None, don't have any reason for this, yet
            Any
        }
        
        public Sprite elementSprite;
    }
}