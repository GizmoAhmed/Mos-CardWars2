using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ActiveCharm : NetworkBehaviour
{
    public UnityEngine.UI.Image img;
    public bool Active;

    void Start()
    {
        img = GetComponent<UnityEngine.UI.Image>();

        if (img == null)
        {
            Debug.LogWarning("Hey man, looks like img on " + GetComponentInParent<GameObject>().name + "is null");
        }
    }

    public void Activate(bool activate, UnityEngine.UI.Image spell = null)
    {
        img.sprite = spell.sprite;
        Active = activate;
    }

}
