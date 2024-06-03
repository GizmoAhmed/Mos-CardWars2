using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardZoom : NetworkBehaviour
{
	public GameObject Canvas;
	public GameObject ZoomCard;

	private GameObject zoomCard;

	public void Awake()
	{
		Canvas = GameObject.Find("GameCanvas");
	}

	public void ZoomIn()
	{
		Debug.Log("Zoomed In...");
	}
}