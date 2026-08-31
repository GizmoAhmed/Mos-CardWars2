using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStats_Folder;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class SpellMovement : CardMovement
    {
        /// <summary>
        /// This function has two of the base classes checks
        /// The base also has a third check (soul use) but since spells don't use that in this version...
        /// ...I just put the first two into this function
        ///
        /// Easier to look at this way imo
        /// </summary>
        /// <returns></returns>
        private bool SoulExcluding_SpellSpecificPlacementChecks()
        {
            if (cardState == CardState.Preview)
            {
                // Debug.LogWarning($"Preview Card {gameObject.name} is asking about valid placement on {tile.gameObject.name}");
                return false;
            }

            Player cardsPlayer = thisCardOwnerPlayerStats.GetComponent<Player>();

            // if not your turn, you can't place a card anywhere
            if (cardsPlayer != null &&
                cardsPlayer.myTurn == false)
            {
                Debug.LogWarning($"Not Player {cardsPlayer.name}'s turn");
                return false;
            }

            return true;
        }

        protected override bool ValidPlacement(Tile tile)
        {
            // Global checks
            /*if (!base.ValidPlacement(tile))
                return false;*/

            // global checks replaced with:
            if (!SoulExcluding_SpellSpecificPlacementChecks()) // if doesn't pass global checks, abort. else, continue
                return false;

            // Type check
            if (cardStats.cardData.ability is not CastAbilitySO castAbility)
            {
                Debug.LogError($"{cardStats.cardData.cardName} doesn't have a CastAbilitySO!");
                return false;
            }

            // Check side requirement
            if (!CheckSideRequirement(tile, castAbility.castSide))
            {
                Debug.LogWarning(
                    $"Spell ({gameObject.name}) can't be cast on this tile ({tile.gameObject.name}), since it's looking for this side ({castAbility.castSide})");
                return false;
            }

            // Get server tile for logical checks
            Tile serverTile = GetServerTileForClient(tile);

            // Check occupancy requirement
            if (!CheckCastRequirement(serverTile, castAbility.castRequirementType))
            {
                Debug.LogWarning($"{gameObject.name} cast invalid: requires {castAbility.castRequirementType}");
                return false;
            }

            // Check spell-specific conditions
            if (!castAbility.SpecificSpellPlacementConditions(serverTile))
            {
                Debug.LogWarning($"{gameObject.name} doesn't meet specific conditions. See above for details ↑");
                return false;
            }

            // Check magic cost
            /*if (cardStats.soulUse > thisCardOwnerPlayerStats.currentSoul)
            {
                Debug.Log("Not enough magic to cast spell");
                return false;
            }*/

            return true;
        }

        /// <summary>
        /// Check if the spell can be cast on this side
        /// </summary>
        private bool CheckSideRequirement(Tile tile, CastAbilitySO.CastSide side)
        {
            switch (side)
            {
                case CastAbilitySO.CastSide.Either:
                    return true;

                case CastAbilitySO.CastSide.Yours:
                    return tile.clientTileOwner;

                case CastAbilitySO.CastSide.Theirs:
                    return !tile.clientTileOwner;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if the tile meets the casting requirement
        /// </summary>
        private bool CheckCastRequirement(Tile tile, CastAbilitySO.CastRequirementType requirement)
        {
            // Anywhere is always valid
            if (requirement == CastAbilitySO.CastRequirementType.AnyTile)
                return true;

            // CharmTile specific
            if (tile is CharmTile charmTile)
            {
                return requirement == CastAbilitySO.CastRequirementType.OnCharm
                       && charmTile.charms.Count > 0;
            }

            // MiddleTile specific
            if (tile is MiddleTile midTile)
            {
                switch (requirement)
                {
                    case CastAbilitySO.CastRequirementType.AnyTileWithCard:
                    case CastAbilitySO.CastRequirementType.CreatureAndOrBuilding:
                        return midTile.logicalCreature != null || midTile.logicalBuilding != null;

                    case CastAbilitySO.CastRequirementType.OnCreature:
                        return midTile.logicalCreature != null;

                    case CastAbilitySO.CastRequirementType.OnBuilding:
                        return midTile.logicalBuilding != null;

                    default:
                        Debug.LogWarning($"Unknown cast requirement: {requirement}");
                        return false;
                }
            }

            Debug.LogError($"Unknown tile type: {tile.GetType().Name}");
            return false;
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // base.CmdPlaceCardOnTile(tile);

            Tile tileScript = tile.GetComponent<Tile>();

            Tile lTile = GetServerTileForClient(tileScript) as Tile;

            AbilityEventData spellData = new AbilityEventData(
                AbilityEventType.AnySpellCasted,
                lTile.gameObject); // pass Tile as cardToBeEffected, some spells will use it, some won't

            Debug.Log($"<color=purple>Cast</color> {gameObject.name} on {lTile.gameObject.name}");
            cardStats.cardData.ability.ExecuteAbility(gameObject, spellData); // use the spell...

            GlobalBroadcast_AnyCardPlacement(); // ...then tell everyone you used this spell

            
            SpellStats stats = GetComponent<SpellStats>();
            int usesLeft = stats.uses;

            if (usesLeft <= 0) // error check, ideally should never get here
            {
                // I know this is a good error check, because I should never have to comment this if you get what I'm saying
                Debug.LogError($"CmdPlace in SpellMovement ({gameObject.name}) attempted to decrement a spell that has no uses left, which should have never reached here.");    
            }
            else
            {
                // decrement uses, uses -= 1
                stats.Server_UseSpell_DecrementUses();

                if (stats.uses == 0) // spent, discard
                {
                    base.ServerDiscard();
                }
                else // still got more, move back to hand
                {
                    Target_SnapBack(connectionToClient);
                    // todo make the spell face up
                }
            }
        }

        /// <summary>
        /// Runs only on the client that cast the spell then send it back to hand
        /// TargetRPC because called from Command, which is server only, and I need snap back to run per the client who is dragging the card
        /// </summary>
        [TargetRpc]
        private void Target_SnapBack(NetworkConnectionToClient target)
        {
            StartCoroutine(SnapBackToHand());
        }

        // broadcast the cast, who knows, there might be a card that listens to this
        protected override void GlobalBroadcast_AnyCardPlacement()
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