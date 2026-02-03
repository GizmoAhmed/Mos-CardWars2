using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XButton : MonoBehaviour
{
    private GameObject _parent;
    
    void Start()
    {
        _parent = transform.parent.gameObject;
    }
    
    public void ExitOnClick()
    {
        _parent.SetActive(false);
    }
}
