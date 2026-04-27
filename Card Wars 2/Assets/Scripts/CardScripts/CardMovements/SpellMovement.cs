using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.Abilities.AbilityClasses;
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

            // Check if the ability is a CastAbilitySO (which it should) and get the cast requirement
            // todo also get the cast condition in here, ie if creature is of element, buff XYZ
            if (cardStats.cardData.ability is CastAbilitySO castAbility)
            {
                CastAbilitySO.CastRequirementType castType = castAbility.castRequirementType;

                // Free cast ignores side restrictions
                if (castType != CastAbilitySO.CastRequirementType.Free)
                {
                    // Check if side matches requirement
                    if (castAbility.yourSide != tile.clientTileOwner)
                    {
                        Debug.LogWarning($"Can't place {gameObject.name} on this side");
                        return false; // Wrong side!
                    }
                }
                
                Tile logTile = GetServerTileForClient(tile);

                // Check placement based on cast requirement
                bool meetsRequirement = CheckCastRequirement(logTile, castType);

                if (!meetsRequirement)
                {
                    Debug.LogWarning($"{gameObject.name} cast invalid: requires {castType}");
                    return false;
                }
                
                // check more specific spell condition
                bool specificReqsMet = castAbility.SpecificSpellPlacementConditions(tile);
                if (!specificReqsMet)
                {
                    Debug.LogWarning($"{gameObject.name} wasn't cast under under it's more specific conditions, see castAbility.SpecificSpellPlacementConditions();");
                    return false;
                }

                // Check if player has enough magic
                return cardStats.soulUse <= thisCardOwnerPlayerStats.currentMagic;
            }


            Debug.LogError($"{cardStats.cardData.cardName} doesn't have a CastAbilitySO!");
            return false;
        }

        /// <summary>
        /// Check if the Tile meets the casting requirement
        /// </summary>
        private bool CheckCastRequirement(Tile tile, CastAbilitySO.CastRequirementType requirement)
        {
            if (requirement == CastAbilitySO.CastRequirementType.Free)
            {
                return true;
            }
            
            if (tile is MiddleTile midTile)
            {
                switch (requirement)
                {
                    case CastAbilitySO.CastRequirementType.AnywhereOccupied:
                        // Tile must have something on it (creature or building)
                        return midTile.logicalCreature != null || midTile.logicalBuilding != null;

                    case CastAbilitySO.CastRequirementType.OnCreature:
                        // Tile must have a creature
                        return midTile.logicalCreature != null;

                    case CastAbilitySO.CastRequirementType.OnBuilding:
                        // Tile must have a building
                        return midTile.logicalBuilding != null;
                    case CastAbilitySO.CastRequirementType.CreatureAndOrBuilding:
                        // Tile has creature and/or building (at least one)
                        return midTile.logicalCreature != null || midTile.logicalBuilding != null;
                    default:
                        Debug.LogWarning($"Unknown cast requirement: {requirement}");
                        return false;
                }
            }
            
            if (tile is CharmTile charmTile)
            {
                return charmTile.charms.Count > 0;
            }
            
            Debug.LogError($"Unknown Tile type in CheckCastRequirement() for {gameObject.name}");
            return false;
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // base.CmdPlaceCardOnTile(Tile);
            
            Tile tileScript = tile.GetComponent<Tile>();

            // set these rq so we use GetLogical tile to get the correct tile on the server
            logicalRow = logicalPlayerSide; // tileScript.row; 
            logicalColumn = tileScript.column;

            GameObject logTile = GetLogicalTile().gameObject;
            
            AbilityEventData spellData = new AbilityEventData(
                AbilityEventType.AnySpellCasted,
                logTile); // pass Tile as cardToBeEffected, some spells will use it, some won't

            cardStats.cardData.ability.ExecuteAbility(gameObject, spellData); // use the spell...
            
            GlobalBroadcastCardPlacement(); // ...then tell everyone you used this spell
            
            base.ServerDiscard(); // discard on both clients via base call
        }

        // broadcast the cast, who knows, there might be a card that listens to this
        protected override void GlobalBroadcastCardPlacement()
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData castData = new AbilityEventData(
                    AbilityEventType.AnySpellCasted,
                    gameObject
                );
                // tell event manager to tell everyone (that cares) that this rune was binded
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.TriggerEvents_ForAllSubscribersOfType(castData);
            }
            else
            {
                Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
            }
        }
    }
}