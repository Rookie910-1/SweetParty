using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour
{
    public EntityEvents entityEvents;
    
    protected Collider[] m_contactBuffer = new Collider[10];
    public bool isGrounded { get; protected set; } = true;

    public Vector3 velocity { get; set; }

    public float turningDragMultiplier { get; set; } = 1f;

    public float topSpeedMultiplier { get; set; } = 1f;

    public float acclerationMultiplier { get; set; } = 1f;

    public float decelerationMultiple { get; set; } = 1f;

    public float gravityMultiplier { get; set; } = 1f;

    public CharacterController controller { get; protected set; }

    public float lastGroundTime { get; protected set; }

    protected readonly float m_groundOffset = 0.1f;

    public float originalHeight { get; protected set; }

    /// <summary>
    /// 忽略碰撞器缩放的实体位置
    /// </summary>
    public Vector3 unsizePosition => position - height * transform.up * 0.5f + originalHeight * transform.up *0.5f;
    
    public float positionDelta { get; protected set; }

    public RaycastHit groundHit { get; protected set; }
    
    public Vector3 groundNormal { get; protected set; }
    public float groundAngle{ get; protected set; }
    public Vector3 localSlopeDirection{ get; protected set; }

    public Vector3 lateralVelocity
    {
        get
        {
            return new Vector3(velocity.x, 0, velocity.z);
        }

        set
        {
            velocity = new Vector3(value.x, velocity.y, value.z);
        }
    }

    public Vector3 verticalVelocity
    {
        get
        {
            return new Vector3(0, velocity.y, 0);
        }

        set
        {
            velocity = new Vector3(velocity.x, value.y, velocity.z);
        }
    }

    public float height =>controller.height;

    public float radius => controller.radius;

    public Vector3 center=>controller.center;

    public Vector3 position => transform.position + center;
    
    public Vector3 lastPosition { get; set; }
    
    
    public virtual bool CapsuleCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        return CapsuleCast(direction, distance, out _, layer, queryTriggerInteraction);
    }

    public virtual bool CapsuleCast(Vector3 direction, float distance,
        out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var origin = position - direction * radius + center;
        var offset = transform.up * (height * 0.5f - radius);
        var top = origin + offset;
        var bottom = origin - offset;
        return Physics.CapsuleCast(top, bottom, radius, direction,
            out hit, distance + radius, layer, queryTriggerInteraction);
    }
    
    public virtual bool Spherecast(Vector3 direction,float distance,out RaycastHit hit
        ,int layer=Physics.DefaultRaycastLayers
        ,QueryTriggerInteraction queryTriggerInteraction=QueryTriggerInteraction.Ignore)
    {
        //计算球形检测的有效距离，确保球形检测的范围符合预期
        var castDistance = Mathf.Abs(distance - radius);
        //使用物理引擎进行球形碰撞检测
        return Physics.SphereCast(position, radius, direction,out hit, castDistance, layer, queryTriggerInteraction);
    }

    public virtual bool Spherecast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction=QueryTriggerInteraction.Ignore)
    {
        return Spherecast(direction, distance, out _, layer, queryTriggerInteraction);
    }
    
    public virtual int OverlapEntity(Collider[] result, float skinOffset = 0)
    {
        var contactOffset = skinOffset + controller.skinWidth + Physics.defaultContactOffset;
        var overlapsRadius = radius + contactOffset;
        var offset = (height + contactOffset) * 0.5f - overlapsRadius;
        var top = position + Vector3.up * offset;
        var bottom = position + Vector3.down * offset;
        return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
    }

    public Vector3 stepPosition => position - transform.up * (height * 0.5f - controller.stepOffset);

    public virtual bool isPointUnderStep(Vector3 point) => stepPosition.y >point.y;
    
    public virtual void ApplyDamage(int damage, Vector3 origin) { }

    public virtual void ResizeCollider(float height)
    {
        //计算高度差
        var delta = height - this.height;
        //修改角色控制器的高度
        controller.height = height;
        //调整角色控制器的中心位置，随着高度变化自动平移
        controller.center+=Vector3.up * (delta * 0.5f);
    }
}
public class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states {  get; private set; }
    protected void HandleStates() => states.Step();
    
    protected virtual void InitializeStateManager() => states= GetComponent<EntityStateManager<T>>();

    protected virtual void InitializeController()
    {
        controller = GetComponent<CharacterController>();

        if(!controller)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        //碰撞器表面到实际碰撞检测边界的距离（防止卡住用的小偏移）
        controller.skinWidth = 0.005f;
        //最小移动距离（0表示如果非常小的移动也会被检测到）
        controller.minMoveDistance = 0;
        originalHeight = controller.height;
    }

    protected virtual void Awake()
    {
        InitializeController();
        InitializeStateManager();
    }

    protected virtual void Update()
    {
        if (controller.enabled )
        {
            HandleStates();
            HandleController();
            HandleGround();
            HandleContacts();
        }
    }

    protected virtual void HandleGround()
    {
        //角色的一半身高+地面检测的额外偏移量
        var distance = (height * 0.5f) + m_groundOffset;
        //向下发射球体射线检测地面，并且角色的垂直速度<=0(下落或静止状态)
        if(Spherecast(Vector3.down,distance,out var hit) && verticalVelocity.y <= 0)
        {
            //如果之前不在地面
            if(!isGrounded)
            {
                //是否满足落地条件
                if(EvaluateLanding(hit))
                {
                    EnterGround(hit);
                    
                }//已经在地面的状态
                else if (isPointUnderStep(hit.point))
                {
                    UpdateGround(hit);
                    
                    if (Vector3.Angle(hit.normal, Vector3.up) >= controller.slopeLimit)
                    {
                       // HandleSlopeLimit(hit);
                    }
                }
                else
                {
                    HandleHighLedge(hit);
                }
            }
        }
        else
        {
            ExitGround();
        }
    }
    
    protected virtual void HandleContacts()
    {
        var overlaps = OverlapEntity(m_contactBuffer);

        for (int i = 0; i < overlaps; i++)
        {
            if (!m_contactBuffer[i].isTrigger && m_contactBuffer[i].transform != transform)
            {
                OnContact(m_contactBuffer[i]);

                var listeners = m_contactBuffer[i].GetComponents<IEntityContact>();

                foreach (var contact in listeners)
                {
                    contact.OnEntityContact((T)this);
                }

                if (m_contactBuffer[i].bounds.min.y > controller.bounds.max.y)
                {
                    verticalVelocity = Vector3.Min(verticalVelocity, Vector3.zero);
                }
            }
        }
    }
    
    protected virtual void HandlePosition()
    {
        positionDelta = (position - lastPosition).magnitude;
        lastPosition = position;
    }

    protected virtual void HandleController()
    {
        if(controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
            return;
        }
        transform.position += velocity * Time.deltaTime;
    }

    protected virtual void HandleHighLedge(RaycastHit hit)
    {

    }

    protected virtual void EnterGround(RaycastHit hit)
    {
        if(!isGrounded)
        {
            //记录当前地面的射线检测信息（位置、法线等）
            groundHit = hit;
            //标记为在地面上
            isGrounded = true;
            entityEvents.onGroundEnter?.Invoke();
        }
    }

    
    protected virtual void UpdateGround(RaycastHit hit)
    {
        if (isGrounded)
        {
            groundHit = hit;
            //记录地面法线（用于计算坡度方向）
            groundNormal=groundHit.normal;
            //计算当前地面的坡度角（与Y的夹角）
            groundAngle=Vector3.Angle(Vector3.up,groundHit.normal);
            //计算本地的坡度方向（水平投影后的法线方向）
            localSlopeDirection=new Vector3(groundNormal.x,0, groundNormal.z).normalized;
            //如果地面是平台（tag=platform），让角色成为平台的子物体，跟随平台移动，否则取消父子关系
            transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : null;
        }
    }

    protected virtual void ExitGround()
    {
        if(isGrounded)
        {
            isGrounded = false;
            transform.parent = null;
            lastGroundTime = Time.time;
            verticalVelocity = Vector3.Max(verticalVelocity, Vector3.zero);
            entityEvents.onGroundExit?.Invoke();
        }
    }

    public virtual void SnapToGround(float force)
    {
        if(isGrounded &&(verticalVelocity.y<=0))
            verticalVelocity=Vector3.down *force;
    }

    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        //slopeLimit是坡度的最大限制角度
        return isPointUnderStep(hit.point) && Vector3.Angle(hit.normal,Vector3.up) < controller.slopeLimit;
    }
    
    protected virtual void OnContact(Collider other)
    {
        if (other)
        {
            states.OnContact(other);
        }
    }

    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        if (direction.sqrMagnitude > 0)
        {
            var speed = Vector3.Dot(direction, lateralVelocity);
            var velocity = direction * speed;
            var turningVelocity = lateralVelocity - velocity;
            var turningDelta = turningDrag * turningDragMultiplier * Time.deltaTime;
            var targetTopSpeed = topSpeed * topSpeedMultiplier;

            if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
            {
                speed += acceleration * acclerationMultiplier * Time.deltaTime;
                speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
            }

            velocity = direction * speed;
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            lateralVelocity = velocity + turningVelocity;
        }
    }

    public virtual void FaceDirection(Vector3 direction, float degressPerSpeed)
    {
        if(direction !=  Vector3.zero)
        {
            var rotation = transform.rotation;
            var rotationDelta= degressPerSpeed * Time.deltaTime;
            var target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation =Quaternion.RotateTowards(rotation, target, rotationDelta);
        }
    }

    public virtual void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            var rotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = rotation;
        }
    }

    public virtual void Decelerate(float deceleration)
    {
        var delta=deceleration * decelerationMultiple * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);
    }

    public virtual void Gravity(float gravity)
    {
        if(!isGrounded)
        {
            verticalVelocity += Vector3.down * gravity * gravityMultiplier * Time.deltaTime;
        }
    }
    
    protected virtual void LateUpdate()
    {
        if (controller.enabled)
        {
            HandlePosition();
           // HandlePenetration();
        }
    }
}

