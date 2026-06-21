using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System.Linq;
using System.Collections;
using Buttons;
using CardScripts;
using GameManagement;
using PlayerStuff;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class Player : NetworkBehaviour
{
    [Tooltip("0 = Player 1, 1 = Player 2")] [SyncVar]
    public int playerSide = -1;

    public CardPlacement cardPlacer;
    public PlayerStats playerStats;

    [SyncVar] public bool myTurn;

    [Header("Deck & Discard")] public DeckCollection deckCollection;

    private GameManager gameManager;

    private TurnManager turnManager;

    public PlayerCardTracker playerCardTracker;

    [Header("Buttons to disable")] 
    public Button draw;
    public Button upgrade;
    public Button ready;

    public override void OnStartClient()
    {
        base.OnStartClient();

        turnManager = FindAnyObjectByType<TurnManager>();

        deckCollection = GetComponentInChildren<DeckCollection>();

        cardPlacer = GetComponentInChildren<CardPlacement>();
        
        cardPlacer.Init();
        
        playerCardTracker = GetComponentInChildren<PlayerCardTracker>();
        
        playerStats = GetComponentInChildren<PlayerStats>();

        playerStats.InitUI();

        // myTurn = true;

        // find buttons
        draw = FindAnyObjectByType<DrawButton>().gameObject.GetComponent<Button>();
        upgrade = FindAnyObjectByType<UpgradeMagic>().gameObject.GetComponent<Button>();
        ready = FindAnyObjectByType<Ready>().gameObject.GetComponent<Button>();

        if (isServer) // must be the host, the first person to join
        {
        }
        else // the joiner, joins if there is a host, they are never alone
        {
            CmdCheckFullLobby();
        }
    }

    [Command]
    private void CmdCheckFullLobby()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.FullLobby();
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [Command]
    public void CmdSetReady()
    {
        turnManager.PlayerReady(connectionToClient);
    }


    /// <summary>
    /// Toggles a players turn on the server. Only called from a server method in TurnManager.cs
    /// </summary>
    /// <param name="enable">True for, 'yes it's your turn', false for 'not your turn'</param>
    [Server]
    public void Server_ToggleTurn(bool enable)
    {
        myTurn = enable;
    }

    /// <summary>
    /// Disables and enables the buttons of a certain player 
    /// </summary>
    /// <param name="enable">True for enable, false for disable</param>
    [TargetRpc]
    public void TargetRpc_ToggleButtons(bool enable)
    {
        draw.interactable = upgrade.interactable = ready.interactable = enable;
    }
}