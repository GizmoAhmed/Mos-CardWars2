using System;
using Mirror;
using UnityEngine;

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
    
    private void CurrentMagicUpdate(int oldMagic, int newMagic)
    {
        ui.MagicUIUpdate(newMagic, current : true);
    }

    private void MaxMagicUpdate(int oldMagic, int newMagic)
    {
        ui.MagicUIUpdate(newMagic, current : false);
    }

    private void MoneyUpdate(int oldMoney, int newMoney)
    {
        ui.MoneyUIUpdate(newMoney);
    }

    private void DrawUpdate(int oldDraws, int newDraws)
    {
        ui.DrawUIUpdate(newDraws);
    }

    private void ScoreUpdate(int oldScore, int newScore)
    {
        ui.ScoreUIUpdate(newScore);
    }

    private void HealthUpdate(int oldHealth, int newHealth)
    {
        ui.HealthUIUpdate(newHealth);
    }

    private void DrainUpdate(int oldDrain, int newDrain)
    {
        ui.DrainUIUpdate(newDrain);
    }

    private void RoundsUpdate(int oldRounds, int newRounds)
    {
        ui.RoundsUIUpdate(newRounds);
    }

    private void UpgradeCostUpdate(int oldUpgradeCost, int newUpgradeCost)
    {
        ui.UpgradeUIUpdate(newUpgradeCost);
    }
}