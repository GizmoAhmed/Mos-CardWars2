using CardScripts.Abilities;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class SpellMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile tile)
        {
            // If can't pass global checks, abort
            if (!base.ValidPlacement(tile))
                return false;

            // Check if the ability is a CastAbilitySO and get the cast requirement
            if (cardStats.cardData.ability is CastAbilitySO castAbility)
            {
                CastAbilitySO.CastRequirementType castType = castAbility.castRequirementType;

                // Free cast ignores side restrictions
                if (castType != CastAbilitySO.CastRequirementType.Free)
                {
                    // Check if side matches requirement
                    if (castAbility.yourSide != tile.tileOwner)
                    {
                        return false; // Wrong side!
                    }
                }

                // Check placement based on cast requirement
                bool meetsRequirement = CheckCastRequirement(tile, castType);

                if (!meetsRequirement)
                {
                    Debug.Log($"Spell placement invalid: requires {castType}");
                    return false;
                }

                // Check if player has enough magic
                return cardStats.soulUse <= thisCardOwnerPlayerStats.currentMagic;
            }


            Debug.LogError($"{cardStats.cardData.cardName} doesn't have a CastAbilitySO!");
            return false;
        }

        /// <summary>
        /// Check if the tile meets the casting requirement
        /// </summary>
        private bool CheckCastRequirement(Tile tile, CastAbilitySO.CastRequirementType requirement)
        {
            switch (requirement)
            {
                case CastAbilitySO.CastRequirementType.Free:
                    // Can cast anywhere
                    return true;

                case CastAbilitySO.CastRequirementType.AnywhereOccupied:
                    // Tile must have something on it (creature or building)
                    return tile.creature != null || tile.building != null;

                case CastAbilitySO.CastRequirementType.OnCreature:
                    // Tile must have a creature
                    return tile.creature != null;

                case CastAbilitySO.CastRequirementType.OnBuilding:
                    // Tile must have a building
                    return tile.building != null;

                case CastAbilitySO.CastRequirementType.OnCharm:
                    // This is a charm tile and has at least one charm
                    return tile is CharmTile charmTile && charmTile.InUseCharms.Count > 0;
                
                case CastAbilitySO.CastRequirementType.CreatureAndOrBuilding:
                    // Tile has creature and/or building (at least one)
                    return tile.creature != null || tile.building != null;

                default:
                    Debug.LogWarning($"Unknown cast requirement: {requirement}");
                    return false;
            }
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            base.CmdPlaceCardOnTile(
                tile); // todo see in base func that spell cast counts as card placement, due to change

            Debug.Log($"Spell move CMDPlace override called. Casting {gameObject.name} on {tile.name}");

            // todo listener for spell being casted

            Debug.LogWarning(
                $"Command on Server: Spell casted at logical position Row={logicalRow}, Col={logicalColumn}, Side={logicalPlayerSide}");
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            // todo activate the ability
            /*Debug.LogWarning(isOwned
                ? $"Activating {gameObject.name} Spell on {tileObj.name}"
                : $"Activating {gameObject.name} Spell on {tileObj.GetComponent<Tile>().across.name}");*/

            Debug.Log($"Client RPC: {gameObject.name} runs RpcPlaceCardOnTile(), discarding {gameObject.name}...");
            Discard();
        }
    }
}