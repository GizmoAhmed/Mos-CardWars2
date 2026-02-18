using System;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Mirror;
using PlayerStuff;
using UnityEngine;

namespace CardScripts
{
    public class OnCardButtons : NetworkBehaviour
    {
        private CardMovement _move;

        private void Start()
        {
            _move = GetComponent<CardMovement>();
        }
        
        public void BurnButton()
        {
            if (_move == null)
            {
                Debug.LogError($"Move script on {gameObject.name} is null");
                return;
            }

            _move.thisCardOwnerPlayerStats.CmdBurn(gameObject);
        }

        public void CreatureAbility()
        {
            Debug.Log($"Creature Ability-ing...{gameObject.name}");
        }
    }
}
