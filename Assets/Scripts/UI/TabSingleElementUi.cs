using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabSingleElementUi : RequirementsUi, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    System.Action _onClick;
    [SerializeField] Image[] images;

    public void Init(System.Action onClick)
    {
        UnSelected();
        _onClick = onClick;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (images[1].enabled) return;
        Utils.ActivateOneArrayElement(images, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (images[1].enabled) return;
        UnSelected();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CheckRequirements()) return;
        
        AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        Clicked();
    }

    public void Clicked()
    {
        _onClick?.Invoke();
        Utils.ActivateOneArrayElement(images, 1);
    }
    
    public void UnSelected()
    {
        Utils.ActivateOneArrayElement(images);
    }

    
}
