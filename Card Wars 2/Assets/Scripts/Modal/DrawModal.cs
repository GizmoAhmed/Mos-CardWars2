using Buttons;
using GameManagement;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modal
{
    public class DrawModal : MonoBehaviour, IModal
    {
        public Transform cardGroupTransform;

        public GameObject freeDrawsLeftTextObj;

        public GameObject freeChoiceTextObj;
        private int _freePicks;

        public GameObject freeOfferTextObj;
        private int _freeOffer;

        public GameObject drawCostTextObj;
        public int drawCost;

        public GameObject paidChoiceTextObj;
        public int paidPicks;

        public GameObject paidOfferTextObj;
        public int paidOffer;

        public GameObject picksLeftTextObj;
        public int picksLeft;

        private DiscardModal _discardModal;

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

            if (picksLeftTextObj == null)
            {
                Debug.LogError($"picksLeftTextObj is null, make sure to set on {gameObject.name}");
            }
            
            if (cardGroupTransform == null)
            {
                Debug.LogError("_cardGroupTransform couldn't be found and is null");
            }

            GameManager gameManager = FindObjectOfType<GameManager>();

            paidPicks = gameManager.defaultPaidDrawChoices;
            paidOffer = gameManager.defaultPaidDrawOffering;
            paidChoiceTextObj.GetComponent<TextMeshProUGUI>().text = paidPicks.ToString();
            paidOfferTextObj.GetComponent<TextMeshProUGUI>().text = paidOffer.ToString();
            UpdateDrawCostText();

            _discardModal = FindAnyObjectByType<DiscardModal>(FindObjectsInactive.Include);

            if (_discardModal == null)
            {
                Debug.LogError("_discardModal couldn't be found");
            }
        }

        public void OpenDrawModal()
        {
            gameObject.SetActive(true);

            if (_discardModal == null)
            {
                Debug.LogWarning(
                    "While opening the draw model, an attempt was made to close the _discardModal, but it wasn't found");
                return;
            }

            if (_discardModal.gameObject.activeInHierarchy) // if discard modal already open?
            {
                _discardModal.CloseModal();
            }
        }

        //IModal
        public void CloseModal()
        {
            CloseDrawModal();
        }

        private void CloseDrawModal()
        {
            ClearPreviewCards();
            UpdatePicksLeft(0);
            gameObject.SetActive(false);
        }

        public void MinimizeModal()
        {
            gameObject.SetActive(false);
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
            _freePicks = choicePlayerStat;
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

        public void UpdatePaidChoice(int i)
        {
            int toBeChoice = paidPicks + i;

            if (toBeChoice < 1) return;

            if (toBeChoice >= paidOffer) return;

            paidPicks += i;
            paidChoiceTextObj.GetComponent<TextMeshProUGUI>().text = paidPicks.ToString();
            UpdateDrawCostText();
        }

        public void UpdatePaidOffer(int i)
        {
            int toBeOffer = paidOffer + i;

            if (toBeOffer < 1) return;

            if (toBeOffer <= paidPicks) return;

            paidOffer += i;
            paidOfferTextObj.GetComponent<TextMeshProUGUI>().text = paidOffer.ToString();
            UpdateDrawCostText();
        }

        private void UpdateDrawCostText()
        {
            drawCost = paidPicks * 2 + paidOffer;
            drawCostTextObj.GetComponent<TextMeshProUGUI>().text = drawCost.ToString();
        }

        [Client]
        public void ClearPreviewCards()
        {
            if (cardGroupTransform == null)
            {
                Debug.LogError($"{gameObject}: Child object that has the Horizontal Layout Group on it is null on the draw modal");
                return;
            }

            // Destroy all children of the card group
            foreach (Transform card in cardGroupTransform)
            {
                Destroy(card.gameObject);
            }
        }

        public void UpdatePicksLeft(int picks)
        {
            picksLeft = picks;
            picksLeftTextObj.GetComponent<TextMeshProUGUI>().text = picks.ToString();
        }
    }
}