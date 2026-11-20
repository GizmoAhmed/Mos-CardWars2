using UnityEngine;
using UnityEngine.UI;

namespace PlayerStuff
{
    public class ButtonsManager : MonoBehaviour
    {
        public Button draw;
        public Button upgrade;
        public Button ready;

        public void SetInteractable_ForAllButtons(bool value)
        {
            if (draw == null || upgrade == null || ready == null)
            {
                Debug.LogError("<color=orange>Buttons Manager Failure</color>: Some or all buttons were null when a flip was called (This are set in the inspector, check there)");
                return;
            }

            draw.interactable = value;
            upgrade.interactable = value;
            ready.interactable = value;
        }
    }
}
