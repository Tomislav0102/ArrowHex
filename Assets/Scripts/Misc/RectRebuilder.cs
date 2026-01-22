using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectRebuilder : MonoBehaviour
{
   // [SerializeField] RectTransform[] rectTransforms;
    RectTransform _myRect;

    void Awake()
    {
        _myRect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StartCoroutine(Refresh());
    }


    IEnumerator Refresh()
    {
        yield return null;
        if (_myRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(_myRect);
        // for (int i = 0; i < rectTransforms.Length; i++)
        // {
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransforms[i]);
        // }

    }

}
