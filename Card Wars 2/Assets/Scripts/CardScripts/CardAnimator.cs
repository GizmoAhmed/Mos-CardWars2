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
        
        [Header("Spell Target Line")]
        [SerializeField] private Color lineColor = new Color(1f, 0.85f, 0.3f, 0.9f);
        [SerializeField] private float lineThickness = 6f;

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
        
        public void FadeOutThen(CanvasGroup canvasGroup, System.Action onFaded, float duration = 0.8f)
        {
            canvasGroup.DOKill();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;   // can't click or drag a dying card

            canvasGroup.DOFade(0f, duration)
                .SetEase(Ease.InQuad)
                .SetLink(canvasGroup.gameObject)  // auto-kills if the object is destroyed
                .OnComplete(() =>
                {
                    onFaded?.Invoke();            // move to discard board while invisible
                    canvasGroup.alpha = 1f;       // restore so it's visible on the discard board
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                });
        }

        public void ShowTargetLine(Transform from, Transform to, float grow = 0.25f, float hold = 0.8f, float fade = 0.4f)
        {
            Canvas canvas = from.GetComponentInParent<Canvas>()?.rootCanvas;
            if (canvas == null) return;

            RectTransform canvasRect = (RectTransform)canvas.transform;

            // Works for Overlay, Camera, and World Space canvases
            Vector2 a = canvasRect.InverseTransformPoint(from.position);
            Vector2 b = canvasRect.InverseTransformPoint(to.position);
            Vector2 dir = b - a;
            float length = dir.magnitude;
            if (length < 1f) return;

            GameObject go = new GameObject("SpellTargetLine", typeof(RectTransform), typeof(CanvasGroup), typeof(UnityEngine.UI.Image));
            RectTransform rt = (RectTransform)go.transform;
            rt.SetParent(canvasRect, false);
            rt.SetAsLastSibling(); // draw on top

            var img = go.GetComponent<UnityEngine.UI.Image>();
            img.color = lineColor;
            img.raycastTarget = false;

            rt.pivot = new Vector2(0f, 0.5f);          // grow outward from the spell
            rt.localPosition = a;
            rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            rt.sizeDelta = new Vector2(0f, lineThickness);

            CanvasGroup cg = go.GetComponent<CanvasGroup>();

            DOTween.Sequence()
                .Append(rt.DOSizeDelta(new Vector2(length, lineThickness), grow).SetEase(Ease.OutQuad))
                .AppendInterval(hold)
                .Append(cg.DOFade(0f, fade))
                .OnComplete(() => Destroy(go))
                .SetLink(go);
        }
    }
}