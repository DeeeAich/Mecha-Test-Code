using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jacob was here

public enum ButtonAction
{
    ShootRight,
    ShootLeft,
    ReloadRight,
    ReloadLeft,
    Move,
    Look,
    Dash,
    Ult,
    Interact,
    OpenInventory,
    Pause,
    UiNavigation,
    UiEnter,
    UiExit
}
public enum InputDevice
{
    MouseAndKeyboard,
    XboxController,
    PlaystationController
}


public class InputDeviceManager : MonoBehaviour
{
    private static InputDeviceManager Instance;

    public static InputDeviceManager instance
    {
        get
        {
            if(Instance != null)
            {
                return Instance;
            }
            else
            {
                InputDeviceManager[] managers = FindObjectsOfType<InputDeviceManager>();
                foreach (InputDeviceManager l in managers)
                {
                    
                        if (l.gameObject.TryGetComponent<InputDeviceManager>(out Instance))
                        {
                        return Instance;
                        }
                }
                return Instance;
            }
        }
        private set
        {
            if(Instance!=null)
            Instance = value;
        }
    }
    public InputDevice currentInputDevice;


    [Header("ShootRight")]
    [SerializeField] public List<Sprite> ShootRight;

    [Header("ShootLeft")]
    public List<Sprite> ShootLeft;

    [Header("ReloadRight")]
    public List<Sprite> ReloadRight;

    [Header("ReloadLeft")]
    public List<Sprite> ReloadLeft;

    [Header("Move")]
    public List<Sprite> Move;

    [Header("Look")]
    public List<Sprite> Look;

    [Header("Dash")]
    public List<Sprite> Dash;

    [Header("Ult")]
    public List<Sprite> Ult;

    [Header("Interact")]
    public List<Sprite> Interact;

    [Header("OpenInventory")]
    public List<Sprite> OpenInventory;

    [Header("Pause")]
    public List<Sprite> Pause;

    [Header("UiNavigation")]
    public List<Sprite> UiNavigation;

    [Header("UiEnter")]
    public List<Sprite> UiEnter;

    [Header("UiExit")]
    public List<Sprite> UiExit;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            transform.parent = null;
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool test;
    private void Update()
    {
        if (test)
        {
            test = false;
            UpdateInputDevice(currentInputDevice);
        }
    }

    public void UpdateInputDevice(InputDevice inputDevice)
    {
        currentInputDevice = inputDevice;
        foreach (InputDeviceButtonPrompt inputDeviceButtonPrompt in GameObject.FindObjectsOfType<InputDeviceButtonPrompt>())
        {
            inputDeviceButtonPrompt.UpdateButton();
        }
    }
}
