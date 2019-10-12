// GENERATED AUTOMATICALLY FROM 'Assets/Controls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class Controls : InputActionAssetReference
{
    public Controls()
    {
    }
    public Controls(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_Degroup = m_Player.GetAction("Degroup");
        m_Player_Move = m_Player.GetAction("Move");
        m_Player_Reset = m_Player.GetAction("Reset");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player = null;
        m_Player_Degroup = null;
        m_Player_Move = null;
        m_Player_Reset = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Player
    private InputActionMap m_Player;
    private InputAction m_Player_Degroup;
    private InputAction m_Player_Move;
    private InputAction m_Player_Reset;
    public struct PlayerActions
    {
        private Controls m_Wrapper;
        public PlayerActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Degroup { get { return m_Wrapper.m_Player_Degroup; } }
        public InputAction @Move { get { return m_Wrapper.m_Player_Move; } }
        public InputAction @Reset { get { return m_Wrapper.m_Player_Reset; } }
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
    }
    public PlayerActions @Player
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new PlayerActions(this);
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get

        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.GetControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
}
