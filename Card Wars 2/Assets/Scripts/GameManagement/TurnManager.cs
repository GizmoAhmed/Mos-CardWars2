using System.Collections;
using System.Collections.Generic;
using Mirror;
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
        net.identity.GetComponent<Player>().Disable(enable);
    }

    [Server]
    public void StartGame()
    {
        DisablePlayer(gameManager.Player0, whoFirst);
        DisablePlayer(gameManager.Player1, !whoFirst);
        
        _currentTurn = 1; turnUI.text = "TURN: " + _currentTurn;
    }
    
    public void PlayerReady(NetworkConnectionToClient net)
    {
        int currentIndex = net.connectionId;        // 0 or 1
        int otherIndex   = (currentIndex == 0) ? 1 : 0;

        if (_readyHit == 0)  // first player to hit ready
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
        
        gameManager.Player0.identity.GetComponent<PlayerStats>().DrainHealth();
        gameManager.Player1.identity.GetComponent<PlayerStats>().DrainHealth();
        
        Invoke(nameof(ContinuePlay), 3f);
    }
    
    // simply testing the pause nature of things 
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
