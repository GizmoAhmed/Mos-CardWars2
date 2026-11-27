using System.Collections.Generic;
using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities
{
    /// <summary>
    /// For creatures with abilities that cost money to use
    /// </summary>
    public abstract class ActiveAbilitySO : CardAbilitySO
    {
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);

        /// <summary>
        /// Run this before abilities to see if the ability can actuall be used
        /// </summary>
        /// <param name="thisCard"></param>
        /// <returns></returns>
        public virtual bool ValidateAbility(
            GameObject thisCard)
        {
            CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();
            CreatureMovement creatureMovement = thisCard.GetComponent<CreatureMovement>();
            CardAnimator  cardAnimator = thisCard.GetComponent<CardAnimator>();
            
            if (creatureMovement.cardState == CardMovement.CardState.Discard)
                return false; // killed itself, abort
            
            Player player = thisCard.Ext_GetOwningPlayerStats().GetComponent<Player>();

            if (!player.myTurn) // if not your turn, can't floop anyone
            {
                cardAnimator.RpcFloatText("Not Your Turn!", Color.red);
                return false;
            }
            
            if (creatureStats.floopBlockers > 0)// can't floop
            {
                cardAnimator.RpcFloatText("Activation Blocked", Color.red);
                return false;
            } 
            
            return true;      
        }

        protected void FloatCustomText_FromValidationReject(GameObject thisCard, string message)
        {
            // todo anim for rejected ability on thisCard
            
            CardAnimator anim = thisCard.GetComponent<CardAnimator>();
            anim.RpcFloatText(message.ToUpper(), Color.red);
        }

        /// <summary>
        /// Validates whether or not can use ability that buffs spells by checking if there is a spell to buff and if they have uses to begin with
        /// </summary>
        /// <param name="thisCard"></param>
        /// <returns></returns>
        protected bool ValidateRandomSpellUsesUpdate(GameObject thisCard)
        {
            SpellStats randomSpell = GetRandomSpellInHand(thisCard);

            if (randomSpell == null)
            {
                FloatCustomText_FromValidationReject(thisCard, "No Spells to buff");
                return false;
            }

            int currentUses = randomSpell.uses;

            if (currentUses <= 0)
            {
                FloatCustomText_FromValidationReject(thisCard, "No Spells to buff");
                return false;
            }
        
            return true;
        }
    }
}