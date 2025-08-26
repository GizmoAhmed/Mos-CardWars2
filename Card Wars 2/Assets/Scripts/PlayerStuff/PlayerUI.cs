using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Rounds1 Children")]
    public GameObject first1;
    public GameObject second1;
    public GameObject third1;
    public GameObject fourth1;

    [Header("Rounds2 Children")]
    public GameObject first2;
    public GameObject second2;
    public GameObject third2;
    public GameObject fourth2;

    
    [Header("Upgrade Cost")]
    public GameObject costText;
    
    public PlayerStats stats;

    public void Init(PlayerStats s)
    {
        stats = s;
        FindAllUI(); 
    }
    
    private GameObject SafeFind(string objectName)
    {
        var obj = GameObject.Find(objectName);
        if (obj == null)
            Debug.LogWarning($"UI element '{objectName}' not found!");
        return obj;
    }

    private void FindAllUI()
    {
        magic1    = SafeFind("Magic1");
        magic2    = SafeFind("Magic2");
        money1    = SafeFind("Money1");
        money2    = SafeFind("Money2");
        health1   = SafeFind("Health1");
        health2   = SafeFind("Health2");
        score1    = SafeFind("Score1");
        score2    = SafeFind("Score2");
        drawsLeft = GameObject.FindGameObjectWithTag("DrawsLeft");
        rounds1   = SafeFind("Rounds1");
        rounds2   = SafeFind("Rounds2");
        costText  = SafeFind("CostText");
        drain1    = SafeFind("Drain1");
        drain2    = SafeFind("Drain2");

        if (rounds1 != null)
        {
            first1  = rounds1.transform.Find("First1")?.gameObject;
            second1 = rounds1.transform.Find("Second1")?.gameObject;
            third1  = rounds1.transform.Find("Third1")?.gameObject;
            fourth1 = rounds1.transform.Find("Fourth1")?.gameObject;
        }

        if (rounds2 != null)
        {
            first2  = rounds2.transform.Find("First2")?.gameObject;
            second2 = rounds2.transform.Find("Second2")?.gameObject;
            third2  = rounds2.transform.Find("Third2")?.gameObject;
            fourth2 = rounds2.transform.Find("Fourth2")?.gameObject;
        }
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

    public void MoneyUIUpdate(int newMoney)
    {
        TextMeshProUGUI moneyText = (isOwned) ? money1.GetComponent<TextMeshProUGUI>() : money2.GetComponent<TextMeshProUGUI>();
        
        moneyText.text = newMoney.ToString();
    }

    public void DrawUIUpdate(int draws)
    {
        if (!isOwned) return;
        
        TextMeshProUGUI drawText = drawsLeft.GetComponent<TextMeshProUGUI>();

        drawText.text = draws.ToString();
    }

    public void ScoreUIUpdate(int newScore)
    {
        TextMeshProUGUI scoreText = (isOwned) ?  score1.GetComponent<TextMeshProUGUI>() : score2.GetComponent<TextMeshProUGUI>();
        
        scoreText.text = newScore.ToString();
    }

    public void HealthUIUpdate(int newHealth)
    {
        TextMeshProUGUI healthText = (isOwned) ?  health1.GetComponent<TextMeshProUGUI>() : health2.GetComponent<TextMeshProUGUI>();
        
        healthText.text = newHealth.ToString();
    }

    public void DrainUIUpdate(int newDrain)
    {
        TextMeshProUGUI drainText = (isOwned) ? drain1.GetComponent<TextMeshProUGUI>() : drain2.GetComponent<TextMeshProUGUI>();
        
        drainText.text = "- " + newDrain.ToString();
    }

    public void RoundsUIUpdate(int newRounds)
    {
        GameObject roundsParent = (isOwned) ? rounds1 : rounds2;
        
        for (int i = 0; i < roundsParent.transform.childCount; i++)
        {
            Transform child = roundsParent.transform.GetChild(i);
            Image bubbleImage = child.GetComponent<Image>();

            if (bubbleImage != null)
            {
                if (i < newRounds)
                {
                    bubbleImage.color = Color.green;
                }
                else
                {
                    bubbleImage.color = Color.white; 
                }
            }
        }
    }

    public void UpgradeUIUpdate(int cost)
    {
        if (!isOwned) return;
        
        TextMeshProUGUI c = costText.GetComponent<TextMeshProUGUI>();
        
        c.text = cost.ToString();
    }

}
