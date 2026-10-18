using System;
using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using Extensions;
using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.CardStatss
{
    public class CreatureStats : CardStats
    {
        private CreatureDisplay _creatureDisplay;
        
        private CreatureMovement _creatureMovement;
        
        [Header("Creature Specific Stats")] 
        
        [SyncVar(hook = nameof(Hook_UpdateCreatureStrength))]
        public int strength;
        [SyncVar] public int strengthMult = 1;

        [SyncVar(hook = nameof(Hook_UpdateCreatureDefense))]
        public int defense;
        [SyncVar] public int defenseMult = 1;

        [SyncVar(hook = nameof(UpdateScore))] public int score;

        [SyncVar(hook = nameof(Hook_UpdateAbilityCost))]
        public int abilityCost;
        
        [Header("Element")] // todo eventually have a hook here if you want element changes to update UI on the fly
        [SyncVar] public CreatureDataSO.Element element;

        [Header("Ability Stuff")]
        
        [Tooltip("Amount of floop blockers, if 0, can floop")]
        public int floopBlockers = 0;
        
        // how many times a creature can floop in a turn
        [SyncVar] public int maxFloops = 1;
        [SyncVar] public int floopsLeft;

        [SyncVar] public bool multiFloop = false;
        [SyncVar] public bool isGamblingAbility = false;

        [Range(0, 100)]
        public int gambleChance = 0;
        
        [Header("Rune Booleans")]
        // If immortal, creature can't be killed and their defense can go negative as a result
        [SyncVar] public bool immortal = false;

        [Header("Blockers")] 
        [SyncVar] public int canBuffStrength = 0;
        [SyncVar] public int canBuffDefense = 0;
        // [SyncVar] public bool canBeBuffed = true;
        
        public enum AnimTarget
        {
            Strength,
            Defense
        }
        
        protected override void Awake()
        {
            base.Awake(); // set base display 
        
            _creatureDisplay = Display as CreatureDisplay;
        
            if (_creatureDisplay == null)
            {
                Debug.LogError($"CreatureDisplay not found on {gameObject.name}!");
            }
            
            _creatureMovement = Movement as  CreatureMovement;
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
                
                element = cData.element;

                abilityCost = cData.abilityCost;
            }
            else
            {
                Debug.LogError($"{gameObject.name}: card data was null when retrieved here");
            }
        }
        
        /// <summary>
        /// Check if this creature meets an element requirement
        /// </summary>
        public bool ElementMatch(CreatureDataSO.Element req)
        {
            if (req == CreatureDataSO.Element.Any)
            {
                return true;
            }

            return element == CreatureDataSO.Element.Any || element == req;
        }

        [Server] // called from inside a command
        public void UpdateCreatureStrength(int amount, bool buff, bool applyMult = true)
        {
            if (amount == 0) // no point
            {
                RpcPopAnimate(AnimTarget.Strength, buff);
                return;
            } 
            
            if (applyMult == true) // apply mult
            {
                amount *= strengthMult; 
            }
            
            // Get the middleTile this card is on
            Tile middleTile = GetComponent<CardMovement>().GetLogicalTile();
            TileEventManager tileEventManager = middleTile.GetComponent<TileEventManager>();

            if (buff)
            {
                if (canBuffStrength > 0) // > 0 means an ability incremented it, so can't buff
                {
                    Debug.Log($"Attempted to <color=orange>strengthen</color> {gameObject.name} but was <color=red>blocked</color>");
                    return;
                }
                
                strength += amount;
                
                // tell the middleTile the creature is on that it just got buffed, so the middleTile can tell other things on itself that
                tileEventManager.OnBuffCreatureStrengthOnTile(gameObject, amount);
            }
            else
            {
                strength -= amount; // can go negative
                
                // GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureStrengthNerfed(gameObject, amount);
                tileEventManager.OnNerfCreatureStrengthOnTile(gameObject, amount);
            }
            
            GlobalBroadCastStatChange(amount: amount, buff: buff, str: true);

            RpcPopAnimate(AnimTarget.Strength, buff);
            score = strength + defense;
        }

        [Server]
        public void UpdateCreatureDefense(int amount, bool buff, bool applyMult = true)
        {
            if (amount == 0) // no point
            {
                RpcPopAnimate(AnimTarget.Defense, buff);
                return;
            } 

            if (applyMult == true)
            {
                amount *= defenseMult; // gluttony rune
            }
            
            // Get the middleTile this card is on
            Tile middleTile = GetComponent<CardMovement>().GetLogicalTile();
            TileEventManager tileEventManager = middleTile.GetComponent<TileEventManager>();

            if (buff)
            {
                if (canBuffDefense > 0) // > 0 means an ability incremented it, so can't buff
                {
                    Debug.Log($"Attempted to <color=cyan>fortify</color> {gameObject.name} but was <color=red>blocked</color>");
                    return;
                }
                
                // if (!canBeBuffed) return; // if can't be buffed, return
                
                defense += amount;

                GlobalBroadCastStatChange(amount: amount, buff: true, str: false);
                
                // tell the middleTile the creature is on that it just got buffed, so the middleTile can tell other things on itself that
                tileEventManager.OnBuffCreatureDefenseOnTile(gameObject, amount);
            }
            else
            {
                if (defense - amount <= 0 && !immortal) // can die and gonna die
                {
                    // broadcast left over defense instead of amount
                    // GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureDefenseNerfed(gameObject, defense);
                    tileEventManager.OnNerfCreatureDefenseOnTile(gameObject, defense);
                    
                    // needs to happen before discard since those post discard values are problematic to some abilities
                    // ie logical player side being reset 
                    GlobalBroadCastStatChange(amount: amount, buff: false, str: false);
                    
                    // unbinds runes as well btw, so all the broadcast stuff should happen before
                    GetComponent<CreatureMovement>().ServerDiscard(); 
                }
                else // survived, the blow or just immortal 
                {
                    defense -= amount;
                    
                    GlobalBroadCastStatChange(amount: amount, buff: false, str: false);
                    
                    // GlobalAbilityEventManager.GlobalAbilityManagerInstance.OnAnyCreatureDefenseNerfed(gameObject, amount);
                    tileEventManager.OnNerfCreatureDefenseOnTile(gameObject, amount);
                }
            }

            RpcPopAnimate(AnimTarget.Defense, buff);
            score = strength + defense;
        }

        private void GlobalBroadCastStatChange(int amount, bool buff, bool str)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventType statChangeType;

                if (buff && str) // buff to strength
                {
                    statChangeType = AbilityEventType.AnyCreatureStrengthBuff;
                }
                else if (!buff && str) // nerf to strength
                {
                    statChangeType = AbilityEventType.AnyCreatureStrengthNerf;
                }
                else if (buff && !str) // buff to defense
                {
                    statChangeType = AbilityEventType.AnyCreatureDefenseBuff;
                }
                else // if (!buff && !Strength) // nerf to defense
                {
                    statChangeType = AbilityEventType.AnyCreatureDefenseNerf;
                }
                
                AbilityEventData statData = new AbilityEventData(
                        type: statChangeType,
                        targ: gameObject,
                        val: amount
                    );
                
                GlobalAbilityEventManager.GlobalAbilityManagerInstance
                    .TriggerEvents_ForAllSubscribersOfType(statData);
            }
            else
            {
                Debug.LogError(
                    $"<color=Orange>Failed Stat Change Broadcast</color> {name} couldn't find the GlobalAbilityEventManage Instance");
            }
        }

        [ClientRpc]
        private void RpcPopAnimate(AnimTarget target, bool isBuff)
        {
            _creatureDisplay?.PopAnimate(target, isBuff);
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
                // note, negative ability costs give the player money back
                abilityCost -= amount;
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
        
        /// <summary>
        /// Usually, abilities are called by clicking a creature ability button
        /// This function is for cases where it's done otherwise, typically from another ability
        /// </summary>
        [Server]
        public void ActivateCreatureAbility()
        {
            if (floopBlockers > 0) return; // if can't floop don't bother
            
            try
            {
                if (cardData.ability == null)
                {
                    Debug.LogError($"<color=orange>{gameObject.name}</color> has no ability set");
                    return;
                }

                if (!isGamblingAbility) // not gambling, just run ability
                {
                    ExecuteAndBroadcastAbility();
                }
                else
                {
                    // run chance
                    bool hit = cardData.ability.RollChance(chance: gambleChance, gameObject);

                    if (hit)
                    {
                        ExecuteAndBroadcastAbility();

                        // check:
                        if (ValidateAbilityExecution()) // passed checks?
                        {
                            ExecuteAndBroadcastAbility(); // go again
                        }
                        
                        Debug.Log($"{gameObject.name} gambled on an additional floop and <color=red>won</color>");
                    }
                    else
                    {
                        Debug.Log($"{gameObject.name} gambled on an additional floop and <color=red>missed</color>");
                        // missed? too bad, ability wasn't set off at all
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Failed to activate ability <color=orange>{name}</color>. <color=red>Error</color>: {e.Message}");
            }

        }
        
        private void ExecuteAndBroadcastAbility()
        {
            Debug.Log($"<color=green>ExecuteAndBroadCastAbility</color> on {gameObject.name}");
            cardData.ability.ExecuteAbility(gameObject, null);
            
            PlayerCardTracker playerCardTracker = gameObject.Ext_GetOwningCardTracker();

            // increment floop counter
            playerCardTracker.Server_IncrementCreatureActivation(gameObject);
            
            LocalWhisperCardAbilityActivate(this);
            // todo global broadcast
        }
        
        /// <summary>
        /// Check if ability can be re-executed after initial execute
        /// </summary>
        /// <returns></returns>
        private bool ValidateAbilityExecution()
        {
            // make sure it didn't invalidate its ability to floop with the first execute
                        
            if (floopBlockers > 0) return false; // can't floop, abort

            if (_creatureMovement.cardState == CardMovement.CardState.Discard) 
                return false; // killed itself, abort
                        
            // todo and any other reasons below
            return true;
        }

        private void LocalWhisperCardAbilityActivate(CreatureStats creature)
        {
            // get tile of creature flooped
            Tile tile = creature.GetComponent<CreatureMovement>().GetLogicalTile();

            TileEventManager tileEventManager = tile.gameObject.GetComponent<TileEventManager>();

            // tell tile manager to broadcast that this creature flooped
            tileEventManager.OnCreatureAbilityOnTile(creature.gameObject);
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