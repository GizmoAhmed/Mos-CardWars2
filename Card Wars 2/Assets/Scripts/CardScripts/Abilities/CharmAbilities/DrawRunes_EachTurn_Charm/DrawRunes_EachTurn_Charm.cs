using System.Collections.Generic;
using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardMovements;
using GameManagement;
using PlayerStuff;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardScripts.Abilities.CharmAbilities.DrawRunes_EachTurn_Charm
{
    [CreateAssetMenu(fileName = "DrawRunes_EachTurn_Charm", menuName = "Abilities/Charm/DrawRunes_EachTurn_Charm")]
    public class DrawRunes_EachTurn_Charm : PassiveAbilitySO
    {
        [Header("Available Runes to Draw From")]
        public List<RuneDataSO> runePool;
    
        [Header("Draw Amount")]
        public int runesToDraw;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log($"Executing {name}...");
        
            if (runePool == null || runePool.Count == 0)
            {
                Debug.LogError($"{name} has no runes in pool!");
                return;
            }
        
            if (runesToDraw <= 0 || runesToDraw > runePool.Count)
            {
                Debug.LogWarning($"{name} runesToDraw is {runesToDraw}, which doesn't fit rune list amount");
                return;
            }
        
            // Get owning player
            PlayerStats owningPlayer = thisCard
                .GetComponent<CardMovement>()
                .thisCardOwnerPlayerStats;
        
            if (owningPlayer == null)
            {
                Debug.LogError($"{name} couldn't find owning player!");
                return;
            }
        
            MasterDeck masterDeck = FindObjectOfType<MasterDeck>();
        
            if (masterDeck == null)
            {
                Debug.LogError($"{name} couldn't find MasterDeck!");
                return;
            }
        
            // Draw random runes
            List<RuneDataSO> randomRunes = GetRandomRunes(amount: runesToDraw);
        
            foreach (RuneDataSO rune in randomRunes)
            {
                masterDeck.CreateThenSpawnCard(rune.cardID, owningPlayer);
                Debug.Log($"{name} drew rune: {rune.cardID}");
            }
        }
    
        /// <summary>
        /// Get a list of random runes from the pool
        /// Allows duplicates (same rune can be drawn twice)
        /// </summary>
        private List<RuneDataSO> GetRandomRunes(int amount)
        {
            List<RuneDataSO> drawn = new List<RuneDataSO>();
        
            for (int i = 0; i < amount; i++)
            {
                int randomIndex = Random.Range(0, runePool.Count);
                drawn.Add(runePool[randomIndex]);
            }
        
            return drawn;
        }

        public void OnValidate()
        {
            if (!isGlobalListener) // needs to be global listener, as it listens for turns
            {
                Debug.LogError($"{name} should be globally listening");
            }
        
            if (isExecutableOnPlaced) // needs to be global listener, as it listens for turns
            {
                Debug.LogError($"{name} shouldn't be executing on place");
            }
        }
    }
}