using Buttons;
using UnityEngine;

namespace PlayerStuff
{
    public class DrawModal : MonoBehaviour
    {
        // private bool CanChangeSelectionParams => !(_player.cardsOffered - 1 <= _player.cardsChosen);
    
        // Start is called before the first frame update
        void Start()
        {
            // tell the buttons this class object exists
            foreach (DrawParamsButton button in transform.GetComponentsInChildren<DrawParamsButton>())
            {
                button.InitButton(this);
                Debug.Log($"{button.name} initialized with {gameObject.name}");
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
    }
}
