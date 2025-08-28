
using System;
using UnityEngine.Events;

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
    public UnityEvent OnStopFalling;
    public UnityEvent OnStopLanding;
    public UnityEvent OnledgeGrabbed;  
    public UnityEvent OnledgeClimbing;
    public UnityEvent OnAirDive;
    public UnityEvent OnBackflip;
    public UnityEvent OnGlidingStop;    
    public UnityEvent OnGlidingStart;
    public UnityEvent OnDashStart;
    public UnityEvent OnDashStop;

}

