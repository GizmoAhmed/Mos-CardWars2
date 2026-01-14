using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlayerStuff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerStats))]
public class PlayerUI : NetworkBehaviour
{
    [Header("Magic")] private GameObject _magic1;
    private GameObject _magic2;
    private readonly Color _magicGreen = new Color32(87, 169, 108, 255); // 0-255 constructor

    [Header("Money")] private GameObject _money1;
    private GameObject _money2;

    [Header("Draws")] private GameObject _drawsLeft;

    [Header("Score")] private GameObject _score1;
    private GameObject _score2;

    [Header("Health")] private GameObject _health1;
    private GameObject _health2;
    private GameObject _drain1;
    private GameObject _drain2;

    [Header("Rounds")] private GameObject _rounds1;
    private GameObject _rounds2;
    public GameObject winDotSprite;

    [Header("Upgrade Cost")] private GameObject _costText;

    private PlayerStats stats;

    public void Init(PlayerStats s)
    {
        stats = s;
        FindAllUI();
    }

    private GameObject SafeFind(string objectName)
    {
        var obj = GameObject.Find(objectName);
        if (obj == null)
            Debug.LogWarning($"UI elementSprite '{objectName}' not found!");
        return obj;
    }

    private void FindAllUI()
    {
        _magic1 = SafeFind("Magic1");
        _magic2 = SafeFind("Magic2");
        _money1 = SafeFind("Money1");
        _money2 = SafeFind("Money2");
        _health1 = SafeFind("Health1");
        _health2 = SafeFind("Health2");
        _score1 = SafeFind("Score1");
        _score2 = SafeFind("Score2");
        _drawsLeft = GameObject.FindGameObjectWithTag("DrawsLeft");
        _rounds1 = SafeFind("Rounds1");
        _rounds2 = SafeFind("Rounds2");
        _costText = SafeFind("CostText");
        _drain1 = SafeFind("Drain1");
        _drain2 = SafeFind("Drain2");
    }

    public void MagicUIUpdate(int magic, bool current_max, bool goingUnder = false)
    {
        // magic 1 or 2
        TextMeshProUGUI magicText =
            (isOwned) ? _magic1.GetComponent<TextMeshProUGUI>() : _magic2.GetComponent<TextMeshProUGUI>();

        if (magicText == null)
        {
            Debug.LogError($"UI elementSprite '{magicText}' not found!");
            return;
        }

        if (stats == null)
        {
            Debug.LogError($"Stats component not found on this UI component!");
            return;
        }

        magicText.text = (current_max) ? magic + " / " + stats.maxMagic : stats.currentMagic + " / " + magic;

        if (goingUnder)
        {
            magicText.color = Color.red;
        }
        else
        {
            magicText.color = _magicGreen;
        }
    }

    public void MoneyUIUpdate(int newMoney)
    {
        TextMeshProUGUI moneyText =
            (isOwned) ? _money1.GetComponent<TextMeshProUGUI>() : _money2.GetComponent<TextMeshProUGUI>();

        moneyText.text = newMoney.ToString();
    }

    public void DrawUIUpdate(int draws)
    {
        if (!isOwned) return;

        TextMeshProUGUI drawText = _drawsLeft.GetComponent<TextMeshProUGUI>();

        drawText.text = draws.ToString();
    }

    public void ScoreUIUpdate(int newScore)
    {
        TextMeshProUGUI scoreText =
            (isOwned) ? _score1.GetComponent<TextMeshProUGUI>() : _score2.GetComponent<TextMeshProUGUI>();

        scoreText.text = newScore.ToString();
    }

    public void HealthUIUpdate(int newHealth)
    {
        TextMeshProUGUI healthText =
            (isOwned) ? _health1.GetComponent<TextMeshProUGUI>() : _health2.GetComponent<TextMeshProUGUI>();

        healthText.text = newHealth.ToString();
    }

    public void DrainUIUpdate(int newDrain)
    {
        TextMeshProUGUI drainText =
            (isOwned) ? _drain1.GetComponent<TextMeshProUGUI>() : _drain2.GetComponent<TextMeshProUGUI>();

        drainText.text = "- " + newDrain.ToString();
    }

    public void RoundsUIUpdate(int newRounds)
    {
        GameObject roundsParent = (isOwned) ? _rounds1 : _rounds2;

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

        TextMeshProUGUI c = _costText.GetComponent<TextMeshProUGUI>();

        c.text = cost.ToString();
    }

    public void PopulateWinDots(int numberOfRoundsToWin)
    {
        if (winDotSprite == null)
        {
            Debug.LogError("Win sprite object not set on player UI");
            return;
        }

        for (int i = 0; i < numberOfRoundsToWin; i++)
        {
            if (isOwned)
            {
                Instantiate(winDotSprite, _rounds1.transform);
            }
            else
            {
                Instantiate(winDotSprite, _rounds2.transform);
            }
        }
    }
}