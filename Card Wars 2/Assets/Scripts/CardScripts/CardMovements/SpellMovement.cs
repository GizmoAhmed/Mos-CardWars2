using AbilityEvents;
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

            // Check if the ability is a CastAbilitySO (which it should) and get the cast requirement
            // todo also get the cast condition in here, ie if creature is of element, buff XYZ
            if (cardStats.cardData.ability is CastAbilitySO castAbility)
            {
                CastAbilitySO.CastRequirementType castType = castAbility.castRequirementType;

                // Free cast ignores side restrictions
                if (castType != CastAbilitySO.CastRequirementType.Free)
                {
                    // Check if side matches requirement
                    if (castAbility.yourSide != tile.tileOwner)
                    {
                        Debug.LogWarning($"Can't place {gameObject.name} on this side");
                        return false; // Wrong side!
                    }
                }

                // Check placement based on cast requirement
                bool meetsRequirement = CheckCastRequirement(tile, castType);

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
                    return tile.creatureVisual != null || tile.buildingVisual != null;

                case CastAbilitySO.CastRequirementType.OnCreature:
                    // Tile must have a creature
                    return tile.creatureVisual != null;

                case CastAbilitySO.CastRequirementType.OnBuilding:
                    // Tile must have a building
                    return tile.buildingVisual != null;

                case CastAbilitySO.CastRequirementType.OnCharm:
                    // This is a charm tile and has at least one charm
                    return tile is CharmTile charmTile && charmTile.InUseCharms.Count > 0;

                case CastAbilitySO.CastRequirementType.CreatureAndOrBuilding:
                    // Tile has creature and/or building (at least one)
                    return tile.creatureVisual != null || tile.buildingVisual != null;

                default:
                    Debug.LogWarning($"Unknown cast requirement: {requirement}");
                    return false;
            }
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // base.CmdPlaceCardOnTile(tile);
            
            AbilityEventData spellData = new AbilityEventData(
                AbilityEventType.AnySpellCasted,
                tile); // pass tile as cardToBeEffected, some spells will use it, some won't

            cardStats.cardData.ability.ExecuteAbility(gameObject, spellData); // use the spell...
            
            GlobalBroadcastCardPlacement(); // ...then tell everyone you used this spell
            
            base.RpcDiscard(); // discard on both clients via base call
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