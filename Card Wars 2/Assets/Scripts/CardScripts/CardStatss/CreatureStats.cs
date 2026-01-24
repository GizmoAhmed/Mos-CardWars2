using CardScripts.CardData;
using CardScripts.CardDisplays;
using Mirror;
using UnityEngine;

namespace CardScripts.CardStatss
{
    public class CreatureStats : CardStats
    {
        public CreatureDataSO creatureData => cardData as CreatureDataSO;
        
        public CreatureDisplay creatureDisplay;
        
        [SyncVar(hook = nameof(UpdateAttack))] public int attack;

        [SyncVar(hook = nameof(UpdateDefense))] public int defense;
        
        [SyncVar(hook = nameof(UpdateScore))] public int score;
        
        [SyncVar(hook = nameof(UpdateAbilityCost))] public int abilityCost;
        
        // TODO public Charm ActiveCharm
        
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
    }
}