using CardScripts;
using Mirror;
using UnityEngine;

namespace PlayerStuff
{
    [RequireComponent(typeof(PlayerUI))] 
    public class PlayerStats : NetworkBehaviour
    {
        public PlayerUI ui;
    
        [Header("Magic")]
        [SyncVar(hook = nameof(CurrentMagicUpdate))] public int currentMagic;
        [SyncVar(hook = nameof(MaxMagicUpdate))] public int maxMagic;

        [Header("Money")]
        [SyncVar(hook = nameof(MoneyUpdate))] public int money;

        [Header("Draws")]
        [SyncVar(hook = nameof(DrawUpdate))] public int drawCost;

        [Header("Score")]
        [SyncVar(hook = nameof(ScoreUpdate))] public int score;

        [Header("Health")]
        [SyncVar(hook = nameof(HealthUpdate))] public int health;
        [SyncVar(hook = nameof(DrainUpdate))] public int drain;

        [Header("Rounds Won")]
        [SyncVar(hook = nameof(RoundsWonUpdate))] public int roundsWon;
        
        [Header("Rounds Required")]
        [SyncVar(hook = nameof(RoundsRequiredUpdate))] public int roundsRequired;

        [Header("Upgrade Cost")]
        [SyncVar(hook = nameof(UpgradeCostUpdate))] public int upgradeCost;
    
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
                
                CardMovement cardMove =  cardToBurn.GetComponent<CardMovement>();
                
                if (cardMove.cardState == CardMovement.CardState.Field)
                {
                    currentMagic += cardStats.magic; // give back magic
                    score -= (cardStats.attack + cardStats.defense);
                }
                
                cardMove.RpcDiscard();
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
        /// should never reach this function if already over-magic
        /// see ValidPlace() in middleland
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

        public void CurrentMagicUpdate(int oldMagic, int newMagic)
        {
            if (ui == null) 
            {
                Debug.LogWarning("PlayerStats UI component is null when trying to update current magic\n" +
                               "Attempting to add UI Component again");
                
                // weirdly, after putting lobby stuff, _ui was null-ing itself for the host.
                // Didn't know why. This seems like a band-aid solution but a solution nonetheless
                // InitUI(); 
                
                return;
            }

            if (currentMagic < 0)
            {
                ui.MagicUIUpdate(newMagic, current_max : true, goingUnder: true);
                return;
            }
        
            ui.MagicUIUpdate(newMagic, current_max : true);
        }

        public void MaxMagicUpdate(int oldMagic, int newMagic)
        {
            if (currentMagic > maxMagic)
            {
                ui.MagicUIUpdate(newMagic, current_max : false, goingUnder: true);
                return;
            }
        
            ui.MagicUIUpdate(newMagic, current_max : false);
        }

        public void MoneyUpdate(int oldMoney, int newMoney)
        {
            ui.MoneyUIUpdate(newMoney);
        }

        public void DrawUpdate(int oldDraws, int newDraws)
        {
            ui.DrawUIUpdate(newDraws);
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