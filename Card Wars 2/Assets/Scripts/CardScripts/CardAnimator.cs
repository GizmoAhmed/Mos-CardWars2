using CardScripts.CardDisplays;
using DG.Tweening;
using Mirror;
using TMPro;
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
        
        [Header("Floating Text")]
        [SerializeField] private TextMeshProUGUI popUpTextPrefab;   // simple TMP text prefab
        [SerializeField] private float fadeOutDuration = 0.6f;
        
        [Header("Roll Chance Colors")]
        [SerializeField] private Color hitColor  = new Color32(80, 220, 120, 255);
        [SerializeField] private Color missColor = new Color32(230, 80, 80, 255);
        
        [Header("Stat Colors")] // todo CreatureStats decides the colors right now, card anim should use these intead of a passed color param
        [SerializeField] private Color strengthColor  = new Color32(255,77,121, 255);
        [SerializeField] private Color defenseColor = new Color32(230, 80, 80, 255);
        
        private static RectTransform _effectsLayer;

        private static RectTransform EffectsLayer
        {
            get
            {
                if (_effectsLayer == null)
                    _effectsLayer = GameObject.FindWithTag("EffectsLayer")?.GetComponent<RectTransform>();

                if (_effectsLayer == null) // if still null throw error
                {
                    Debug.LogError("No EffectsLayer found, make sure the EffectsCanvas Object is tagged with 'EffectsLayer'");
                }

                return _effectsLayer;
            }
        }
        
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
        
        public void PopTextAnimate(Transform target, bool isBuff = true)
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
        
        public void FadeOutThen(CanvasGroup canvasGroup, System.Action onFaded, float duration = 1.1f)
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
        
        [Server]
        public void Server_ShowRollResult(bool hit)
        {
            RpcShowRollResult(hit);
        }

        [ClientRpc]
        private void RpcShowRollResult(bool hit)
        {
            string text = hit ? $"Hit!" : $"Missed!";
            FloatText(text, hit ? hitColor : missColor);
        }

        [ClientRpc]
        public void FloatStatText(string text, Color color)
        {
            FloatText(text, color);
        }

        /// <summary>
        /// Makes text appear for a moment and float upwards to indicate something happened
        /// </summary>
        /// <param name="text">Text to Show</param>
        /// <param name="textColor">Color of text</param>
        private void FloatText(string text, Color textColor)
        {
            // get root GameCanvas
            Canvas canvas = GetComponentInParent<Canvas>()?.rootCanvas;
            if (canvas == null || popUpTextPrefab == null) return;

            // Parent to the effect canvas root so the card's own fade, layout group, or clipping can't affect it
            Transform parent = EffectsLayer != null ? EffectsLayer : canvas.transform;
            TextMeshProUGUI t = Instantiate(popUpTextPrefab, parent);
            t.text = text;
            t.color = textColor;
            t.raycastTarget = false;
            t.transform.SetAsLastSibling();

            CanvasGroup cg = t.gameObject.AddComponent<CanvasGroup>();

            // Appear beside the card, alternating sides so back-to-back rolls don't overlap exactly
            float side = Random.value < 0.5f ? -1f : 1f;
            RectTransform cardRect = (RectTransform)transform;
            Vector3 start = canvas.transform.InverseTransformPoint(cardRect.position)
                            + new Vector3(side * cardRect.rect.width * 0.7f, 0f, 0f);

            RectTransform rt = (RectTransform)t.transform;
            rt.localPosition = start;
            rt.localScale = Vector3.zero;

            DOTween.Sequence()
                .Append(rt.DOScale(1f, 0.2f).SetEase(Ease.OutBack))           // pop in
                .Join(rt.DOLocalMoveY(start.y + 80f, 1.1f).SetEase(Ease.OutCubic)) // drift up
                .Insert(0.8f, cg.DOFade(0f, fadeOutDuration))                             // fade near the end
                .OnComplete(() => Destroy(t.gameObject))
                .SetLink(t.gameObject);
        }

        /// <summary>
        /// Draws a line between a used spell, and it's target, shows on the other player, since this player already knows where it's place.
        /// It's really for the person viewing the other player use a spell
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="grow"></param>
        /// <param name="hold"></param>
        /// <param name="fade"></param>
        public void ShowTargetLine(Transform from, Transform to, float grow = 0.25f, float hold = 0.8f, float fade = 0.4f)
        {
            // Prefer the effects layer, fall back to the root canvas if it isn't set up
            RectTransform parent = EffectsLayer != null
                ? EffectsLayer
                : (RectTransform)from.GetComponentInParent<Canvas>()?.rootCanvas.transform;

            if (parent == null) return;

            // Convert into the PARENT's space (the effects layer), not the root canvas
            Vector2 a = parent.InverseTransformPoint(from.position);
            Vector2 b = parent.InverseTransformPoint(to.position);
            Vector2 dir = b - a;
            float length = dir.magnitude;
            if (length < 1f) return;

            GameObject go = new GameObject("SpellTargetLine", typeof(RectTransform), typeof(CanvasGroup), typeof(UnityEngine.UI.Image));
            RectTransform rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);    
            rt.SetAsLastSibling();

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