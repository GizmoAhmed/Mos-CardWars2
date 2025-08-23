using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))] 
public class PlayerUI : NetworkBehaviour
{
    [Header("Magic")]
    public GameObject magic1;
    public GameObject magic2;
    
    [Header("Money")]
    public GameObject money1;
    public GameObject money2;
    
    [Header("Draws")]
    public GameObject drawsLeft;
    
    [Header("Score")]
    public GameObject score1;
    public GameObject score2;
    
    [Header("Health")]
    public GameObject health1;
    public GameObject health2;
    public GameObject drain1;
    public GameObject drain2;
    
    [Header("Rounds")]
    public GameObject rounds1;
    public GameObject rounds2;
    
    [Header("Upgrade Cost")]
    public GameObject costText;
    
    public PlayerStats stats;

    public void Init(PlayerStats s)
    {
        Debug.Log("Initializing player UI");
        stats = s; // player stats set up first, then comes here and sets it to ui, now there is an order of initialization
        FindAllUI(); 
    }
    
    private GameObject SafeFind(string objectName)
    {
        var obj = GameObject.Find(objectName);
        if (obj == null)
            Debug.LogWarning($"UI element '{objectName}' not found!");
        return obj;
    }

    public void FindAllUI()
    {
        magic1    = SafeFind("Magic1");
        magic2    = SafeFind("Magic2");
        money1    = SafeFind("Money1");
        money2    = SafeFind("Money2");
        health1   = SafeFind("Health1");
        health2   = SafeFind("Health2");
        score1    =  SafeFind("Score1");
        score2    =  SafeFind("Score2");
        drawsLeft = SafeFind("DrawsLeft");
        rounds1   = SafeFind("Rounds1");
        rounds2   = SafeFind("Rounds2");
        costText  = SafeFind("CostText");
        drain1    = SafeFind("Drain1");
        drain2    = SafeFind("Drain2");
        drawsLeft = SafeFind("drawsLeft");
    }


    public void MagicUIUpdate(int magic, bool current)
    {
        // magic 1 or 2
        TextMeshProUGUI magicText = (isOwned) ? magic1.GetComponent<TextMeshProUGUI>() : magic2.GetComponent<TextMeshProUGUI>();

        if (magicText == null)
        {
            Debug.LogError($"UI element '{magicText}' not found!");
            return;
        }

        if (stats == null)
        {
            Debug.LogError($"Stats component not found on this UI component!");
            return;
        }

        magicText.text = (current) ? magic + " / " + stats.maxMagic : stats.currentMagic + " / " + magic;
    }
}
