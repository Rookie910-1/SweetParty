
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Examples/Scripts/Misc/Breakable")]
public class Breakable :MonoBehaviour
{
    public GameObject display;

    public AudioClip clip;
    
    public UnityEvent OnBreak;
    
    protected Collider m_collider;
    
    protected AudioSource m_audio;
    
    protected Rigidbody m_rigidbody;
    
    public bool broken { get; protected set; }

    public virtual void Break()
    {
        if (!broken)
        {
            //如果有刚体 则设为运动学，停止模拟
            if (m_rigidbody)
            {
                m_rigidbody.isKinematic = true;
            }

            broken = true;
            display.SetActive(false);
            m_collider.enabled = false;
            m_audio.PlayOneShot(clip);
            OnBreak?.Invoke();
        }
    }

    protected void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_collider = GetComponent<Collider>();
        TryGetComponent(out m_rigidbody);
    }
}
