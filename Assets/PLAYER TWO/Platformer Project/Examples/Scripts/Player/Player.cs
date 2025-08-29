using UnityEngine;

public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;

    public PlayerInputManager inputs { get; protected set; }

    public PlayerStatusManager stats { get; protected set; }

    public int jumpCounter { get; protected set; }

    public bool holding { get; protected set; }

    protected virtual void initializeInputs() => inputs = GetComponent<PlayerInputManager>();

    protected virtual void initializeStats() => stats = GetComponent<PlayerStatusManager>();

    protected override void Awake()
    {
        base.Awake();
        initializeInputs();
        initializeStats();
    }

    public virtual void Accelerate(Vector3 direction)
    {
        var turningDrag = isGrounded && inputs.GetRun() 
            ? stats.current.runningTurnningDrag 
            : stats.current.turningDrag;

        var accelerate = isGrounded && inputs.GetRun()
            ? stats.current.runningAcceleration
            : stats.current.acceleration;

        var topSpeed = inputs.GetRun()
            ? stats.current.runningTopSpeed
            : stats.current.acceleration;

        var finalAcceleration = isGrounded ? accelerate : stats.current.airAcceleration;

        Accelerate(direction.normalized, turningDrag, finalAcceleration, topSpeed);
    }

    public virtual void FaceDirectionSmooth(Vector3 direction)
    {
        FaceDirection(direction, stats.current.rotationSpeed);
    }

    public virtual void Decelerate()
    {
        Decelerate(stats.current.deceleration);
    }

    public virtual void Backflip(float force)
    {
        if(stats.current.canBackflip)
        {
            verticalVelocity=Vector3.up * stats.current.backflipJumpHeight;
            lateralVelocity = -transform.forward * force;
            states.Change<BackflipPlayerState>();
            playerEvents?.OnBackflip?.Invoke();
        }
    }

   public virtual void BackflipAcceleration()
   {
        var direction = inputs.GetMovementCamerDirection();
        Accelerate(direction, stats.current.backflipGravity,stats.current.backflipAirAcceleration, stats.current.backflipTopSpeed);
   }

   public virtual void Friction()
   {
        Decelerate(stats.current.friction);
   }

   public virtual void Gravity()
   {
        if(!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
        {
            var speed = verticalVelocity.y;
            var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
            speed -= force * gravityMultiplier * Time.deltaTime;
            speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);
            verticalVelocity = new Vector3(0, speed, 0);
        }
   }

    public virtual void Fall()
    {
        if(!isGrounded)
        {
            states.Change<FallPlayerState>();
        }
    }

}
