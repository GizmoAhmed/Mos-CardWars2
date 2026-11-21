using DG.Tweening;
using TMPro;
using UnityEngine;

namespace PlayerStuff
{
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Floating Text")] [SerializeField]
        private TextMeshProUGUI floatingTextPrefab; // simple TMP text prefab

        [Tooltip("How long it takes for text to fade out")]
        [SerializeField] private float floatTextFadeOutDuration = 1.8f;
        
        [Tooltip("How high the text floats from the starting anchor")]
        [SerializeField] private float floatHeight = 80f;
        
        [Tooltip("How long the text takes to rise in seconds")]
        [SerializeField] private float floatDuration = 1.1f;

        [Header("Float Text Stacking")] [SerializeField]
        private float stackWindow = 0.5f; // seconds: spawns this close together stack

        [SerializeField] private float stackSpacing = 45f; // screen pixels between stacked texts

        private float _lastFloatTime = -10f;
        private int _floatStackIndex;
        

        private static RectTransform _effectsLayer;

        private static RectTransform EffectsLayer
        {
            get
            {
                if (_effectsLayer == null)
                    _effectsLayer = GameObject.FindWithTag("EffectsLayer")?.GetComponent<RectTransform>();

                if (_effectsLayer == null) // if still null throw error
                {
                    Debug.LogError(
                        "No EffectsLayer found, make sure the EffectsCanvas Object is tagged with 'EffectsLayer'");
                }

                return _effectsLayer;
            }
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
                punch: Vector3.one * 0.25f, // todo increase punch depending on amount buffed
                duration: 0.5f,
                vibrato: 1,
                elasticity: 0.6f
            );
        }

        public void FloatPlayerStatText(string text, Color textColor, Transform anchor)
        {
            if (anchor == null || floatingTextPrefab == null) return; 

            RectTransform parent = EffectsLayer;
            if (parent == null) return;

            Canvas canvas = parent.GetComponentInParent<Canvas>();   // the effects canvas, not a random one
            float scale = canvas != null ? canvas.scaleFactor : 1f;

            // instantiate text prefab, then set it with passed text and color
            TextMeshProUGUI t = Instantiate(floatingTextPrefab, parent);
            t.text = text;
            t.color = textColor;
            t.raycastTarget = false;
            t.transform.SetAsLastSibling();
            CanvasGroup cg = t.gameObject.AddComponent<CanvasGroup>();

            // World-space center of the stat object as it appears on screen
            Vector3[] corners = new Vector3[4];
            ((RectTransform)anchor).GetWorldCorners(corners);
            Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;

            // stacking (same as before)
            if (Time.time - _lastFloatTime < stackWindow) _floatStackIndex++;
            else _floatStackIndex = 0;
            _lastFloatTime = Time.time;

            float side = Random.Range(-0.5f, 0.5f);
            float offsetWorld = 90f * scale;
            float stackWorld = _floatStackIndex * stackSpacing * scale;
            float jitterX = Random.Range(-15f, 15f) * scale;
            float jitterY = Random.Range(-5f, 5f) * scale;

            Vector3 worldStart = worldCenter + new Vector3(side * offsetWorld + jitterX, stackWorld + jitterY, 0f);
            Vector3 start = parent.InverseTransformPoint(worldStart);

            RectTransform rt = (RectTransform)t.transform;
            rt.localPosition = start;
            rt.localScale = Vector3.zero;

            DOTween.Sequence()
                .SetDelay(_floatStackIndex * 0.15f)
                .Append(rt.DOScale(1f, 0.2f).SetEase(Ease.OutBack))
                .Join(rt.DOLocalMoveY(start.y + floatHeight, floatDuration).SetEase(Ease.OutCubic))
                .Insert(0.8f, cg.DOFade(0f, floatTextFadeOutDuration))
                .OnComplete(() => Destroy(t.gameObject))
                .SetLink(t.gameObject);
        }
    }
}