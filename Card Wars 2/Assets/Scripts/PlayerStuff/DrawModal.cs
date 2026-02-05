using Buttons;
using TMPro;
using UnityEngine;

namespace PlayerStuff
{
    public class DrawModal : MonoBehaviour
    {
        public GameObject freeDrawsLeftTextObj;
        
        // private bool CanChangeSelectionParams => !(_player.cardsOffered - 1 <= _player.cardsChosen);
    
        void Start()
        {
            // tell the buttons this class object exists
            foreach (DrawParamsButton button in transform.GetComponentsInChildren<DrawParamsButton>())
            {
                button.InitButton(this);
                Debug.Log($"{button.name} initialized with {gameObject.name}");
            }

            if (freeDrawsLeftTextObj == null)
            {
                Debug.LogError($"freeDrawsLeftTextObj is null, make sure to set on {gameObject.name} in the inspector");
            }
        }

        public void OnOpen()
        {
            // Debug.Log($"Opening {gameObject.name}...");
            gameObject.SetActive(true);
        }

        public void OnClose()
        {
            // Debug.Log($"Closing {gameObject.name}...");
            gameObject.SetActive(false);
        }

        public void SetFreeDrawsLeft(int draws)
        {
            if (freeDrawsLeftTextObj == null)
            {
                Debug.LogError("freeDrawsLeftTextObj is null");
                return;
            }
            
            freeDrawsLeftTextObj.GetComponent<TextMeshProUGUI>().text = draws.ToString();
        }
    }
}
