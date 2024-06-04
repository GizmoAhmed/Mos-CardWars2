using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloopExit : MonoBehaviour
{
	public GameObject card;
    public CardZoom zoom;

	private void Start()
	{
		zoom = card.GetComponent<CardZoom>();
	}
	public void floop() 
    {
        Debug.Log("To Be overwritten");

		exit();
    }

    public void exit() 
    {
		Debug.Log("exiting zoom...");

		zoom.ZoomOut();
	}
}
