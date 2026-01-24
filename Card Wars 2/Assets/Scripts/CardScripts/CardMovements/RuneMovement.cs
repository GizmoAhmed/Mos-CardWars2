using Lands;
using Mirror;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class RuneMovement : BaseMovement
    {
        protected override bool ValidPlacement(MiddleLand land)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(land))
                return false;

            // todo, if over-runed?
            return land.creature != null; // return, if the land has a creature on it
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            // todo bind/apply the rune...
            if (isOwned)
            {
                Debug.LogWarning($"Activating {gameObject.name} Rune on {tileObj.name}"); // activate ability, then...
            }
            else
            {
                Debug.LogWarning(
                    $"Activating {gameObject.name} Rune on {tileObj.GetComponent<MiddleLand>().across.name}");
            }
            
            // ...then discard
            Discard();
        }
    }
}