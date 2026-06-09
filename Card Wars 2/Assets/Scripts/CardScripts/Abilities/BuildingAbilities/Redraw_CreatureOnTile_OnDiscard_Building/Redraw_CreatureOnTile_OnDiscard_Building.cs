using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.Redraw_CreatureOnTile_OnDiscard_Building
{
    [CreateAssetMenu(fileName = "Redraw_CreatureOnTile_OnDiscard_Building", menuName = "Abilities/Building/Redraw_CreatureOnTile_OnDiscard_Building")]
    public class Redraw_CreatureOnTile_OnDiscard_Building : PassiveAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log($"{thisCard.name} calls redraw on {eventData.target.gameObject.name}");

            CreatureStats targetsCreatureStats = eventData.target.GetComponent<CreatureStats>();

            string redrawID = targetsCreatureStats.cardData.cardID;

            PlayerStats owningPlayer = eventData.target
                .GetComponent<CreatureMovement>()
                .thisCardOwnerPlayerStats;
        
            MasterDeck masterDeck = FindObjectOfType<MasterDeck>();

            // redraw creature
            masterDeck.CreateThenSpawnCard(redrawID, owningPlayer);
        
            // discard this building
            thisCard.GetComponent<CardMovement>().ServerDiscard();
        }
    
        public void OnValidate()
        {
            if (isGlobalListener && isExecutableOnPlaced && !eventsThatTriggerThisAbility.Contains(AbilityEventType.CardDiscardedFromTile))
            {
                Debug.LogError($"{name} shouldn't be executable on place, and should be a tile listener for CardDiscardedFromTile");
            }
        }
    }
}
