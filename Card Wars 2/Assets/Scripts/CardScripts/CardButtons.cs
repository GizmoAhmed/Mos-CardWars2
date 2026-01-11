using System;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    public class CardButtons : NetworkBehaviour
    {
        private CardStats cardStats;
        private CardMovement cardMove;
        private bool burned;

        private void Start()
        {
            cardStats = GetComponent<CardStats>();
            cardMove = GetComponent<CardMovement>();
            
            burned = false;
        }

        
        public void BurnButton() 
        {
            
        }

        public void CreatureAbility()
        {
            Debug.Log($"Creature Ability-ing...{gameObject.name}");
        }
    }
}
