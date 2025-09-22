using System;
using UnityEngine;

namespace Buttons
{
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject host;
        public GameObject join;

        public void OnHostClick()
        {
            if (host == null)
            {
                Debug.LogError("Missing Host Board - Set in Inspector");
            }
            
            host.SetActive(true);
        }

        public void OnJoinClick()
        {
            if (join == null)
            {
                Debug.LogError("Missing Join Board - Set in Inspector");
            }
            
            join.SetActive(true);
        }

        public void GoBack()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}