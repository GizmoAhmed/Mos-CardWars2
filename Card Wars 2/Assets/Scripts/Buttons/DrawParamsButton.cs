using Mirror;
using PlayerStuff;
using UnityEngine;

namespace Buttons
{
    public class DrawParamsButton : NetworkBehaviour
    {
        // private PlayerStats _player;
        
        // if increasing or decreasing choice or offer respectively to where,
        // offer <= choice, don't do it
        /*private bool CanChangeSelectionParams => !(_player.cardsOffered - 1 <= _player.cardsChosen);

        public void Init(PlayerStats player)
        {
            _player = player;
            
            if (_player == null)
            {
                Debug.LogError("Tried to init with a PlayerStats, but it was null");
            }
        }*/

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