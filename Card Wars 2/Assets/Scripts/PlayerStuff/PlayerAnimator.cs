using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
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
            punch: Vector3.one * 0.75f, // todo increase punch depending on amount buffed
            duration: 0.5f,
            vibrato: 1,
            elasticity: 0.6f
        );
    }
}
