using System.Collections;
using System.Collections.Generic;
using GameManagement;
using Mirror;
using PlayerStuff;
using TMPro;
using UnityEngine;
using AbilityEvents;


public class TurnManager : NetworkBehaviour
{
    public GameManager gameManager;

    private PlayerStats _player0Stats;
    private PlayerStats _player1Stats;

    [SyncVar(hook = nameof(OnTurnChanged))]
    public int currentTurn = -1;

    [SyncVar] private int _lastPlayed;

    [SyncVar] private int _readyHit = 0;

    public GameObject turnObj;

    private TextMeshProUGUI turnUI;

    [Tooltip("True (P0 goes first)\nFalse (P1 goes first)")]
    public bool whoFirst;
    
    [SerializeField] private TextMeshProUGUI nextUpUI;

    public void Init(GameManager gm)
    {
        gameManager = gm;

        turnUI = turnObj.GetComponent<TextMeshProUGUI>();

        currentTurn = _readyHit = 0;

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

        net.identity.GetComponent<Player>().Server_ToggleTurn(enable);
        net.identity.GetComponent<Player>().TargetRpc_ToggleButtons(enable);
    }

    /// <summary>
    /// Starts one player and disables the other
    /// Starts turn counter at 1
    /// Sets PlayerStats
    /// </summary>
    [Server]
    public void StartGame()
    {
        DisablePlayer(gameManager.Player1, whoFirst);
        DisablePlayer(gameManager.Player2, !whoFirst);
        
        // Player1 acts first when whoFirst is true, so the other player is next
        int firstIndex = whoFirst ? 0 : 1;
        UpdateNextUp(1 - firstIndex); // update ui

        currentTurn = 1;
        turnUI.text = "TURN: " + currentTurn;
        
        _player0Stats = gameManager.Player1.identity.GetComponent<PlayerStats>();
        _player1Stats = gameManager.Player2.identity.GetComponent<PlayerStats>();
    }

    public void PlayerReady(NetworkConnectionToClient net)
    {
        int currentIndex = net.connectionId; // 0 or 1
        int otherIndex = (currentIndex == 0) ? 1 : 0;

        if (_readyHit == 0) // first player to hit ready
        {
            _readyHit = 1;

            // disable current player, enable the other
            DisablePlayer(gameManager.playersConnections[currentIndex], false);
            DisablePlayer(gameManager.playersConnections[otherIndex], true);
            
            // otherIndex is acting now, and also opens the next turn, so they're "next up" again
            UpdateNextUp(otherIndex);
        }
        else // both players went, swap phase
        {
            SwapPhase();
            _readyHit = 0;
        }
    }

    /// <summary>
    /// Tells each player who acts after the player currently acting, relative to themselves.
    /// </summary>
    [Server]
    private void UpdateNextUp(int nextPlayerIndex)
    {
        for (int i = 0; i < 2; i++)
        {
            TargetUpdateNextUpUI(ConnectionFor(i), isYou: i == nextPlayerIndex);
        }
    }

    private NetworkConnectionToClient ConnectionFor(int playerIndex)
        => playerIndex == 0 ? gameManager.Player1 : gameManager.Player2;

    [TargetRpc]
    private void TargetUpdateNextUpUI(NetworkConnectionToClient target, bool isYou)
    {
        if (nextUpUI == null) return;
        nextUpUI.text = isYou ? "YOU" : "THEM";
    }
    
    [Server]
    private void SwapPhase()
    {
        _lastPlayed = gameManager.Player1.identity.GetComponent<Player>().myTurn ? 0 : 1;

        // disable both players
        /*DisablePlayer(gameManager.Player1, false);
        DisablePlayer(gameManager.Player2, false);*/

        PlayerStats stats0 = gameManager.Player1.identity.GetComponent<PlayerStats>();
        PlayerStats stats1 = gameManager.Player2.identity.GetComponent<PlayerStats>();

        stats0.DrainHealth();
        stats1.DrainHealth();
        
        GlobalBroadcastTurnEnd();
        
        Server_EndOfTurnReset_BothPlayers();
        
        bool gameWinnerDecided = CheckScore();
        
        if (gameWinnerDecided)
        {
            // don't continue play
        }
        else // continue play
        {
            ContinuePlay();
            // Invoke(nameof(ContinuePlay), .2f); 
        }
    }

    [Server]
    private bool CheckScore()
    {
        bool p0Cleared = _player0Stats.playerTotalScore >= _player0Stats.health;
        bool p1Cleared = _player1Stats.playerTotalScore >= _player1Stats.health;

        PlayerStats roundWinner = DecideRoundWinner(p0Cleared, p1Cleared);

        if (roundWinner != null)
        {
            return AwardRound(roundWinner);
        }
        
        return false;
    }

    /// <summary>Returns the round winner, or null if nobody won (or it's a tie).</summary>
    private PlayerStats DecideRoundWinner(bool p0Cleared, bool p1Cleared)
    {
        if (p0Cleared && p1Cleared)
        {
            // both cleared: higher total score wins, equal means a tie
            if (_player0Stats.playerTotalScore > _player1Stats.playerTotalScore) return _player0Stats;
            if (_player1Stats.playerTotalScore > _player0Stats.playerTotalScore) return _player1Stats;
            return null; // tie: do nothing for now
        }

        if (p0Cleared) return _player0Stats;
        if (p1Cleared) return _player1Stats;
        return null;
    }

    /// <summary>Gives the round to a player and checks whether that wins the game.</summary>
    private bool AwardRound(PlayerStats winner)
    {
        gameManager.RoundWin(winner);

        if (winner.roundsWon == gameManager.roundsToWin)
        {
            gameManager.GameWin(ConnectionOf(winner));
            return true;
        }
        return false;
    }

    private NetworkConnectionToClient ConnectionOf(PlayerStats stats)
        => stats == _player0Stats ? gameManager.Player1 : gameManager.Player2;
    
    [Server]
    private void GlobalBroadcastTurnEnd()
    {
        if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
        {
            AbilityEventData turnEndData = new AbilityEventData
            (
                AbilityEventType.AnyTurnEnd
            );

            // tell event manager to tell everyone (that cares) that the turn ended
            GlobalAbilityEventManager.GlobalAbilityManagerInstance
                .TriggerEvents_ForAllSubscribersOfType(turnEndData);
        }
        else
        {
            Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
        }
    }

    [Server]
    public void ContinuePlay()
    {
        currentTurn++;

        if (_lastPlayed == 0)
        {
            DisablePlayer(gameManager.Player1, true);
            DisablePlayer(gameManager.Player2, false);
        }
        else
        {
            DisablePlayer(gameManager.Player1, false);
            DisablePlayer(gameManager.Player2, true);
        }
        
        // _lastPlayed opens the new turn, so the other player is next
        UpdateNextUp(1 - _lastPlayed);
    }

    /// <summary>
    /// reset card tracking stats in each card tracker
    /// </summary>
    [Server]
    private void Server_EndOfTurnReset_BothPlayers()
    {
        PlayerCardTracker ct0 = _player0Stats.GetComponent<PlayerCardTracker>();
        PlayerCardTracker ct1 = _player1Stats.GetComponent<PlayerCardTracker>();
        
        // reset card tracking
        ct0.Server_EndOfTurnCardTrackerIncrement();
        ct1.Server_EndOfTurnCardTrackerIncrement();

        // reset draws
        _player0Stats.freeDrawsLeft = _player0Stats.freeDrawsPerTurn;
        _player1Stats.freeDrawsLeft = _player1Stats.freeDrawsPerTurn;
    }
}