using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Magic : NetworkBehaviour
{
    private PlayerManager playerManager;

    private TextMeshProUGUI magicText;

    public int CurrentMagic = 2;

    // Start is called before the first frame update
    void Start()
    {
        CurrentMagic = 2;
        magicText = GetComponent<TextMeshProUGUI>();
		magicText.text = CurrentMagic.ToString();
	}

    public void SpendMagic(int cost) 
    {
        CurrentMagic -= cost;
		magicText.text = CurrentMagic.ToString();

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();

        /// update across all clients
        playerManager.CmdUpdateMagic(CurrentMagic);
	}

	public void GainMagic(int gain) 
    {
        Debug.Log("Gaining Magic...");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
