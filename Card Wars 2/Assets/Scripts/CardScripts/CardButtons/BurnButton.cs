using CardScripts.CardMovements;
using Mirror;
using UnityEngine;

namespace CardScripts.CardButtons
{
    public class BurnButton : NetworkBehaviour
    {
        private CardMovement _move;

        private bool CanBeBurned(out string reason)
        {
            reason = null;
            
            if (_move == null) { reason = "_move not set"; return false; }
            if (_move.thisCardOwnerPlayerStats == null) { reason = "_move.thisCardOwnerPlayerStats is null"; return false; }
            if (_move.cardState == CardMovement.CardState.Discard) { reason = "discarded"; return false; }
            if (_move.cardState == CardMovement.CardState.Preview) { reason = "being previewed"; return false; }

            return true;
        }

        private void Start()
        {
            _move = GetComponent<CardMovement>();
        }
        
        public void OnClickBurn()
        {
            if (!CanBeBurned(out string reason))
            {
                Debug.Log($"<color=orange>Burn denied</color> on {gameObject.name}: {reason}");
                return;
            }
            
            _move.thisCardOwnerPlayerStats.CmdBurn(gameObject);
        }
    }
}
