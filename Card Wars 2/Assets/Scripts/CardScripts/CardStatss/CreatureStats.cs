using CardScripts.CardData;
using CardScripts.CardDisplays;
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
        
        [SyncVar(hook = nameof(UpdateAttack))] public int attack;

        [SyncVar(hook = nameof(UpdateDefense))] public int defense;
        
        [SyncVar(hook = nameof(UpdateScore))] public int score;
        
        [SyncVar(hook = nameof(UpdateAbilityCost))] public int abilityCost;
        
        [Header("Rune Stuff")]
        
        [SyncVar(hook = nameof(RuneChange))] public RuneBase currentRune1;
        
        public bool overRuneable;
        
        [SyncVar(hook = nameof(RuneChange))] public RuneBase currentRune2;

        public bool CanBeRuned => (currentRune1 == null) || (overRuneable && currentRune2 == null);
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            creatureDisplay = GetComponent<CreatureDisplay>();
            
            creatureDisplay.InitDisplay(this);

            creatureDisplay.UpdateUIAttack(attack);
            creatureDisplay.UpdateUIDefense(defense);
            creatureDisplay.UpdateUI_AbilityCost(abilityCost);
            
            score = attack + defense;
            creatureDisplay.UpdateUI_Score(score);
        }

        [Command]
        public override void RefreshCardStats()
        {
            base.RefreshCardStats();
            
            attack = creatureData.attack;
            defense = creatureData.defense;
            score = attack + defense;
            
            abilityCost = creatureData.abilityCost;
        }
        
        public void UpdateAttack(int oldAttack, int newAttack)
        {
            creatureDisplay.UpdateUIAttack(newAttack);
            
            score = attack + defense;
            creatureDisplay.UpdateUI_Score(score);
        }

        public void UpdateDefense(int oldDefense, int newDefense)
        {
            creatureDisplay.UpdateUIDefense(newDefense);
            
            score = attack + defense;
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