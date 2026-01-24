using UnityEngine;

namespace CardScripts.CardData
{
    [CreateAssetMenu(fileName = "_____ Data", menuName = "New Creature")]
    public class CreatureDataSO : CardDataSO
    {
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