using CardScripts;
using Mirror;
using Modal;
using UnityEngine;

namespace Buttons
{
    public class DrawButton : NetworkBehaviour
    {
        public GameObject drawModalObj;
        
        private CardInfoHandler _cardInfoHandler;

        public void InitDrawModal(GameObject d)
        {
            drawModalObj = d;
            
            _cardInfoHandler = FindObjectOfType<CardInfoHandler>();
        }

        public void ToggleDrawModal()
        {
            if (drawModalObj == null)
            {
                Debug.LogError("DrawModal is null, must've not been set in the GameManager");
                return;
            }

            if (!drawModalObj.activeInHierarchy) // if not active...
            {
                drawModalObj.GetComponent<DrawModal>().OpenDrawModal(); // open
            }
            
            _cardInfoHandler.CloseAnyOpenInfo();
        }

        public void DrawCardOnClick()
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            Player player = networkIdentity.GetComponent<Player>();
            
            player.deckCollection.CmdDrawCard();
        }
        
        public void DrawAllOnClick()
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            Player player = networkIdentity.GetComponent<Player>();
            
            player.deckCollection.CmdDrawAllFromDeck();
        }
    }
}