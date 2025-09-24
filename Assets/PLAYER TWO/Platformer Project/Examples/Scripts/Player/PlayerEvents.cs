
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class PlayerEvents
{
    public UnityEvent OnJump;
    public UnityEvent OnMove;
    public UnityEvent OnHurt;
    public UnityEvent OnDie;
    public UnityEvent OnSpin;
    public UnityEvent OnPickUp;
    public UnityEvent OnThrow;
    public UnityEvent OnStopStarted;
    public UnityEvent OnStompFalling;
    public UnityEvent OnStompLanding;
    public UnityEvent OnStompStarted;
    public UnityEvent OnStompEnding;
    public UnityEvent OnledgeGrabbed;  
    public UnityEvent OnledgeClimbing;
    public UnityEvent OnAirDive;
    public UnityEvent OnBackflip;
    public UnityEvent OnGlidingStop;    
    public UnityEvent OnGlidingStart;
    public UnityEvent OnDashStart;
    public UnityEvent OnDashStop;

}

