using System;
using UnityEngine;

namespace CardScripts
{
    public class CardButtons : MonoBehaviour
    {
        private CardStats cardStats;
        private CardMovement cardMove;

        private void Start()
        {
            cardStats = GetComponent<CardStats>();
            cardMove = GetComponent<CardMovement>();
        }

        public void Burn() 
        {
            Debug.Log($"Burning... {gameObject.name}");

            if (cardStats.thisCardOwner.money >= cardStats.burnCost) // enough money?
            {
                cardMove.Discard();
                cardStats.thisCardOwner.money -= cardStats.burnCost;
                Debug.Log($"Burned: {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"Not enough money to burn {gameObject.name}");
            }
        }

        public void CreatureAbility()
        {
            Debug.Log($"Creature Ability-ing...{gameObject.name}");
        }
    }
}
