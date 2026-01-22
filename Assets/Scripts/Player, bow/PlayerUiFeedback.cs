using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUiFeedback : MonoBehaviour
{
    [SerializeField] Material matLineRenderer, matInvisible;
    [SerializeField] LineRenderer leftHandRayLineRend, rightHandRayLineRend;
    [SerializeField] SpriteRenderer fadeSprite;
    GameObject _fadeGo;
    [SerializeField] bool linesAlwaysVisible;
    AudioManager _audioManager;
    protected virtual void OnEnable()
    {
        leftHandRayLineRend.material = rightHandRayLineRend.material = linesAlwaysVisible ? matLineRenderer : matInvisible;
        
        _fadeGo = fadeSprite.gameObject;
        CallEv_FadeMethod(GenMove.Enter);
        Utils.FadeOut += CallEv_FadeMethod;
        
        _audioManager = AudioManager.Instance;
    }
    protected virtual void OnDisable()
    {
        Utils.FadeOut -= CallEv_FadeMethod;
    }

    void CallEv_FadeMethod(GenMove sceneTransition)
    {
        if (sceneTransition == GenMove.QuickToggle)
        {
            if (!linesAlwaysVisible) leftHandRayLineRend.material = rightHandRayLineRend.material = matInvisible;
            fadeSprite.color = Color.black;
            _fadeGo.SetActive(!_fadeGo.activeSelf);
            return;
        }
        
        Utils.Activation(_fadeGo, GenActivation.On);
        Color from = sceneTransition == GenMove.Enter ? Color.black : Color.clear;
        Color to = sceneTransition == GenMove.Enter ? Color.clear : Color.black;
        fadeSprite.DOColor(to, 2f)
            .From(from)
            .OnComplete(() =>
            {
                if (sceneTransition == GenMove.Enter)
                {
                    Utils.Activation(_fadeGo, GenActivation.Off);
                }
            });
    }

    
    public virtual void UiHoverLeft(bool hover)
    {
        if (linesAlwaysVisible) leftHandRayLineRend.material = matLineRenderer;
        else leftHandRayLineRend.material = hover ? matLineRenderer : matInvisible;
        
        if (_audioManager == null || !hover) return;
        _audioManager.PlaySfx(_audioManager.hoverEnter);
    }
    public virtual void UiHoverRight(bool hover)
    {
        if (linesAlwaysVisible) rightHandRayLineRend.material = matLineRenderer;
        else rightHandRayLineRend.material = hover ? matLineRenderer : matInvisible;
        
        if (_audioManager == null || !hover) return;
        _audioManager.PlaySfx(_audioManager.hoverEnter);
    }

    public void RemoveLines()
    {
        leftHandRayLineRend.material = rightHandRayLineRend.material = matInvisible;
    }


}
