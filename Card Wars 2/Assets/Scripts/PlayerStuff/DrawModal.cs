using Buttons;
using TMPro;
using UnityEngine;

namespace PlayerStuff
{
    public class DrawModal : MonoBehaviour
    {
        public GameObject freeDrawsLeftTextObj;
        public GameObject freeChoiceTextObj;
        public GameObject freeOfferTextObj;
        
        // private bool CanChangeSelectionParams => !(_player.freeCardsOffered - 1 <= _player.freeCardsChosen);
    
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

            if (freeChoiceTextObj == null)
            {
                Debug.LogError($"freeChoiceTextObj is null, make sure to set on {gameObject.name}");
            }

            if (freeOfferTextObj == null)
            {
                Debug.LogError($"freeOfferTextObj is null, make sure to set on {gameObject.name}");
            }
        }

        public void OpenDrawModal()
        {
            gameObject.SetActive(true);
        }

        public void CloseDrawModal()
        {
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

        public void SetFreeChoice(int choice)
        {
            if (freeChoiceTextObj == null)
            {
                Debug.LogError("freeChoiceTextObj is null");
                return;
            }
            
            freeChoiceTextObj.GetComponent<TextMeshProUGUI>().text = choice.ToString();
        }

        public void SetFreeOffer(int offer)
        {
            if (freeOfferTextObj == null)
            {
                Debug.LogError("freeOfferTextObj is null");
                return;
            }
            
            freeOfferTextObj.GetComponent<TextMeshProUGUI>().text = offer.ToString();
        }
    }
}
