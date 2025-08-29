using UnityEngine;

public class PlayerStatus : EntityStatus<PlayerStatus>
{
    [Header("General Status")]
    public float rotationSpeed = 970f;
    public float friction = 16f;
    public float gravityTopSpeed = 50f;
    public float gravity = 38f;
    public float fallGravity = 65f;
    [Header("Motion Status")]
    public float breakThreshold = -0.8f;
    public float turningDrag = 28f;
    public float acceleration = 13f;
    public float topSpeed = 6f;
    public float airAcceleration;
    public float deceleration = 28f;
    [Header("Running Status")]
    public float runningAcceleration = 16f;
    public float runningTopSpeed = 7.5f;
    public float runningTurnningDrag = 14f;
    [Header("Backflip stats")]
    public bool canBackflip=true;
    public float backflipJumpHeight = 23f;
    public float backflipGravity = 35f;
    public float backflipDrag = 2.5f;
    public float backflipAirAcceleration = 12f;
    public float backflipTopSpeed=7.5f;
    public float backflipBackwardTurnForece = 8f;
    [Header("Jump stats")]
    public int multiJumps = 1;
    public float coyoteJumpThreshold = 0.15f;
    public float maxJumpHeight = 17f;
    public float miniJumpHeight = 10f;

}

