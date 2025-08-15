using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CardMovement : NetworkBehaviour
{
    private void Start()
    {
        Debug.Log("Card Movement Start for " + name);
    }

    public void OnPointerEnter()
    {
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit()
    {
        Debug.Log("OnPointerExit");
    }

    public void BeginDrag()
    {
        Debug.Log("BeginDrag");
    }

    public void EndDrag()
    {
        Debug.Log("EndDrag");
    }
}
