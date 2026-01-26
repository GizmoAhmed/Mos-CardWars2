using CardScripts.CardStatss;
using CardScripts.CardStatss.Runes;
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

            if (!land.creature) return false; // there's no creature on this land? nope can't place here

            CreatureStats creature = land.creature.GetComponent<CreatureStats>();

            bool canBeRuned = land.tileOwner &&
                land.creature != null &&
                (creature.currentRune1 == null) || (creature.overRuneable && creature.currentRune2 == null);

            return land.tileOwner && land.creature != null &&
                   canBeRuned; // return, if the land has a creature on it and creature can be runed
        }

        [ClientRpc] // dw, already asked if valid placement above
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            if (isOwned)
            {
                // the creature on this land
                CreatureStats creature = tileObj.GetComponent<MiddleLand>().creature.GetComponent<CreatureStats>();

                Debug.LogWarning(
                    $"Binding {gameObject.name} to {creature.gameObject.name} on {tileObj.name}"); // activate ability, then...

                creature.BindRune(GetComponent<RuneBase>());
            }

            // ...then discard
            Discard();
        }
    }
}