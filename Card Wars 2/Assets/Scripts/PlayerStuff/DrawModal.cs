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
        
        public GameObject drawCostTextObj;
        public int drawCost;

        public GameObject paidChoiceTextObj;
        public int paidChoice;
        
        public GameObject paidOfferTextObj;
        public int paidOffer;
        
        // private bool CanChangeSelectionParams => !(_player.freeCardsOffered - 1 <= _player.freeCardsChosen);
    
        void Start()
        {
            // tell the buttons this class object exists
            foreach (DrawParamsButton button in transform.GetComponentsInChildren<DrawParamsButton>())
            {
                button.InitButton(this);
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

            if (paidChoiceTextObj == null)
            {
                Debug.LogError($"paidChoiceTextObj is null, make sure to set on {gameObject.name}");
            }

            if (paidOfferTextObj == null)
            {
                Debug.LogError($"paidOfferTextObj is null, make sure to set on {gameObject.name}");
            }

            if (drawCostTextObj == null)
            {
                Debug.LogError($"drawCostTextObj is null, make sure to set on {gameObject.name}");
            }

            GameManager gameManager = FindObjectOfType<GameManager>();

            paidChoice = gameManager.defaultPaidDrawChoices;
            paidOffer = gameManager.defaultPaidDrawOffering;
            paidChoiceTextObj.GetComponent<TextMeshProUGUI>().text = paidChoice.ToString();
            paidOfferTextObj.GetComponent<TextMeshProUGUI>().text = paidOffer.ToString();
            UpdateDrawCostText();
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
        
        private void UpdateDrawCostText()
        {
            drawCost = paidChoice * 2 + paidOffer;
            drawCostTextObj.GetComponent<TextMeshProUGUI>().text = drawCost.ToString();
        }

        public void UpdatePaidChoice(int i)
        {
            int toBeChoice = paidChoice + i;

            if (toBeChoice < 1) return;

            if (toBeChoice >= paidOffer) return;

            paidChoice += i;
            paidChoiceTextObj.GetComponent<TextMeshProUGUI>().text = paidChoice.ToString();
            UpdateDrawCostText();
        }

        public void UpdatePaidOffer(int i)
        {
            int toBeOffer = paidOffer + i;
            
            if (toBeOffer < 1) return;
            
            if (toBeOffer <= paidChoice) return;
            
            paidOffer += i;
            paidOfferTextObj.GetComponent<TextMeshProUGUI>().text = paidOffer.ToString();
            UpdateDrawCostText();
        }

        // draws left math already decided in PlayerStats.CmdRequestFreeDraw()
        public void GenerateFreeCards()
        {
            Debug.Log($"Generating: Please choose {_freeChoice} of {_freeOffer} cards");
        }
        
        public void GeneratePaidCards()
        {
            Debug.Log($"Thanks for the {drawCost} money! Generating: {paidChoice} of {paidOffer} cards");
        }
    }
}
