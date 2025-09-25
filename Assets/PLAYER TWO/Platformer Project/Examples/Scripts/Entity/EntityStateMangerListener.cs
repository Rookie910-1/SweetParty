using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("PLAYER TWO/Platformer Project/Examples/Scripts/Entity/EntityStateMangerListener")]
public class EntityStateMangerListener :MonoBehaviour
{
    public UnityEvent onEnter;
    
    public UnityEvent onExit;
    
    public List<string> states;
    
    public EntityStateManager m_manager { get; protected set; }
    protected void Start()
    {
        if (!m_manager)
            m_manager = GetComponentInParent<EntityStateManager>();
        
        m_manager.events.onEnter.AddListener(OnEnter);
        m_manager.events.onExit.AddListener(OnExit);
    }

    protected virtual void OnEnter(Type state)
    {
        if(states.Contains(state.Name))
            onEnter?.Invoke();
    }
    
    protected virtual void OnExit(Type state)
    {
        if(states.Contains(state.Name))
            onExit?.Invoke();
    }
}
