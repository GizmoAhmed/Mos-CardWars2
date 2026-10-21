using CardScripts.CardMovements;
using Mirror;
using UnityEngine;

namespace CardScripts.CardButtons
{
    public class BurnButton : NetworkBehaviour
    {
        private CardMovement _move;

        private void Start()
        {
            _move = GetComponent<CardMovement>();
        }
        
        public void OnClickBurn()
        {
            if (_move == null)
            {
                Debug.LogError($"Move script on {gameObject.name} is null");
                return;
            }

            if (_move.thisCardOwnerPlayerStats == null)
            {
                Debug.LogWarning($"Move script on {gameObject.name} does not have it's owning player set at time of burn");
                return;
            }

            _move.thisCardOwnerPlayerStats.CmdBurn(gameObject);
        }
    }
}
