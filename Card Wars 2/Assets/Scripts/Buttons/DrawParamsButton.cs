using Mirror;
using PlayerStuff;
using UnityEngine;

namespace Buttons
{
    public class DrawParamsButton : MonoBehaviour
    {
        private DrawModal _drawModal;

        public void InitButton(DrawModal d)
        {
            _drawModal = d;
        }

        public void IncreaseOffering()
        {
            _drawModal.UpdatePaidOffer(1);
        }

        public void DecreaseOffering()
        {
            _drawModal.UpdatePaidOffer(-1);
        }

        public void IncreaseChoice()
        {
            _drawModal.UpdatePaidChoice(1);
        }

        public void DecreaseChoice()
        {
            _drawModal.UpdatePaidChoice(-1);
        }
    }
}