using CardScripts.CardData;
using CardScripts.CardDisplays;
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
        
        [Header("Creature Specific Stats")]
        
        [SyncVar(hook = nameof(UpdateAttack))] public int strength;

        [SyncVar(hook = nameof(UpdateDefense))] public int defense;
        
        [SyncVar(hook = nameof(UpdateScore))] public int score;
        
        [SyncVar(hook = nameof(UpdateAbilityCost))] public int abilityCost;
        
        [Header("Rune Stuff")]
        
        [SyncVar(hook = nameof(RuneChange))] public RuneBase currentRune1;
        
        public bool overRuneable;
        
        [SyncVar(hook = nameof(RuneChange))] public RuneBase currentRune2;

        public bool CanBeRuned => (currentRune1 == null) || (overRuneable && currentRune2 == null);
        
        public override void InitializeCard()
        {
            creatureDisplay = GetComponent<CreatureDisplay>();
            
            base.InitializeCard();
            
            creatureDisplay.InitDisplayWithData(this);

            creatureDisplay.UpdateUIStrength(strength);
            creatureDisplay.UpdateUIDefense(defense);
            creatureDisplay.UpdateUI_AbilityCost(abilityCost);
            
            score = strength + defense;
            creatureDisplay.UpdateUI_Score(score);
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

        [Server]
        public void ChangeStrength(int amount, bool buff)
        {
            if (buff)
            {
                strength += amount;
            }
            else
            {
                strength -= amount;
            }
        }

        [Command]
        public void CmdChangeDefense(int amount, bool buff)
        {
            if (buff)
            {
                defense += amount;
            }
            else
            {
                defense -= amount;
            }
        }

        [Command]
        public void CmdChangeAbilityCost(int amount, bool buff)
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

        public void UpdateAttack(int oldAttack, int newAttack)
        {
            creatureDisplay.UpdateUIStrength(newAttack);
            
            score = strength + defense;
            creatureDisplay.UpdateUI_Score(score);
        }

        public void UpdateDefense(int oldDefense, int newDefense)
        {
            creatureDisplay.UpdateUIDefense(newDefense);
            
            score = strength + defense;
            creatureDisplay.UpdateUI_Score(score);
        }

        public void UpdateAbilityCost(int oldCost, int newCost)
        {
            creatureDisplay.UpdateUI_AbilityCost(newCost);
        }
        
        public void UpdateScore(int oldScore, int newScore)
        {
            creatureDisplay.UpdateUI_Score(newScore);
        }

        public void RuneChange(RuneBase oldRune, RuneBase newRune)
        {
            creatureDisplay.DisplayRune(newRune);
        }
    }
}