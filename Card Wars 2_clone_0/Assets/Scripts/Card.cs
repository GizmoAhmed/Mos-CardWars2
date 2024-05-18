using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool Grabbed;
    public bool Movable = true;
    public bool FaceUp; // true = visible, false = back

    public GameObject GameCanvas;

    public GameObject   DropZone;
    public GameObject   StartParent;
    public Vector2      StartPos;
    public GameObject   NewDropZone;
    public bool         isOverDropZone;

    void Start()
    {
        Movable = true;
        GameCanvas = GameObject.Find("GameCanvas");
		DropZone = GameObject.Find("DropZone");
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
        Debug.Log("Collided with something...")
;
        if (other.CompareTag("Land"))
        {
            Debug.Log("Landed...");

			NewDropZone = other.gameObject;
			isOverDropZone = true;
        }
        else 
        {
			isOverDropZone = false;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log("Leaving Land...");
		NewDropZone = null;
		isOverDropZone = false;
	}

	public void Grab() 
    { 
        Grabbed = true;

        // upon grab...
        StartParent = transform.parent.gameObject;      // save parent 
        StartPos = transform.position;                  // save position 
    }

    public void LetGo() 
    {
        Grabbed = false;

        if (isOverDropZone)
        {
            transform.SetParent(NewDropZone.transform, true);
			transform.localPosition = Vector2.zero;
			Movable = false;
        }
        else 
        {
            transform.position = StartPos;
            transform.SetParent(StartParent.transform, false);
        }
    }

    void Update()
    {

        if (Movable && Grabbed)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            // transform.SetParent(GameCanvas.transform, true);
        }
    }

}
