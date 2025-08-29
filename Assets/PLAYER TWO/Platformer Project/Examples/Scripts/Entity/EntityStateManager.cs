using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class EntityStateManager : MonoBehaviour
{
    public EntityStateManagerEvents events;
}
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    protected List<EntityState<T>> m_list=new List<EntityState<T>>();

    protected Dictionary<Type,EntityState<T>> m_states=new Dictionary<Type,EntityState<T>>();

    protected abstract List<EntityState<T>> GetStatelist();

    public EntityState<T> current { get; protected set; }

    public T entity { get; protected set; }

    public EntityState<T> last { get; protected set; }

    

    public int lastIndex => m_list.IndexOf(last);

    public int index => m_list.IndexOf(current);

    protected virtual void InitializeEntity() => entity=GetComponent<T>();

    protected virtual void InitializeStates()
    {
        m_list = GetStatelist();

        foreach(var state in m_list)
        {
            var type = state.GetType();
            if(!m_states.ContainsKey(type))
            {
                m_states.Add(type, state);
            }
        }

        if(m_list.Count > 0)
        {
            current = m_list[0];
        }
    }

    protected void Start()
    {
        InitializeEntity();
        InitializeStates();
    }

    public virtual void Step()
    {
        if(current != null && Time.timeScale > 0)
        {
            current.Step(entity);
        }
    }

    public virtual void Change<TState>() where TState : EntityState<T>
    {
        var type = typeof(TState);
        if(m_states.ContainsKey(type))
        {
            Change(m_states[type]);
            Debug.Log(type.Name);
        }
    }

    public virtual void Change(EntityState<T> to)
    {
        if(null != to && Time.timeScale>0)
        { 
            if(current !=null)
            {
                current.Exit(entity);
                last = current;
            }

            current = to;
            current.Enter(entity);
        }
    }
}

