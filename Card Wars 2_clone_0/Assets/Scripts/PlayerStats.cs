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
    [SyncVar(hook = nameof(DrawUpdate))] public int draws;

    [Header("Score")]
    [SyncVar(hook = nameof(ScoreUpdate))] public int score;

    [Header("Health")]
    [SyncVar(hook = nameof(HealthUpdate))] public int health;
    [SyncVar(hook = nameof(DrainUpdate))] public int drain;

    [Header("Rounds")]
    [SyncVar(hook = nameof(RoundsUpdate))] public int roundsWon;

    [Header("Upgrade Cost")]
    [SyncVar(hook = nameof(UpgradeCostUpdate))] public int upgradeCost;
    
    public void InitStats()
    {
        ui = GetComponent<PlayerUI>();
        ui.Init(this);
    }

    private void Awake()
    {
        ui = GetComponent<PlayerUI>();
    }
    private void CurrentMagicUpdate(int oldMagic, int newMagic)
    {
        Debug.Log($"CURRENT >> {oldMagic} to {newMagic}...");
        ui.MagicUIUpdate(newMagic, current : true);
    }

    private void MaxMagicUpdate(int oldMagic, int newMagic)
    {
        Debug.Log($"MAX >> {oldMagic} to {newMagic}...");
        ui.MagicUIUpdate(newMagic, current : false);
    }

    private void MoneyUpdate(int oldMoney, int newMoney)
    {
        
    }

    private void DrawUpdate(int oldDraws, int newDraws)
    {
        
    }

    private void ScoreUpdate(int oldScore, int newScore)
    {
        
    }

    private void HealthUpdate(int oldHealth, int newHealth)
    {
        
    }

    private void DrainUpdate(int oldDrain, int newDrain)
    {
        
    }

    private void RoundsUpdate(int oldRounds, int newRounds)
    {
        
    }

    private void UpgradeCostUpdate(int oldUpgradeCost, int newUpgradeCost)
    {
        
    }
}