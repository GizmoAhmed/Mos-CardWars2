using Mirror;
using PlayerStuff;
using UnityEngine;

namespace Buttons
{
    public class ConfirmDrawButton : MonoBehaviour
    {
        private DrawModal _drawModal;
        
        void Start()
        {
            _drawModal = transform.parent.parent.GetComponent<DrawModal>();

            if (_drawModal == null)
            {
                Debug.LogError($"Attempted to get draw modal for {gameObject.name} on {transform.parent.parent.name}, but it wasn't there");
            }
        }

        public void OnFreeDrawConfirm()
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            Player player = networkIdentity.GetComponent<Player>();
            PlayerStats stats = player.GetComponent<PlayerStats>();
            
            stats.CmdRequestFreeDraw();
        }
        
        public void OnPaidDrawConfirm()
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            Player player = networkIdentity.GetComponent<Player>();
            PlayerStats stats = player.GetComponent<PlayerStats>();

            // Debug.LogWarning($"{player.name} wants a PAID draw");
            
            stats.CmdRequestPaidDraw(
                _drawModal.paidChoice,
                _drawModal.paidOffer
            );
        }
    }
}
