// GENERATED AUTOMATICALLY FROM 'Assets/Offline Mode/Input/InputManager.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputManager : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""PlayerControlls"",
            ""id"": ""ea5dcec3-1f7d-45ea-a28a-fb0edc38e908"",
            ""actions"": [
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""3ecc8b66-f569-4fcc-872d-e656681c8e50"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AltShoot"",
                    ""type"": ""Button"",
                    ""id"": ""1dc0859b-6c72-4ed7-af30-6841ca90c00c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BombShoot"",
                    ""type"": ""Button"",
                    ""id"": ""9a2618ca-7dd5-47db-943b-b4e36eae7be5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""c0e52332-c465-4d26-b070-eda52794e2f1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpawnPlayer"",
                    ""type"": ""Button"",
                    ""id"": ""0f81fabb-bb5e-4ed6-a85f-6ba9b387eab4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dfe1dedb-1b80-4a4c-a22f-c2e923963dc8"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e475bc6f-739d-415b-9a14-705d4db2cdc5"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""AltShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""ec0d39f6-0c92-451b-8c0e-d24b9b6c5b24"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ee3ec266-00da-4fb8-bb20-010b8a10d9b8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ecb91222-671c-4629-bbd2-eb900f8c052f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6d9ec9f8-fb47-4848-9c68-171bad692228"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4a03ad1d-7472-444c-986b-a212eba34af6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9030427a-804b-448e-bb00-16cff2da8b82"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d737707-48da-4078-b6d8-2abf64c2d424"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48f84b77-3729-4d9d-8dda-0d7a6cc0f2e5"",
                    ""path"": ""<Keyboard>/numpad0"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""effea6d8-7d28-45ee-a24b-25dd4b39dca6"",
                    ""path"": ""<Keyboard>/pageDown"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""AltShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""170cbce7-01a2-4b70-bab0-f91baeac723b"",
                    ""path"": ""<Keyboard>/rightShift"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""AltShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c78624fa-73b8-4d00-a4b1-6a7ea3d1d5c3"",
                    ""path"": ""<Keyboard>/numpadEnter"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""AltShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""ArrowKeys"",
                    ""id"": ""e9a5be94-9ce0-4e3d-9a08-34c074cb36f6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c894b2f1-6a74-49d5-b1ca-fe676a2e5c60"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f7c879f6-2b97-452e-947f-8b2be7110f96"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""430f3870-eafb-4ba3-9db2-6404a36ea37d"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8a9a371d-7ac4-4dae-8288-213b7e4a2073"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""IJKL"",
                    ""id"": ""506a2a9c-882d-451c-8f0c-d209d1ec3e8a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""83bd7602-4b65-45a5-9502-c73393fadf81"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""17049ea8-3a4b-4eb6-960a-0a47dffc225b"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""927b81d3-9fc0-4242-9dcd-6224d0b09e2a"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""165a1506-798d-49b4-85f0-01d3574a3441"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""NumPad 8456"",
                    ""id"": ""b79e4278-4041-4b35-9a3b-e787f75902a6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5b8537b1-8fcb-48ce-ac4c-f8bacf4f7eba"",
                    ""path"": ""<Keyboard>/numpad8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""553ee5a2-5a8f-4e2d-804f-7ef029a0eff5"",
                    ""path"": ""<Keyboard>/numpad5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7bff5c09-2b09-4c22-8135-7e102a21c7c1"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""db687ba8-c238-4401-a5ff-6e437f544a20"",
                    ""path"": ""<Keyboard>/numpad6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fb0e2fea-779c-4cdb-a0c7-ba38c81251aa"",
                    ""path"": ""<Keyboard>/numpadPlus"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""SpawnPlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c60a97ee-2006-4793-bcc7-fed1f567d134"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player1"",
                    ""action"": ""BombShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56c5f671-2093-4df6-a3b3-d12f39d799f6"",
                    ""path"": ""<Keyboard>/end"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player2"",
                    ""action"": ""BombShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6be9ab0d-2f42-4749-916f-4937f63e9588"",
                    ""path"": ""<Keyboard>/rightCtrl"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player3"",
                    ""action"": ""BombShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76c21b46-110e-441f-acde-af409dac45cd"",
                    ""path"": ""<Keyboard>/numpadPeriod"",
                    ""interactions"": ""Press(behavior=2),Hold"",
                    ""processors"": """",
                    ""groups"": ""Player4"",
                    ""action"": ""BombShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Player1"",
            ""bindingGroup"": ""Player1"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Player2"",
            ""bindingGroup"": ""Player2"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Player3"",
            ""bindingGroup"": ""Player3"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Player4"",
            ""bindingGroup"": ""Player4"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerControlls
        m_PlayerControlls = asset.FindActionMap("PlayerControlls", throwIfNotFound: true);
        m_PlayerControlls_Shoot = m_PlayerControlls.FindAction("Shoot", throwIfNotFound: true);
        m_PlayerControlls_AltShoot = m_PlayerControlls.FindAction("AltShoot", throwIfNotFound: true);
        m_PlayerControlls_BombShoot = m_PlayerControlls.FindAction("BombShoot", throwIfNotFound: true);
        m_PlayerControlls_Movement = m_PlayerControlls.FindAction("Movement", throwIfNotFound: true);
        m_PlayerControlls_SpawnPlayer = m_PlayerControlls.FindAction("SpawnPlayer", throwIfNotFound: true);
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

    // PlayerControlls
    private readonly InputActionMap m_PlayerControlls;
    private IPlayerControllsActions m_PlayerControllsActionsCallbackInterface;
    private readonly InputAction m_PlayerControlls_Shoot;
    private readonly InputAction m_PlayerControlls_AltShoot;
    private readonly InputAction m_PlayerControlls_BombShoot;
    private readonly InputAction m_PlayerControlls_Movement;
    private readonly InputAction m_PlayerControlls_SpawnPlayer;
    public struct PlayerControllsActions
    {
        private @InputManager m_Wrapper;
        public PlayerControllsActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shoot => m_Wrapper.m_PlayerControlls_Shoot;
        public InputAction @AltShoot => m_Wrapper.m_PlayerControlls_AltShoot;
        public InputAction @BombShoot => m_Wrapper.m_PlayerControlls_BombShoot;
        public InputAction @Movement => m_Wrapper.m_PlayerControlls_Movement;
        public InputAction @SpawnPlayer => m_Wrapper.m_PlayerControlls_SpawnPlayer;
        public InputActionMap Get() { return m_Wrapper.m_PlayerControlls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerControllsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerControllsActions instance)
        {
            if (m_Wrapper.m_PlayerControllsActionsCallbackInterface != null)
            {
                @Shoot.started -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnShoot;
                @AltShoot.started -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnAltShoot;
                @AltShoot.performed -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnAltShoot;
                @AltShoot.canceled -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnAltShoot;
                @BombShoot.started -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnBombShoot;
                @BombShoot.performed -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnBombShoot;
                @BombShoot.canceled -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnBombShoot;
                @Movement.started -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnMovement;
                @SpawnPlayer.started -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnSpawnPlayer;
                @SpawnPlayer.performed -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnSpawnPlayer;
                @SpawnPlayer.canceled -= m_Wrapper.m_PlayerControllsActionsCallbackInterface.OnSpawnPlayer;
            }
            m_Wrapper.m_PlayerControllsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @AltShoot.started += instance.OnAltShoot;
                @AltShoot.performed += instance.OnAltShoot;
                @AltShoot.canceled += instance.OnAltShoot;
                @BombShoot.started += instance.OnBombShoot;
                @BombShoot.performed += instance.OnBombShoot;
                @BombShoot.canceled += instance.OnBombShoot;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @SpawnPlayer.started += instance.OnSpawnPlayer;
                @SpawnPlayer.performed += instance.OnSpawnPlayer;
                @SpawnPlayer.canceled += instance.OnSpawnPlayer;
            }
        }
    }
    public PlayerControllsActions @PlayerControlls => new PlayerControllsActions(this);
    private int m_Player1SchemeIndex = -1;
    public InputControlScheme Player1Scheme
    {
        get
        {
            if (m_Player1SchemeIndex == -1) m_Player1SchemeIndex = asset.FindControlSchemeIndex("Player1");
            return asset.controlSchemes[m_Player1SchemeIndex];
        }
    }
    private int m_Player2SchemeIndex = -1;
    public InputControlScheme Player2Scheme
    {
        get
        {
            if (m_Player2SchemeIndex == -1) m_Player2SchemeIndex = asset.FindControlSchemeIndex("Player2");
            return asset.controlSchemes[m_Player2SchemeIndex];
        }
    }
    private int m_Player3SchemeIndex = -1;
    public InputControlScheme Player3Scheme
    {
        get
        {
            if (m_Player3SchemeIndex == -1) m_Player3SchemeIndex = asset.FindControlSchemeIndex("Player3");
            return asset.controlSchemes[m_Player3SchemeIndex];
        }
    }
    private int m_Player4SchemeIndex = -1;
    public InputControlScheme Player4Scheme
    {
        get
        {
            if (m_Player4SchemeIndex == -1) m_Player4SchemeIndex = asset.FindControlSchemeIndex("Player4");
            return asset.controlSchemes[m_Player4SchemeIndex];
        }
    }
    public interface IPlayerControllsActions
    {
        void OnShoot(InputAction.CallbackContext context);
        void OnAltShoot(InputAction.CallbackContext context);
        void OnBombShoot(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnSpawnPlayer(InputAction.CallbackContext context);
    }
}
