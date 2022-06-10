//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Inside"",
            ""id"": ""14b4a1f4-7dda-43e5-ad22-985f76528407"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""7055b0f5-e4ae-4744-8095-71f68b5efbf6"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""0e5620df-ba16-4ac0-8a6a-1b6406d445aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""PassThrough"",
                    ""id"": ""92843cad-3478-4267-952f-a07f3e00974a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""PassThrough"",
                    ""id"": ""417d33ed-a19f-436b-b151-acedddd33666"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""KeyboardLR"",
                    ""id"": ""ba177425-0473-4121-b77f-c014fc5622f9"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a2f63867-4532-4f3b-9046-8ce7ac2d9c37"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""da706b39-47c7-4393-b38f-db3f3ecdef1e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""JoystickLR"",
                    ""id"": ""ff5a339f-0a6e-4809-a810-5a46f8da3a54"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""208ad07b-a24a-4622-9c4b-ee4a1902b754"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""03682a75-ae1d-45c8-9258-82d7fc6cbe95"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c079b570-17fa-41d9-9fad-f89d0be654e5"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""99581a2d-00df-45a6-a31d-9aa5d9aeff41"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""459d7b8a-6ebe-4094-a6a0-a9831be09916"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""387a00f0-4554-4181-a79f-d8637e900919"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Inside
        m_Inside = asset.FindActionMap("Inside", throwIfNotFound: true);
        m_Inside_Move = m_Inside.FindAction("Move", throwIfNotFound: true);
        m_Inside_Jump = m_Inside.FindAction("Jump", throwIfNotFound: true);
        m_Inside_Aim = m_Inside.FindAction("Aim", throwIfNotFound: true);
        m_Inside_Shoot = m_Inside.FindAction("Shoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Inside
    private readonly InputActionMap m_Inside;
    private IInsideActions m_InsideActionsCallbackInterface;
    private readonly InputAction m_Inside_Move;
    private readonly InputAction m_Inside_Jump;
    private readonly InputAction m_Inside_Aim;
    private readonly InputAction m_Inside_Shoot;
    public struct InsideActions
    {
        private @PlayerControls m_Wrapper;
        public InsideActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Inside_Move;
        public InputAction @Jump => m_Wrapper.m_Inside_Jump;
        public InputAction @Aim => m_Wrapper.m_Inside_Aim;
        public InputAction @Shoot => m_Wrapper.m_Inside_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_Inside; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InsideActions set) { return set.Get(); }
        public void SetCallbacks(IInsideActions instance)
        {
            if (m_Wrapper.m_InsideActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_InsideActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_InsideActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_InsideActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_InsideActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_InsideActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_InsideActionsCallbackInterface.OnJump;
                @Aim.started -= m_Wrapper.m_InsideActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_InsideActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_InsideActionsCallbackInterface.OnAim;
                @Shoot.started -= m_Wrapper.m_InsideActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_InsideActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_InsideActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_InsideActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public InsideActions @Inside => new InsideActions(this);
    public interface IInsideActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}
