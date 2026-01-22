using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerRigRef : PlayerUiFeedback
{
    public Transform root, head, leftHand, rightHand;
    public Camera mainCam;
    [HideInInspector] public Transform camMainTransform;
    public ActionBasedController leftController;
    public ActionBasedController rightController;
    public PlayerGame playerGame;

    void Awake()
    {
        camMainTransform = mainCam.transform;
    }

    public override void UiHoverLeft(bool hover)
    {
        if (playerGame == null || playerGame.shootingCurrent.controllerPullingString) return;
        base.UiHoverLeft(hover);
    }

    public override void UiHoverRight(bool hover)
    {
        if (playerGame == null || playerGame.shootingCurrent.controllerPullingString) return;
        base.UiHoverRight(hover);
    }



}
