using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FakeButtonUi : RequirementsUi, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image[] images;
    System.Action _onClick;
    [Space]
    [Title("Alternative setup - callback from inspector, not from code")]
    [SerializeField] bool assignInInspector;
    [ShowIf(nameof(assignInInspector))]
    [SerializeField] UnityEvent onClickUnityEvent;
    
    public void InitializeMe(System.Action onClick)
    {
        _onClick = onClick;
    }
    
    void OnEnable()
    {
        Utils.MainUiUnselect += CallEv_Unselect;
        Utils.ActivateOneArrayElement<Image>(images);
    }
    void OnDisable()
    {
        Utils.MainUiUnselect -= CallEv_Unselect;
    }
    void CallEv_Unselect()
    {
        Utils.ActivateOneArrayElement<Image>(images);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Utils.MainUiUnselect.Invoke();
        Utils.ActivateOneArrayElement<Image>(images, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CallEv_Unselect();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CheckRequirements()) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        Utils.ActivateOneArrayElement<Image>(images, 1);
        _onClick?.Invoke();
        onClickUnityEvent?.Invoke();
    }

    
}


