using System;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Mirror;
using PlayerStuff;
using UnityEngine;

namespace CardScripts
{
    public class CardButtons : NetworkBehaviour
    {
        private BaseMovement _move;

        private void Start()
        {
            _move = GetComponent<BaseMovement>();
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
