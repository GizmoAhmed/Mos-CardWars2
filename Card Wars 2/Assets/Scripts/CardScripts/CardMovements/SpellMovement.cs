using Lands;
using Mirror;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class SpellMovement : BaseMovement
    {
        protected override bool ValidPlacement(MiddleLand land)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(land))
                return false;

            // if have enough magic to use spell. notice the lack of tile checks todo well actually, prolly have to expand this for certain spells
            return cardStats.magicUse <= thisPlayersStats.currentMagic;
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            // todo activate the ability
            if (isOwned)
            {
                Debug.LogWarning($"Activating {gameObject.name} Spell on {tileObj.name}"); // activate ability, then...
            }
            else
            {
                Debug.LogWarning(
                    $"Activating {gameObject.name} Spell on {tileObj.GetComponent<MiddleLand>().across.name}");
            }

            Discard();
        }
    }
}