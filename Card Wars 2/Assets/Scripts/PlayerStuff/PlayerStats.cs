using System.Collections.Generic;
using Buttons;
using CardScripts;
using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Mirror;
using UnityEngine;

namespace PlayerStuff
{
    [RequireComponent(typeof(PlayerUI))]
    public class PlayerStats : NetworkBehaviour
    {
        public PlayerUI ui;

        [Header("Magic")] [SyncVar(hook = nameof(CurrentMagicUpdate))]
        public int currentMagic;

        [SyncVar(hook = nameof(MaxMagicUpdate))]
        public int maxMagic;

        [Header("Money")] [SyncVar(hook = nameof(MoneyUpdate))]
        public int money;

        [Header("Score")] [SyncVar(hook = nameof(ScoreUpdate))]
        public int score;

        [Header("Health")] [SyncVar(hook = nameof(HealthUpdate))]
        public int health;

        [SyncVar(hook = nameof(DrainUpdate))] public int drain;

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

            if (ui == null)
            {
                Debug.LogError("[SERVER] PlayerStats UI component could not be initialized.");
            }
            else
            {
                Debug.Log("[SERVER] PlayerStats UI component initialized.");
            }
        }

        [Command]
        public void CmdUpgradeMagic()
        {
            if (money >= upgradeCost)
            {
                money -= upgradeCost;
                maxMagic += 1;
                currentMagic += 1;
                upgradeCost += 1;
            }
        }

        [Command]
        public void CmdBurn(GameObject cardToBurn)
        {
            CardStats cardStats = cardToBurn.GetComponent<CardStats>();

            if (money >= cardStats.burnCost) // enough money to burn, then burn
            {
                money -= cardStats.burnCost; // spend to burn

                CardMovement cardMove = cardToBurn.GetComponent<CardMovement>();

                cardMove.RpcDiscard(); // discard the card, it (Discard) will handle the rest
            }
            else
            {
                Debug.Log("Insufficient funds to burn");
            }
        }

        public void DrainHealth()
        {
            health -= drain;
        }

        /// <summary>
        /// should never reach this function if already over-magicUse
        /// see ValidPlacement() in each card move child class
        /// </summary>
        /// <param name="amount"></param>
        public void UseMagic(int amount)
        {
            if (!isServer) return;

            currentMagic -= amount;
        }

        public void AddScore(int amount)
        {
            if (!isServer) return;
            score += amount;
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

            TargetGenerateFreeCards(connectionToClient);
        }

        [TargetRpc]
        private void TargetGenerateFreeCards(NetworkConnection target)
        {
            GetComponent<Player>().deckCollection.PreviewFreeCards();
        }

        [Command]
        public void CmdRequestPaidDraw(int choice, int offer)
        {
            if (choice < 1) return;
            if (offer <= choice) return;

            int drawCost = choice * 2 + offer;

            if (money < drawCost)
            {
                Debug.Log("Not enough money");
                return;
            }

            money -= drawCost;

            // GetComponentInChildren<DeckCollection>().PreviewOfferedCards(connectionToClient, offer);
        }
        
        public void CurrentMagicUpdate(int oldMagic, int newMagic)
        {
            if (ui == null)
            {
                Debug.LogWarning("PlayerStats UI component is null when trying to update current magicUse\n" +
                                 "Attempting to add UI Component again");

                // weirdly, after putting lobby stuff, _ui was null-ing itself for the host.
                // Didn't know why. This seems like a band-aid solution but a solution nonetheless
                // InitUI(); 

                return;
            }

            if (currentMagic < 0)
            {
                ui.MagicUIUpdate(newMagic, current_max: true, goingUnder: true);
                return;
            }

            ui.MagicUIUpdate(newMagic, current_max: true);
        }

        public void MaxMagicUpdate(int oldMagic, int newMagic)
        {
            if (currentMagic > maxMagic)
            {
                ui.MagicUIUpdate(newMagic, current_max: false, goingUnder: true);
                return;
            }

            ui.MagicUIUpdate(newMagic, current_max: false);
        }

        public void MoneyUpdate(int oldMoney, int newMoney)
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

        public void ScoreUpdate(int oldScore, int newScore)
        {
            ui.ScoreUIUpdate(newScore);
        }

        public void HealthUpdate(int oldHealth, int newHealth)
        {
            ui.HealthUIUpdate(newHealth);
        }

        public void DrainUpdate(int oldDrain, int newDrain)
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