using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss.Runes;
using Mirror;
using UnityEngine;

namespace CardScripts.CardStatss
{
    public class CreatureStats : CardStats
    {
        public CreatureDataSO creatureData => cardData as CreatureDataSO;

        [HideInInspector] public CreatureDisplay creatureDisplay;

        [Header("Creature Specific Stats")] [SyncVar(hook = nameof(Hook_UpdateCreatureStrength))]
        public int strength;

        [SyncVar(hook = nameof(UpdateDefense))]
        public int defense;

        [SyncVar(hook = nameof(UpdateScore))] public int score;

        [SyncVar(hook = nameof(UpdateAbilityCost))]
        public int abilityCost;

        [Header("Rune Stuff")] [SyncVar(hook = nameof(RuneChange))]
        public RuneBase currentRune1;

        public bool overRuneable;

        [SyncVar(hook = nameof(RuneChange))] public RuneBase currentRune2;

        public bool CanBeRuned => (currentRune1 == null) || (overRuneable && currentRune2 == null);

        public override void InitializeCard()
        {
            creatureDisplay = GetComponent<CreatureDisplay>();

            base.InitializeCard();

            creatureDisplay.InitDisplayWithData(this);

            creatureDisplay.UpdateUIStrength(strength);
            creatureDisplay.UpdateCardUIDefense(defense);
            creatureDisplay.UpdateUI_AbilityCost(abilityCost);

            score = strength + defense;
            creatureDisplay.UpdateCardUI_Score(score);
        }

        [Command]
        public override void CmdRefreshCardStats()
        {
            base.CmdRefreshCardStats();
            ApplyStatsFromData();
        }

        protected override void LocallyRefreshCardStats()
        {
            base.LocallyRefreshCardStats();
            ApplyStatsFromData();
        }

        protected override void ApplyStatsFromData()
        {
            base.ApplyStatsFromData();

            CreatureDataSO cData = cardData as CreatureDataSO;

            if (cData != null)
            {
                strength = cData.attack;
                defense = cData.defense;
                score = strength + defense;

                abilityCost = cData.abilityCost;
            }
            else
            {
                Debug.LogError($"{gameObject.name}: card data is passed null here");
            }
        }

        [Server] // called from inside a command
        public void ChangeCreatureStrength(int amount, bool buff)
        {
            if (buff)
            {
                strength += amount;
            }
            else
            {
                strength -= amount;
            }
            score = strength + defense;
        }

        [Server]
        public void ChangeDefense(int amount, bool buff)
        {
            if (buff)
            {
                defense += amount;
            }
            else
            {
                defense -= amount;
            }
            score = strength + defense;
        }

        [Server]
        public void ChangeAbilityCost(int amount, bool buff)
        {
            if (buff)
            {
                abilityCost -= amount;
            }
            else
            {
                abilityCost += amount;
            }
        }

        public void Hook_UpdateCreatureStrength(int old, int newStrength)
        {
            creatureDisplay.UpdateUIStrength(newStrength);

            CreatureMovement move = GetComponent<CreatureMovement>();

            // FIELD: only update player score for cards on the field (ie placing and stats change)
            // DISCARD: discard says to refresh stats, so don't mess with player score when resetting via discard since they are not on the board anymore
            if (move.cardState == CardMovement.CardState.Field)
            {
                int oldScore = score;
                int newScore = strength + defense;

                int diffScore = newScore - oldScore;

                // update player score accordingly
                GetComponent<CardMovement>().thisCardOwnerPlayerStats.AddPlayerScore(diffScore);
            }
        }

        public void UpdateDefense(int oldDefense, int newDefense)
        {
            creatureDisplay.UpdateCardUIDefense(newDefense);
            CreatureMovement move = GetComponent<CreatureMovement>();

            // only update player score for cards on the field (ie placing and stats change)
            if (move.cardState == CardMovement.CardState.Field)
            {
                int oldScore = score;
                int newScore = strength + defense;

                int diffScore = newScore - oldScore;

                // update player score accordingly
                GetComponent<CardMovement>().thisCardOwnerPlayerStats.AddPlayerScore(diffScore);
            }
        }

        public void UpdateAbilityCost(int oldCost, int newCost)
        {
            creatureDisplay.UpdateUI_AbilityCost(newCost);
        }

        public void UpdateScore(int oldScore, int newScore)
        {
            creatureDisplay.UpdateCardUI_Score(newScore);
        }

        public void RuneChange(RuneBase oldRune, RuneBase newRune)
        {
            creatureDisplay.DisplayRune(newRune);
        }
    }
}