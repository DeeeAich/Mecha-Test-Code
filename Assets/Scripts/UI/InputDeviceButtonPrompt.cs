using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Jacob was here

public class InputDeviceButtonPrompt : MonoBehaviour
{

    public ButtonAction buttonAction;
    public Image buttomImage;

    private void OnEnable()
    {
        UpdateButton();
    }

    private void Start()
    {
        UpdateButton();
    }

    public void UpdateButton()
    {
        if (buttomImage == null)
        {
            buttomImage = GetComponent<Image>();
        }

        if (buttomImage.preserveAspect == false)
        {
            buttomImage.preserveAspect = true;
        }
        switch (buttonAction)
        {

            case ButtonAction.ShootRight:
                SetImage(InputDeviceManager.instance.ShootRight);
                break;

            case ButtonAction.ShootLeft:
                SetImage(InputDeviceManager.instance.ShootLeft);
                break;

            case ButtonAction.ReloadRight:
                SetImage(InputDeviceManager.instance.ReloadRight);
                break;

            case ButtonAction.ReloadLeft:
                SetImage(InputDeviceManager.instance.ReloadRight);
                break;

            case ButtonAction.Move:
                SetImage(InputDeviceManager.instance.Move);
                break;

            case ButtonAction.Look:
                SetImage(InputDeviceManager.instance.Look);
                break;

            case ButtonAction.Dash:
                SetImage(InputDeviceManager.instance.Dash);
                break;

            case ButtonAction.Ult:
                SetImage(InputDeviceManager.instance.Ult);
                break;

            case ButtonAction.Interact:
                SetImage(InputDeviceManager.instance.Interact);
                break;

            case ButtonAction.OpenInventory:
                SetImage(InputDeviceManager.instance.OpenInventory);
                break;

            case ButtonAction.UiNavigation:
                SetImage(InputDeviceManager.instance.UiNavigation);
                break;

            case ButtonAction.UiEnter:
                SetImage(InputDeviceManager.instance.UiEnter);
                break;

            case ButtonAction.UiExit:
                SetImage(InputDeviceManager.instance.UiExit);
                break;
        }
    }


    void SetImage(List<Sprite> buttonImages)
    {
        switch (InputDeviceManager.instance.currentInputDevice)
        {
            case InputDevice.MouseAndKeyboard:
                buttomImage.sprite = buttonImages[0];
                break;

            case InputDevice.XboxController:
                buttomImage.sprite = buttonImages[1];
                break;

            case InputDevice.PlaystationController:
                buttomImage.sprite = buttonImages[2];
                break;
        }
    }

}
