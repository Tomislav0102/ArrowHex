using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotTrackerUi : MonoBehaviour
{
    GameObject[] _marks;
    Image[]  _win, _arrowMarks;
    bool _initialized;
    int _childCount;

    public void SetImages(int total, int wonSoFar, int arrowPos = 0, bool showArrow = true)
    {
        if (!_initialized)
        {
            _initialized = true;
            _childCount = transform.childCount;
            _marks = new GameObject[_childCount];
            _win = new Image[_childCount];
            _arrowMarks = new Image[_childCount];
            for (int i = 0; i < _childCount; i++)
            {
                _marks[i] = transform.GetChild(i).gameObject;
                _win[i] = _marks[i].transform.GetChild(0).GetComponent<Image>();
                _arrowMarks[i] = _marks[i].transform.GetChild(1).GetComponent<Image>();
            }
        }
        
        if (wonSoFar > _childCount) wonSoFar = _childCount;
        
        Utils.ActivateOneArrayElement(_marks);
        Utils.ActivateOneArrayElement(_win);
        Utils.ActivateOneArrayElement(_arrowMarks);
        for (int i = 0; i < total; i++)
        {
            Utils.Activation(_marks[i], GenActivation.On);
        }
        for (int i = 0; i < wonSoFar; i++)
        {
            Utils.Activation(_win[i], GenActivation.On);
        }
        if (showArrow) Utils.ActivateOneArrayElement(_arrowMarks, arrowPos);
    }
}
