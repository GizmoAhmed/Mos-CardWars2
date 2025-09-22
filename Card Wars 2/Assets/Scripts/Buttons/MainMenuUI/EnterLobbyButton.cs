using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterLobbyButton : MonoBehaviour
{
    public TMP_InputField inputField;

    [HideInInspector] public string code;

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError($"_inputField is null on {name} - Set In Inspector");
        }
    }

    public void HostLobby()
    {
        code = inputField.text;
        Debug.LogWarning($"Hosting Lobby w/ code: {code}");
    }

    public void JoinLobby()
    {
        code = inputField.text;
        Debug.LogWarning($"Attempting to join Lobby w/ code: {code}");   
    }
}
