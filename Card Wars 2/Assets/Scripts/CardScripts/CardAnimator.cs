using CardScripts.CardDisplays;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

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
            transform.DOKill();
            
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
        
        public void PopAnimate(Transform target, bool isBuff = true)
        {
            if (target == null)
            {
                Debug.LogWarning("PopAnimate: target is null!");
                return;
            }
    
            target.DOKill();
            target.localScale = Vector3.one;
    
            target.DOPunchScale(
                punch: Vector3.one * 0.75f, // todo increase punch depending on amount buffed
                duration: 0.5f,
                vibrato: 1,
                elasticity: 0.6f
            );
        }
        
        public void PulseBrightness(Image image, float duration = 0.6f, int loops = 1)
        {
            if (image == null) return;

            image.DOKill();
            Color original = image.color;
            Color bright = Color.Lerp(original, Color.white, 0.7f);

            Sequence pulse = DOTween.Sequence();
            pulse.Append(image.DOColor(bright, duration * 0.4f).SetEase(Ease.OutQuad));
            pulse.Append(image.DOColor(original, duration * 0.6f).SetEase(Ease.InQuad));
            pulse.SetLoops(loops);
            pulse.SetTarget(image);
        }
    }
}