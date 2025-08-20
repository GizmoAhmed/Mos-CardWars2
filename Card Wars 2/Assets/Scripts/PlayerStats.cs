using Mirror;
using TMPro;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [Header("Magic")]
    public int CurrentMagic;
    public int MaxMagic;

    public GameObject magic1;
    public GameObject magic2;
    
    [Header("Money")]
    public int Money;
    
    public GameObject money1;
    public GameObject money2;
    
    [Header("Draws")]
    public int draws;

    public GameObject drawsLeft;
    
    [Header("Score")]
    public int Score;
    
    public GameObject score1;
    public GameObject score2;
    
    [Header("Health")]
    public int Health;
    public int Drain;
    
    public GameObject health1;
    public GameObject health2;
    public GameObject drain1;
    public GameObject drain2;
    
    [Header("Rounds")]
    public int RoundsWon;
    
    public GameObject rounds1;
    public GameObject rounds2;
    
    [Header("Upgrade Cost")]
    public int UpgradeCost;
    
    public GameObject costText;

    private void FindAllUI()
    {
        magic1      = GameObject.Find("Magic1");
        magic2      = GameObject.Find("Magic2");
        money1      = GameObject.Find("Money1");
        money2      = GameObject.Find("Money2");
        health1     = GameObject.Find("Health1");
        health2     = GameObject.Find("Health2");
        rounds1     = GameObject.Find("Rounds1");
        rounds2     = GameObject.Find("Rounds2");
        costText    = GameObject.Find("CostText");
        drain1      = GameObject.Find("Drain1");
        drain2      = GameObject.Find("Drain2");
        drawsLeft   = GameObject.Find("drawsLeft");
        
    }

    // ternary operator
    // variable = (condition) ? expressionTrue :  expressionFalse;
    
    [TargetRpc]
    public void MagicUpdate(int newCurrentMagic, int newMaxMagic)
    {
        TextMeshProUGUI magicText = (isOwned) ? magic1.GetComponent<TextMeshProUGUI>() : magic2.GetComponent<TextMeshProUGUI>();
        
        if (magicText != null)
        {
            magicText.text = newCurrentMagic + " / " +  newMaxMagic;
        }
        else
        {
            Debug.LogError("Magic update failed, magic text is null");
        }
    }
    
    [TargetRpc]
    public void InitializeStats(
        int startMagic, 
        int startMoney, 
        int startDraws,
        int startScore,
        int startHealth,
        int startRounds,
        int startHealthDrain,
        int startUpgradeCost)
    {
        
    }
}