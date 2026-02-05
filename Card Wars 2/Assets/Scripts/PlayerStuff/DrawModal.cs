using Buttons;
using Mirror;
using TMPro;
using UnityEngine;

namespace PlayerStuff
{
    public class DrawModal : MonoBehaviour
    {
        public GameObject freeDrawsLeftTextObj;
        
        public GameObject freeChoiceTextObj;
        private int _freeChoice;
        
        public GameObject freeOfferTextObj;
        private int _freeOffer;
        
        private int _drawCost;
        private int _paidChoice;
        private int _paidOffer;
        
        // private bool CanChangeSelectionParams => !(_player.freeCardsOffered - 1 <= _player.freeCardsChosen);
    
        void Start()
        {
            // tell the buttons this class object exists
            foreach (DrawParamsButton button in transform.GetComponentsInChildren<DrawParamsButton>())
            {
                button.InitButton(this);
                // Debug.Log($"{button.name} initialized with {gameObject.name}");
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
            // R*E*S*E*T
        }

        public void SetFreeDrawsLeft(int drawsPlayerStat)
        {
            if (freeDrawsLeftTextObj == null)
            {
                Debug.LogError("freeDrawsLeftTextObj is null");
                return;
            }
            
            freeDrawsLeftTextObj.GetComponent<TextMeshProUGUI>().text = drawsPlayerStat.ToString();
        }

        public void SetFreeChoice(int choicePlayerStat)
        {
            if (freeChoiceTextObj == null)
            {
                Debug.LogError("freeChoiceTextObj is null");
                return;
            }
            
            freeChoiceTextObj.GetComponent<TextMeshProUGUI>().text = choicePlayerStat.ToString();
            _freeChoice = choicePlayerStat;
        }

        public void SetFreeOffer(int offerPlayerStat)
        {
            if (freeOfferTextObj == null)
            {
                Debug.LogError("freeOfferTextObj is null");
                return;
            }
            
            freeOfferTextObj.GetComponent<TextMeshProUGUI>().text = offerPlayerStat.ToString();
            _freeOffer = offerPlayerStat;
        }

        // draws left math already decided in PlayerStats.CmdRequestFreeDraw()
        public void GenerateFreeCards()
        {
            Debug.Log($"Generating: Please choose {_freeChoice} of {_freeOffer} cards");
        }
        
        public void GeneratePaidCards()
        {
            
        }
    }
}
