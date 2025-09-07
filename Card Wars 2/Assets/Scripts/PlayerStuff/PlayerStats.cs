using Mirror;
using UnityEngine;

namespace PlayerStuff
{
    [RequireComponent(typeof(PlayerUI))] 
    public class PlayerStats : NetworkBehaviour
    {
        private PlayerUI ui;
    
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

        [Header("Rounds")]
        [SyncVar(hook = nameof(RoundsUpdate))] public int roundsWon;

        [Header("Upgrade Cost")]
        [SyncVar(hook = nameof(UpgradeCostUpdate))] public int upgradeCost;
    
        public void InitUI()
        {
            ui = GetComponent<PlayerUI>();
            ui.Init(this);
        }
    
        [Command]
        public void CmdUpgradeMagic()
        {
            if (money >= upgradeCost)
            {
                money -= upgradeCost;
                maxMagic += 1;
                upgradeCost += 1;
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
        public void AddMagic(int amount)
        {
            if (!isServer) return;

            currentMagic += amount;
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
                Debug.LogError("PlayerStats UI component is null when trying to update current magic.");
                return;
            }

            if (currentMagic > maxMagic)
            {
                ui.MagicUIUpdate(newMagic, current_max : true, goingOver: true);
                return;
            }
        
            ui.MagicUIUpdate(newMagic, current_max : true);
        }

        public void MaxMagicUpdate(int oldMagic, int newMagic)
        {
            if (currentMagic > maxMagic)
            {
                ui.MagicUIUpdate(newMagic, current_max : false, goingOver: true);
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

        public void RoundsUpdate(int oldRounds, int newRounds)
        {
            ui.RoundsUIUpdate(newRounds);
        
            // todo some kind of reset, clear the board or something
        }

        public void UpgradeCostUpdate(int oldUpgradeCost, int newUpgradeCost)
        {
            ui.UpgradeUIUpdate(newUpgradeCost);
        }
    }
}