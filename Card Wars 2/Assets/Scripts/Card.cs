using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool Grabbed;
    public bool Movable;
    public bool FaceUp; // true = visible, false = back

    void Start()
    {
        Movable = true;
    }

    public void Grab() { Grabbed = true; }

    public void LetGo() { Grabbed = false; }

    // Update is called once per frame
    void Update()
    {

        if (Movable && Grabbed)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

}
