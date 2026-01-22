using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CampDescriptionUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    MainMenuManager _mm;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform contentRt;
    const float CONST_Scale = 0.4f;
    Vector3[] _imgPositions, _namePositions;
    bool _initialized;
    const float CONST_TweenSpeed = 3f;
    TweenParams _tweenParams;
    void Awake()
    {
        _mm = MainMenuManager.Instance;
    }

    void Start()
    {
        _imgPositions = new Vector3[2];
        _namePositions = new Vector3[2];
        for (int i = 0; i < 2; i++)
        {
            _imgPositions[i] = _mm.focusImages[i].position;
            _namePositions[i] = _mm.focusNames[i].position;
        }
        _initialized = true;
        _tweenParams = new TweenParams().SetSpeedBased(true).SetEase(Ease.Linear);
    }
    

    void OnEnable()
    {
        Focused(GenActivation.Off);
        StartCoroutine(SortVerticalLayout());
    }

    IEnumerator SortVerticalLayout()
    {
        contentRt.GetComponent<VerticalLayoutGroup>().enabled = false;
        yield return new WaitForEndOfFrame();
        contentRt.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Focused(GenActivation.On);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Focused(GenActivation.Off);
    }

    void Focused(GenActivation activation)
    {
        if (!_initialized) return;
        
        scrollRect.enabled = activation == GenActivation.On;
        
        Vector3 imgTarget = _imgPositions[activation == GenActivation.On ? 1 : 0];
        Vector3 nameTarget = _namePositions[activation == GenActivation.On ? 1 : 0];
        Vector2 imgScaleTarget = activation == GenActivation.On ? CONST_Scale * Vector3.one : Vector3.one;

        _mm.focusImages[0].DOMove(imgTarget, CONST_TweenSpeed).SetAs(_tweenParams);
        _mm.focusImages[0].DOScale(imgScaleTarget, CONST_TweenSpeed).SetAs(_tweenParams);
        _mm.focusNames[0].DOMove(nameTarget, CONST_TweenSpeed).SetAs(_tweenParams);

        if (activation == GenActivation.Off) contentRt.anchoredPosition = Vector2.zero;
    }
}
