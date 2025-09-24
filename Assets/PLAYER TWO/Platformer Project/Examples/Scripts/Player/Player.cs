using UnityEngine;

public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;

    public PlayerInputManager inputs { get; protected set; }

    public PlayerStatusManager stats { get; protected set; }

    public int jumpCounter { get; protected set; }
    
    public bool onWater { get; protected set; }
    
    public Health health { get; protected set; }

    public bool holding { get; protected set; }

    protected virtual void initializeInputs() => inputs = GetComponent<PlayerInputManager>();

    protected virtual void initializeStats() => stats = GetComponent<PlayerStatusManager>();
    
    protected virtual void initializeHealth() => health = GetComponent<Health>();
    
    protected virtual void initializeTag()=>tag=GameTags.Player;

    protected override void Awake()
    {
        base.Awake();
        initializeInputs();
        initializeStats();
        initializeHealth();
        initializeTag();
        
        entityEvents.onGroundEnter.AddListener(()=>
        {
            ResetJumps();
        });
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

    public virtual void CrawlingAccelerate(Vector3 direction)
    {
        Accelerate(direction,stats.current.crawlingTurningSpeed,stats.current.crawlingAcceleration,stats.current.crawlingTopSpeed);
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
            verticalVelocity=Vector3.up * stats.current.backflipJumpHeight;//上跳力
            lateralVelocity = -transform.forward * force;//向后推力
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

    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = inputs.GetMovementCamerDirection();
        Accelerate(inputDirection);
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

   public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

   public override void ApplyDamage(int amount, Vector3 origin)
   {
       if (!health.isEmpty && !health.recovering)
       {
           health.Damage(amount);
           var damageDir = origin - transform.position;
           damageDir.y = 0;
           damageDir = damageDir.normalized;
           FaceDirection(damageDir);//面向攻击方向
           //受伤后向后退
           lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;
           //不在水中，则会被击飞向上进入受击状态
           if (!onWater)
           {
               verticalVelocity = Vector3.up * stats.current.hurtUpwardForce;
               states.Change<HurtPlayerState>();
           }

           playerEvents.OnHurt?.Invoke();

           /*if (health.isEmpty)
           {
               Throw();
               playerEvents.OnDie?.Invoke();
           }*/
       }
   }

   public virtual bool canStandUp => !Spherecast(Vector3.up, originalHeight);
    
   public virtual void ResetJumps()=> jumpCounter=0;

   public virtual void SetJumps(int times) => jumpCounter = times;

    public virtual void Fall()
    {
        if(!isGrounded)
        {
            states.Change<FallPlayerState>();
        }
    }

    public virtual void Jump()
    {
        //是否能多段跳
        var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
        //土狼跳（落地一小段时间内仍然可跳）
        var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);

        //在地面/多段跳/土狼跳的条件满足时才可以跳
        if(isGrounded || canMultiJump || canCoyoteJump) 
        {
            if(inputs.GetJumpDown())
            {
                Jump(stats.current.maxJumpHeight);
            }
        }
        //松开跳跃键时，如果还在上升，限制为最小跳跃高度（实现“按得短跳的低”的效果），早松手早限制
        if(inputs.GetJumpUp() && jumpCounter >0 && verticalVelocity.y > stats.current.miniJumpHeight)
        {
            verticalVelocity = Vector3.up * stats.current.miniJumpHeight;
        }
    }

    public virtual void Jump(float height)
    {
        jumpCounter++;
        verticalVelocity=Vector3.up * height;
        states.Change<FallPlayerState>();
        playerEvents.OnJump?.Invoke();
    }

}
