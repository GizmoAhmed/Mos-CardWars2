using CardScripts.CardDisplays;
using DG.Tweening;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    public class CardAnimator : NetworkBehaviour
    {
        private RectTransform _rectTransform;
        
        private CardDisplay _cardDisplay;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _cardDisplay = GetComponent<CardDisplay>();
        }

        [Server] // called from an execution
        public void JiggleCard()
        {
            RpcJiggleCard();
        }

        [ClientRpc] // jiggle on both clients
        private void RpcJiggleCard()
        {
            // Sequence for jiggle animation
            Sequence jiggleSequence = DOTween.Sequence();

            jiggleSequence.Append(_rectTransform.DOShakeAnchorPos(
                duration: 0.5f, 
                strength: 6f, 
                vibrato: 10,
                randomness: 90, 
                snapping: false, 
                fadeOut: true));

            // Debug.Log($"<color=green>{this}</color> on {gameObject.name} just jiggled");
        }
    }
}