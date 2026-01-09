using UnityEngine;

namespace CardScripts
{
    public class CardButtons : MonoBehaviour
    {
        public void Burn() 
        {
            Debug.Log($"Burning: {gameObject.name}");
        }

        public void CreatureAbility()
        {
            Debug.Log($"Creature Ability: {gameObject.name}");
        }
    }
}
