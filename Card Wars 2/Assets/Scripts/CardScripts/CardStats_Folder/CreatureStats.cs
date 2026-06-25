using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardStatss
{
    public class CreatureStats : CardStats
    {
        private CreatureDisplay _creatureDisplay;
        
        [Header("Creature Specific Stats")] [SyncVar(hook = nameof(Hook_UpdateCreatureStrength))]
        public int strength;
        [SyncVar] public int strengthMult = 1;

        [SyncVar(hook = nameof(Hook_UpdateCreatureDefense))]
        public int defense;
        [SyncVar] public int defenseMult = 1;

        [SyncVar(hook = nameof(UpdateScore))] public int score;

        [SyncVar(hook = nameof(Hook_UpdateAbilityCost))]
        public int abilityCost;

        [Header("Floop Amount")]
        // how many times a creature can floop in a turn
        [SyncVar] public int maxFloops = 1;
        [SyncVar] public int floopsLeft;
        
        [Header("Rune Booleans")]
        // If immortal, creature can't be killed and their defense can go negative as a result
        [SyncVar] public bool immortal = false;

        [SyncVar] public bool canBeBuffed = true;
        
        // todo prideful
        
        protected override void Awake()
        {
            base.Awake(); // set base display 
        
            _creatureDisplay = Display as CreatureDisplay;
        
            if (_creatureDisplay == null)
            {
                Debug.LogError($"CreatureDisplay not found on {gameObject.name}!");
            }
        }

        public override void SetAndApplyCardData(CardDataSO data, bool serverCall)
        {
            base.SetAndApplyCardData(data, serverCall);
            
            // for specifically creature stats and data, add on this stuff:
            if (!serverCall)
            {
                // stats already set from base call above, use them here
                _creatureDisplay.UpdateUIStrength(strength);
                _creatureDisplay.UpdateCardUIDefense(defense);
                _creatureDisplay.UpdateUI_AbilityCost(abilityCost);
                _creatureDisplay.UpdateScoreUI(score);
            }
        }

        public override void SetStats_FromData()
        {
            base.SetStats_FromData();

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
                Debug.LogError($"{gameObject.name}: card data was null when retrieved here");
            }
        }

        [Server] // called from inside a command
        public void ChangeCreatureStrength(int amount, bool buff)
        {
            amount *= strengthMult; 
            
            // Get the middleTile this card is on
            Tile middleTile = GetComponent<CardMovement>().GetLogicalTile();
            TileEventManager tileEventManager = middleTile.GetComponent<TileEventManager>();

            if (buff)
            {
                if (!canBeBuffed) return; // if can't be buffed, return

                strength += amount;

                GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureStrengthBuffed(gameObject, amount);

                // tell the middleTile the creature is on that it just got buffed, so the middleTile can tell other things on itself that
                tileEventManager.OnBuffCreatureStrengthOnTile(gameObject, amount);
            }
            else
            {
                if (strength - amount < 0) // so doesn't go negative
                {
                    amount = strength;
                    strength = 0; 
                }
                else
                {
                    strength -= amount;
                }
                
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureStrengthNerfed(gameObject, amount);
                tileEventManager.OnNerfCreatureStrengthOnTile(gameObject, amount);
            }

            score = strength + defense;
        }

        [Server]
        public void ChangeCreatureDefense(int amount, bool buff)
        {
            amount *= defenseMult; // gluttony rune
            
            // Get the middleTile this card is on
            Tile middleTile = GetComponent<CardMovement>().GetLogicalTile();
            TileEventManager tileEventManager = middleTile.GetComponent<TileEventManager>();

            if (buff)
            {
                if (!canBeBuffed) return; // if can't be buffed, return
                
                defense += amount;

                GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureDefenseBuffed(gameObject, amount);

                // tell the middleTile the creature is on that it just got buffed, so the middleTile can tell other things on itself that
                tileEventManager.OnBuffCreatureDefenseOnTile(gameObject, amount);
            }
            else
            {
                if (defense - amount <= 0 && !immortal) // and can die
                {
                    // broadcast left over defense instead of amount
                    GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureDefenseNerfed(gameObject, defense);
                    tileEventManager.OnNerfCreatureDefenseOnTile(gameObject, defense);
                    
                    // unbinds runes as well btw, so all the broadcast stuff should happen before
                    // todo i know for a fact I'll be back here when I have discard listeners
                    GetComponent<CreatureMovement>().ServerDiscard(); 
                }
                else // either not dead, or immortal
                {
                    defense -= amount;
                    
                    GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureDefenseNerfed(gameObject, amount);
                    tileEventManager.OnNerfCreatureDefenseOnTile(gameObject, amount);
                }
            }

            score = strength + defense;
        }

        [Server]
        public void ChangeAbilityCost(int amount, bool increase)
        {
            if (increase)
            {
                abilityCost += amount;
            }
            else
            {
                if (abilityCost - amount < 0) // subtracting causes negative?...
                {
                    abilityCost = 0; // ...then just set to 0
                }
                else
                {
                    abilityCost -= amount;
                }
            }
        }

        public void Hook_UpdateCreatureStrength(int old, int newStrength)
        {
            _creatureDisplay.UpdateUIStrength(newStrength);

            ContributeCreatureScore();
        }

        public void Hook_UpdateCreatureDefense(int oldDefense, int newDefense)
        {
            _creatureDisplay.UpdateCardUIDefense(newDefense);

            ContributeCreatureScore();
        }

        private void ContributeCreatureScore()
        {
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

        [Server]
        public void ResetFloops()
        {
            floopsLeft = maxFloops;
        }

        public void Hook_UpdateAbilityCost(int oldCost, int newCost)
        {
            _creatureDisplay.UpdateUI_AbilityCost(newCost);
        }

        public void UpdateScore(int oldScore, int newScore)
        {
            _creatureDisplay.UpdateScoreUI(newScore);
        }
    }
}