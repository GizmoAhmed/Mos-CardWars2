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
            return cardStats.soulUse <= thisCardOwnerPlayerStats.currentMagic;
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            Debug.Log($"Spell move CMDPlace override called. Casting {gameObject.name} on {tile.name}");
            
            // todo listener for spell being casted
            
            Tile tileScript = tile.GetComponent<Tile>();
        
            // SERVER: Store LOGICAL position (authoritative game state)
            logicalRow = tileScript.row;
            logicalColumn = tileScript.column;
            logicalPlayerSide = tileScript.playerSide;
        
            Debug.LogWarning($"Command on Server: Spell casted at logical position Row={logicalRow}, Col={logicalColumn}, Side={logicalPlayerSide}");
            
            RpcPlaceCardOnTile(tile);
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            // todo activate the ability
            /*Debug.LogWarning(isOwned
                ? $"Activating {gameObject.name} Spell on {tileObj.name}"
                : $"Activating {gameObject.name} Spell on {tileObj.GetComponent<Tile>().across.name}");*/

            Discard();
        }
    }
}