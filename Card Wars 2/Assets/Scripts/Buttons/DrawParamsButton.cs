using Mirror;
using PlayerStuff;
using UnityEngine;

namespace Buttons
{
    public class DrawParamsButton : NetworkBehaviour
    {
        private DrawModal _drawModal;
        
        public void InitButton(DrawModal d)
        {
            _drawModal = d;
        }

        public void IncreaseOffering()
        {
            CmdUpdateOffer(1);
        }

        public void DecreaseOffering()
        {
            // if (CanChangeSelectionParams) return;

            CmdUpdateOffer(-1);
        }

        public void IncreaseChoice()
        {
            // if (!CanChangeSelectionParams) return;

            CmdUpdateChoice(1);
        }

        public void DecreaseChoice()
        {
            // can't have zero choices
            // if (_player.cardsChosen == 1) return;

            CmdUpdateChoice(-1);
        }

        [Command]
        private void CmdUpdateChoice(int i)
        {
            if (i == 0)
            {
                Debug.LogError("Didn't specify increase or decrease in choice");
                return;
            }
            
            // _player.cardsChosen += i;
        }
        
        [Command]
        private void CmdUpdateOffer(int i)
        {
            if (i == 0)
            {
                Debug.LogError("Didn't specify increase or decrease in choice");
                return;
            }
            
            //_player.cardsOffered += i;
        }
    }
}