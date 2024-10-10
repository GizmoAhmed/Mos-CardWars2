using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ActiveCharm : NetworkBehaviour
{
    private UnityEngine.UI.Image img;
    public bool Active;

    void Start()
    {
        Active = false;

        img = GetComponent<UnityEngine.UI.Image>();

        if (img == null)
        {
            Debug.LogWarning("Hey man, looks like img on " + GetComponentInParent<GameObject>().name + "is null");
        }
    }

    public void AttachCharm(GameObject spell)
    {
        Active = true;
        
        GameObject spellFaceObj = spell.transform.Find("FaceImage").gameObject;

		UnityEngine.UI.Image spellImage = spellFaceObj.gameObject.GetComponent<UnityEngine.UI.Image>();

		img.sprite = spellImage.sprite;
	}
}
