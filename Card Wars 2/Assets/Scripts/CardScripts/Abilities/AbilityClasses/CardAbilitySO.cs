using System;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities
{
    [Serializable]
    public abstract class CardAbilitySO : ScriptableObject
    {
        public abstract void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);

        /// <summary>
        /// An ability execution calls this when they want the tile the card is sitting on
        /// </summary>
        /// <param name="thisCard"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public CreatureStats GetCreature_FromTileInEventData(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats;

            if (eventData.CustomData !=
                null) // Execution was manual, called via placement in RegisterPassiveAbility, this building was placed on a middleTile
            {
                MiddleTile placedOnMiddleTile = eventData.CustomData["middleTile"] as MiddleTile;

                if (placedOnMiddleTile.logicalCreature != null) // middleTile has creature
                {
                    creatureStats = placedOnMiddleTile.logicalCreature.GetComponent<CreatureStats>();
                }
                else
                {
                    return null; // no creature to buff 
                }
            }
            else // Execution was passive, triggered by call back from event system, this building was told a creature was placed on it
            {
                if (thisCard == eventData.target)
                {
                    return null; // Don't buff self
                }

                creatureStats = eventData.target.GetComponent<CreatureStats>();

                // CreatureStats 
                if (creatureStats == null)
                {
                    return
                        null; // Not a creature, todo more ability event types (ie SpellCasted) would remove this if statement
                }
            }

            return creatureStats;
        }

        /// <summary>
        /// Essentially duplicates a card and adds to hand 
        /// </summary>
        /// <param name="redrawMe">the card you want to redraw</param>
        [Server]
        public void RedrawCard(GameObject redrawMe)
        {
            CardStats stats = redrawMe.GetComponent<CardStats>();

            string redrawID = stats.cardData.cardID;

            PlayerStats player = redrawMe
                .GetComponent<CardMovement>()
                .thisCardOwnerPlayerStats;
            
            MasterDeck masterDeck = FindObjectOfType<MasterDeck>();

            // redraw creature
            masterDeck.CreateThenSpawnCard(redrawID, player);
        }
    }
}