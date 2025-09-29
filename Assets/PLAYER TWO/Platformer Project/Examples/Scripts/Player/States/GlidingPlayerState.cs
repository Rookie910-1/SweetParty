using UnityEngine;
public class GlidingPlayerState :PlayerState
{
    protected override void OnEnter(Player player)
    {
        Debug.Log("GlidingPlayerState.OnEnter");
        player.verticalVelocity=Vector3.zero;
        player.playerEvents.OnGlidingStart?.Invoke();
    }

    protected override void OnExit(Player player)
    {
        player.playerEvents.OnGlidingStop?.Invoke();
    }

    protected override void OnStep(Player player)
    {
        var inputDirection = player.inputs.GetMovementCameraDirection();
        HandleGlidingGravity(player);
        player.FaceDirection(player.lateralVelocity);
        player.Accelerate(inputDirection,
            player.stats.current.glidingTurningDrag,
            player.stats.current.airAcceleration,
            player.stats.current.topSpeed);

       // player.LedgeGrab();

       if (player.isGrounded)
       {
           player.states.Change<IdelPlayerState>();
       }
       else if(!player.inputs.GetGlide())
       {
           player.states.Change<FallPlayerState>();
       }
    }

    /// <summary>
    /// 角色在空中缓缓下落
    /// 下落速度不超过glidingMaxFallSpeed
    /// </summary>
    /// <param name="player"></param>
    protected virtual void HandleGlidingGravity(Player player)
    {
        var yVelocity = player.verticalVelocity.y;

        yVelocity -= player.stats.current.glidingGravity * Time.deltaTime;
        
        yVelocity=Mathf.Max(yVelocity,-player.stats.current.glidingMaxFallSpeed);
    }

    public override void OnContact(Player player, Collider other)
    {
        
    }
}
