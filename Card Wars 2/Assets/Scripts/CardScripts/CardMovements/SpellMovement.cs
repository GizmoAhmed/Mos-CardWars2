using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class SpellMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile tile)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(tile))
                return false;
            
            // if have enough magic to use spell. notice the lack of tile checks todo well actually, prolly have to expand this for certain spells
            return cardStats.magicUse <= thisCardOwnerPlayerStats.currentMagic;
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
                    $"Activating {gameObject.name} Spell on {tileObj.GetComponent<Tile>().across.name}");
            }

            Discard();
        }
    }
}