using System;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    public class CardButtons : NetworkBehaviour
    {
        private CardStats cardStats;

        private void Start()
        {
            cardStats = GetComponent<CardStats>();
        }

        
        public void BurnButton()
        {
            Debug.Log($"Player {cardStats.thisCardOwner.gameObject.name} is burning {gameObject.name}...");
            cardStats.thisCardOwner.CmdBurn(gameObject);
        }

        public void CreatureAbility()
        {
            Debug.Log($"Creature Ability-ing...{gameObject.name}");
        }
    }
}
