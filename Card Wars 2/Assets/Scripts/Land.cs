using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Land : NetworkBehaviour
{
    public bool Taken;
    public GameObject Card = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attach(GameObject card) 
    {
        Card = card;
    }
}
