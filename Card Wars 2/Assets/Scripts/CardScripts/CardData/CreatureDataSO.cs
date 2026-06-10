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
            Forge,
            Crystal,
            Haunted,
            Occult,
            Spirit,
            None
        }

        public Sprite elementSprite;
        
        public int abilityCost; 
        
        public int attack, defense;
    }
}