using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManaHealthReady : NetworkBehaviour
{
    public PlayerManager playerManager;

    public GameManager gameManager;

	public TextMeshProUGUI Magic;
	public TextMeshProUGUI Health;

	public Button ReadyButton;

	public void Ready() 
	{
		Debug.Log("Ready!");
	}

	// Start is called before the first frame update
	void Start()
    {
		Magic = GameObject.Find("ThisManaText").GetComponent<TextMeshProUGUI>();
		Health = GameObject.Find("ThisHealthText").GetComponent<TextMeshProUGUI>();

		ReadyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
	}

	public void SpendMagic(int magic) 
	{

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
