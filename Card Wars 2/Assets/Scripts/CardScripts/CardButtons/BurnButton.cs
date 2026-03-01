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
            _move.thisCardOwnerPlayerStats.CmdBurn(gameObject);
        }
    }
}
