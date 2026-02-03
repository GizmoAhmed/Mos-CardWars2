using Mirror;
using PlayerStuff;
using UnityEngine;

namespace Buttons
{
    public class DrawButton : NetworkBehaviour
    {
        public GameObject drawModalObj;

        public void InitDrawModal(GameObject d)
        {
            drawModalObj = d;

            // NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            // _player = networkIdentity.GetComponent<Player>();
        }

        public void ToggleDrawModal()
        {
            if (drawModalObj == null)
            {
                Debug.LogError("DrawModal is null, must've not been set in the GameManager");
                return;
            }

            if (!drawModalObj.activeInHierarchy) drawModalObj.GetComponent<DrawModal>().OnOpen();
            else                                drawModalObj.GetComponent<DrawModal>().OnClose();
        }

        public void DrawCardOnClick()
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            Player player = networkIdentity.GetComponent<Player>();
            
            player.deckCollection.CmdDrawCard();
        }
    }
}