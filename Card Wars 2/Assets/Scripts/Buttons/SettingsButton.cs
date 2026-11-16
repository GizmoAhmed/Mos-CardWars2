using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public GameObject settingsPanel;

    private void Start()
    {
        if (settingsPanel == null)
        {
            Debug.LogError("Settings Panel is null, set it in inspector");
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }

    // button call
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }

    // button call
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
