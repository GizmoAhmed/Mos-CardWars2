using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlayerStuff;
using TMPro;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public GameManager gameManager;

    [SyncVar(hook = nameof(OnTurnChanged))]
    public int _currentTurn = -1;

    [SyncVar] private int _lastPlayed;

    [SyncVar] private int _readyHit = 0;

    public GameObject turnObj;

    private TextMeshProUGUI turnUI;

    [Tooltip("True (P0 goes first)\nFalse (P1 goes first)")]
    public bool whoFirst;

    public void Init(GameManager gm)
    {
        gameManager = gm;

        turnUI = turnObj.GetComponent<TextMeshProUGUI>();

        _currentTurn = _readyHit = 0;

        if (turnUI == null) Debug.LogError($"Make sure to set turnUI in inspector on {gameObject.name}");
    }

    private void OnTurnChanged(int oldValue, int newValue)
    {
        if (turnUI == null && turnObj != null)
            turnUI = turnObj.GetComponent<TextMeshProUGUI>();

        if (turnUI != null)
            turnUI.text = "TURN: " + newValue;
    }


    /// <summary>
    /// True for enable, false for disable
    /// </summary>
    /// <param name="net"></param>
    /// <param name="enable"></param>
    [Server]
    public void DisablePlayer(NetworkConnectionToClient net, bool enable)
    {
        if (net == null)
        {
            Debug.LogError($"Network connect to Client({net.connectionId}) is null right here");
        }

        net.identity.GetComponent<Player>().Disable(enable);
    }

    [Server]
    public void StartGame()
    {
        DisablePlayer(gameManager.Player0, whoFirst);
        DisablePlayer(gameManager.Player1, !whoFirst);

        _currentTurn = 1;
        turnUI.text = "TURN: " + _currentTurn;
    }

    public void PlayerReady(NetworkConnectionToClient net)
    {
        int currentIndex = net.connectionId; // 0 or 1
        int otherIndex = (currentIndex == 0) ? 1 : 0;

        if (_readyHit == 0) // first player to hit ready
        {
            _readyHit = 1;

            // disable current player, enable the other
            DisablePlayer(gameManager.players[currentIndex], false);
            DisablePlayer(gameManager.players[otherIndex], true);
        }
        else // both players went, swap phase
        {
            SwapPhase();
            _readyHit = 0;
        }
    }


    [Server]
    public void SwapPhase()
    {
        _lastPlayed = gameManager.Player0.identity.GetComponent<Player>().myTurn ? 0 : 1;

        DisablePlayer(gameManager.Player0, false);
        DisablePlayer(gameManager.Player1, false);

        PlayerStats stats0 = gameManager.Player0.identity.GetComponent<PlayerStats>();
        PlayerStats stats1 = gameManager.Player1.identity.GetComponent<PlayerStats>();

        stats0.DrainHealth();
        stats1.DrainHealth();

        // todo ActivePhase(), everyone does there stuff, like watching the cards in balatro tick away when you play a hand
        
        CheckScore();

        Invoke(nameof(ContinuePlay), 4.5f); // invoked in time for testing the pausing nature of the ActivePhase
    }

    [Server]
    public void CheckScore()
    {
        Debug.Log("Checking Scores...");
        
        PlayerStats stats0 = gameManager.Player0.identity.GetComponent<PlayerStats>();
        PlayerStats stats1 = gameManager.Player1.identity.GetComponent<PlayerStats>();

        bool p0Cleared = stats0.score >= stats0.health;
        bool p1Cleared = stats1.score >= stats1.health;

        if (p0Cleared && p1Cleared) // both cleared, tie
        {
            if (stats0.score > stats1.score) // both cleared but someone has the higher score
            {
                gameManager.RoundWin(stats0);
            }
            else if (stats1.score > stats0.health)
            {
                gameManager.RoundWin(stats1);
            }
            else
            {
                // else tie case: do nothing for now
            }
        }
        else if (p0Cleared)
        {
            gameManager.RoundWin(stats0);
        }
        else if (p1Cleared)
        {
            gameManager.RoundWin(stats1);
        }

        // Game Win condition check
        if (stats0.roundsWon == gameManager.roundsToWin)
            gameManager.GameWin(gameManager.Player0);
        else if (stats1.roundsWon == gameManager.roundsToWin)
            gameManager.GameWin(gameManager.Player1);
    }

    [Server]
    public void ContinuePlay()
    {
        _currentTurn++;

        if (_lastPlayed == 0)
        {
            DisablePlayer(gameManager.Player0, true);
            DisablePlayer(gameManager.Player1, false);
        }
        else
        {
            DisablePlayer(gameManager.Player0, false);
            DisablePlayer(gameManager.Player1, true);
        }
    }
}