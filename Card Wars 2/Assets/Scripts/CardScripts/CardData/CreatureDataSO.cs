using UnityEngine;

namespace CardScripts.CardData
{
    [CreateAssetMenu(fileName = "-----_Creature", menuName = "New Creature")]
    public class CreatureDataSO : CardDataSO
    {
        [Header("Creature Specific Data")]
        public Element element;
        public enum Element
        {
            Strength,
            Defense,
            Soul,
            Draw,
            Money,
            None,
            All
        }

        public Sprite elementSprite;
        
        public int abilityCost; 
        
        public int attack, defense;
    }
}