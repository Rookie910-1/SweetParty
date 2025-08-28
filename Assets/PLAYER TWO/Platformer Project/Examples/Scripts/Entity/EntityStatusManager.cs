
using UnityEngine;

public abstract class EntityStatusManager<T> : MonoBehaviour where T : EntityStatus<T>
{
    public T[] status;

    public T current { get; protected set; }

    public virtual void Change(int to)
    {
        if(to >=0 && to< status.Length)
        {
            if(current != status[to])
            {
                current = status[to];
            }
        }
    }

    protected void Start()
    {
        if(status.Length > 0)
        {
            current = status[0];
        }
    }
}

