using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using Tiles;
using UnityEngine;

namespace Extensions
{
    public static class CardMethodExtensions
    {
        /// <summary>
        /// Extension method: Given a card game object, returns it's logical server tile
        /// </summary>
        /// <param name="card">Card game object</param>
        /// <returns>Tile the parametrized card is attached to</returns>
        public static Tile GetTile_Ext(this GameObject card)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("GetTile_Ext called on client - ignoring!");
                return null;
            }
            
            CardMovement cardMovement = card.GetComponent<CardMovement>();

            Tile thisTile = cardMovement.GetLogicalTile();

            if (thisTile == null)
            {
                Debug.LogError(card.name + " has no Tile attached");
            }
        
            return thisTile;
        }

        public static void DamageTileAcross_Ext(this MiddleTile tile, int damage)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("<color=orange>DamageTileAcross_Ext</color> called on client - ignoring!");
                return;
            }
            
            MiddleTile across = TileManager.Instance.GetAcrossTile(tile.row, 
                    tile.column, 
                    tile.serverPlayerSide) 
                as MiddleTile;
            
            if (across.logicalCreature != null) // creature over there
            {
                CreatureStats oppCreature =  across.logicalCreature.GetComponent<CreatureStats>();
            
                // deal damage
                oppCreature.ChangeCreatureDefense(damage, buff:false);
            }
            else // empty lane 
            {
                // todo do something, for now, just add player drain or lost money or sum
            }
        }
    }
}
