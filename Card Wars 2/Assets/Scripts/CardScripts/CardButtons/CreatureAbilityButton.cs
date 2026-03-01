using CardScripts.CardMovements;
using Mirror;
using UnityEngine;

namespace CardScripts.CardButtons
{
    public class CreatureAbilityButton : NetworkBehaviour
    {
        private CardMovement _move;
        
        // Start is called before the first frame update
        void Start()
        {
            _move = GetComponent<CardMovement>();
        }

        public void OnAbilityClick() // button in editor
        {
            if (_move == null) 
            {
                Debug.LogError("Ability Click Error: card move & stats are null");
                return;
            }

            // only field cards can use abilities
            if (_move.cardState == CardMovement.CardState.Field)
            {
                _move.thisCardOwnerPlayerStats.CmdActivateCreatureAbility(gameObject);
            }
        }
    }
}
