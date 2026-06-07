using System;
using System.Collections.Generic;
using AbilityEvents;
using Buttons;
using CardScripts;
using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Mirror;
using Tiles;
using UnityEngine;

namespace PlayerStuff
{
    [RequireComponent(typeof(PlayerUI))]
    public class PlayerStats : NetworkBehaviour
    {
        public PlayerUI ui;

        [Header("Soul")] [SyncVar(hook = nameof(CurrentSoulUpdate))]
        public int currentSoul;

        [SyncVar(hook = nameof(MaxSoulUpdate))]
        public int maxSoul;

        [Header("Shards")] [SyncVar(hook = nameof(ShardsUpdate))]
        public int shards;

        [Header("Score")] [SyncVar(hook = nameof(PlayerScoreStatUpdate))]
        public int playerTotalScore;

        [Header("Health")] [SyncVar(hook = nameof(HealthUpdate))]
        public int health;

        [SyncVar(hook = nameof(Hook_DrainUpdate_ClientUI))]
        public int drain;

        [Header("Rounds Won")] [SyncVar(hook = nameof(RoundsWonUpdate))]
        public int roundsWon;

        [Header("Rounds Required")] [SyncVar(hook = nameof(RoundsRequiredUpdate))]
        public int roundsRequired;

        [Header("Upgrade Cost")] [SyncVar(hook = nameof(UpgradeCostUpdate))]
        public int upgradeCost;

        [Header("Free Draw Parameters")] [SyncVar(hook = nameof(FreeDrawsLeftUpdate))]
        public int freeDrawsLeft;

        [SyncVar(hook = nameof(ChoiceUpdate))] public int freeCardsChosen;
        [SyncVar(hook = nameof(OfferUpdate))] public int freeCardsOffered;

        public void InitUI()
        {
            ui = GetComponent<PlayerUI>();
            ui.Init(this);

            if (ui == null) Debug.LogError("PlayerStats UI component could not be initialized.");
        }

        [Command]
        public void CmdUpgradeMagic()
        {
            if (shards >= upgradeCost)
            {
                shards -= upgradeCost;
                maxSoul += 1;
                currentSoul += 1;
                upgradeCost += 1;
            }
        }

        [Command] // called from in game button click
        public void CmdBurn(GameObject cardToBurn)
        {
            CardStats cardStats = cardToBurn.GetComponent<CardStats>();

            if (shards >= cardStats.burnCost && cardStats.canBeBurned) // enough money to burn and can be burned? then burn
            {
                shards -= cardStats.burnCost; // spend to burn
                
                CardMovement cardMove = cardToBurn.GetComponent<CardMovement>();
                
                GlobalBroadcastBurn(cardToBurn);

                cardMove.ServerDiscard();
            }
            else
            {
                Debug.LogWarning($"{cardStats.gameObject.name} can't be burned because of either insufficient funds ({shards}), or burning being blocked for this creature via spell or rune or something");
            }
        }

        private void GlobalBroadcastBurn(GameObject burnedCard)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData burnData = new AbilityEventData(
                    AbilityEventType.AnyCardBurned,
                    burnedCard
                );

