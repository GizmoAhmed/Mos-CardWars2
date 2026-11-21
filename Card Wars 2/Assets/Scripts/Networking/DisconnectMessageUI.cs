using TMPro;
using UnityEngine;

namespace Networking
{
    public class DisconnectMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;

        private void Start()
        {
            messageText = GetComponent<TextMeshProUGUI>();

            if (messageText == null)
            {
                Debug.LogError($"{gameObject.name}: messageText is null!");
            }

            string message = DisconnectNotice.Take();
            messageText.gameObject.SetActive(!string.IsNullOrEmpty(message));
            messageText.text = message;
        }
    }
}