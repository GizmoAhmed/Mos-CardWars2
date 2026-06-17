using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using PlayerStuff;
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

        /// <summary>
        /// Get the player that owns this card
        /// </summary>
        /// <param name="card"></param>
        /// <returns>player that owns this card</returns>
        public static PlayerStats GetOwningPlayerStats_Ext(this GameObject card)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("<color=orange>GetOwningPlayerStats_Ext</color> called on client - ignoring!");
                return null;
            }
            
            return card.GetComponent<CardMovement>().thisCardOwnerPlayerStats;
        }

        /// <summary>
        /// Get card tracker from the player that owns this card
        /// </summary>
        /// <param name="card"></param>
        /// <returns>Card tracker from the player that owns this card</returns>
        public static PlayerCardTracker GetOwningCardTracker_Ext(this GameObject card)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("<color=orange>GetOwningCardTracker_Ext</color> called on client - ignoring!");
                return null;
            }
            
            return card.GetOwningPlayerStats_Ext().GetComponent<PlayerCardTracker>();
        }
        
        /// <summary>
        /// Get the opponent of the player that owns this card
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Opponent of the player that owns this card</returns>
        public static PlayerStats GetOpponent_Ext(this PlayerStats player)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("<color=orange>GetOpponent_Ext</color> called on client - ignoring!");
                return null;
            }
            
            GameManager gm = GameObject.FindObjectOfType<GameManager>();
        
            PlayerStats p1 = gm.Player1.identity.GetComponent<PlayerStats>();
            PlayerStats p2 = gm.Player2.identity.GetComponent<PlayerStats>();
        
            return player == p1 ? p2 : p1;
        }

        public static PlayerCardTracker GetOpponentCardTracker_Ext(this PlayerStats player)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("<color=orange>GetOpponentCardTracker_Ext</color> called on client - ignoring!");
                return null;
            }
            
            return player.GetOpponent_Ext().GetComponent<PlayerCardTracker>();
        }

        public static bool IsCardOwnedByPlayer(this GameObject card, PlayerStats player)
        {
            PlayerStats owningPlayer = card.GetOwningPlayerStats_Ext();

            if (owningPlayer == player)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
