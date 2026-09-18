using DG.Tweening;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    public class CardAnimator : NetworkBehaviour
    {
        private RectTransform _rectTransform;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        [Server] // called from an execution
        public void Jiggle()
        {
            RpcJiggle();
        }

        [ClientRpc] // jiggle on both clients
        private void RpcJiggle()
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

            Debug.Log($"<color=green>{this}</color> on {gameObject.name} just jiggled");
        }
    }
}