                // tell event manager to tell everyone (that cares) that a card was burned
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.TriggerEvents_ForAllSubscribersOfType(
                    burnData);
            }
            else
            {
                Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
            }
        }

        [Command]
        public void CmdActivateCreatureAbility(GameObject creatureToActivate)
        {
            CreatureStats creatureStats = creatureToActivate.GetComponent<CreatureStats>();
            int cost = creatureStats.abilityCost;

            if (shards >= cost)
            {
                shards -= cost;
                // Debug.Log($"...Spending shards ({cost}) to activate {creatureToActivate.name} ability");

                try
                {
                    creatureStats.cardData.ability.ExecuteAbility(creatureToActivate, null);
                    
                    LocalWhisperCardAbilityActivate(creatureStats);
                    // todo global broadcast
                    
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        $"Failed to activate ability {creatureToActivate.name}. <color=red>Error</color>: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"...Insufficient shards ({shards}) to activate {creatureToActivate.name} ({cost})");
            }
        }
        
        private void LocalWhisperCardAbilityActivate(CreatureStats creature)
        {
            // get tile of creature flooped
            Tile tile = creature.GetComponent<CreatureMovement>().GetLogicalTile();
            
            TileEventManager tileEventManager = tile.gameObject.GetComponent<TileEventManager>();

            // tell tile manager to broadcast that this creature flooped
            tileEventManager.OnCreatureAbilityOnTile(creature.gameObject);
        }

        public void DrainHealth()
        {
            health -= drain;
        }

        /// <summary>
        /// should never reach this function if already over-soulUse
        /// see ValidPlacement() in each card move child class
        /// </summary>
        /// <param name="amount"></param>
        public void UseMagic(int amount)
        {
            if (!isServer) return;

            currentSoul -= amount;
        }

        public void AddPlayerScore(int amount)
        {
            if (!isServer) return;
            playerTotalScore += amount;
        }

        [Command]
        public void CmdRequestFreeDraw()
        {
            if (freeDrawsLeft <= 0)
            {
                Debug.LogWarning($"{gameObject.name} tried to free-draw with no draws left");
                return;
            }

            freeDrawsLeft--;

            TargetOfferCardsOnModal(connectionToClient, freeCardsChosen, freeCardsOffered);
        }

        [Command]
        public void CmdRequestPaidDraw(int choice, int offer)
        {
            if (choice < 1) return;
            if (offer <= choice) return;

            int drawCost = choice * 2 + offer;

            if (shards < drawCost)
            {
                Debug.LogWarning(
                    $"{gameObject.name} tried a paid draw ({drawCost}) with insufficient shards ({shards})");
                return;
            }

            shards -= drawCost;

            TargetOfferCardsOnModal(connectionToClient, choice, offer);
        }

        [TargetRpc]
        private void TargetOfferCardsOnModal(NetworkConnection target, int cardsToChoose, int cardsToOffer)
        {
            GetComponent<Player>().deckCollection.OfferCardsPreview(cardsToChoose, cardsToOffer);
        }

        public void CurrentSoulUpdate(int oldMagic, int newMagic)
        {
            if (ui == null)
            {
                Debug.LogWarning("PlayerStats UI component is null when trying to update current soulUse\n" +
                                 "Attempting to add UI Component again");

                // weirdly, after putting lobby stuff, _ui was null-ing itself for the host.
                // Didn't know why. This seems like a band-aid solution but a solution nonetheless
                // InitUI(); 

                return;
            }

            if (currentSoul < 0)
            {
                ui.PlayerSoulUseUIUpdate(newMagic, current_max: true, goingUnder: true);
                return;
            }

            ui.PlayerSoulUseUIUpdate(newMagic, current_max: true);
        }

        public void MaxSoulUpdate(int oldMagic, int newMagic)
        {
            if (currentSoul > maxSoul)
            {
                ui.PlayerSoulUseUIUpdate(newMagic, current_max: false, goingUnder: true);
                return;
            }

            ui.PlayerSoulUseUIUpdate(newMagic, current_max: false);
        }

        public void ShardsUpdate(int oldMoney, int newMoney)
        {
            ui.MoneyUIUpdate(newMoney);
        }

        public void FreeDrawsLeftUpdate(int oldDraws, int newDraws)
        {
            ui.FreeDrawsUpdate(newDraws);
        }

        public void ChoiceUpdate(int old, int n)
        {
            ui.FreeChoiceUIUpdate(n);
        }

        public void OfferUpdate(int o, int n)
        {
            ui.FreeOfferUIUpdate(n);
        }

        public void PlayerScoreStatUpdate(int oldScore, int newScore)
        {
            ui.PlayerScoreUIUpdate(newScore);
        }

        public void HealthUpdate(int oldHealth, int newHealth)
        {
            ui.HealthUIUpdate(newHealth);
        }

        public void Hook_DrainUpdate_ClientUI(int oldDrain, int newDrain)
        {
            ui.DrainUIUpdate(newDrain);
        }

        public void RoundsWonUpdate(int oldRounds, int newRounds)
        {
            ui.RoundsUIUpdate(newRounds);

            // todo some kind of reset, clear the board or something
        }

        // should never actually change, but what the hell
        public void RoundsRequiredUpdate(int oldRounds, int newRounds)
        {
            ui.PopulateWinDots(newRounds);
        }

        public void UpgradeCostUpdate(int oldUpgradeCost, int newUpgradeCost)
        {
            ui.UpgradeUIUpdate(newUpgradeCost);
        }
    }
}