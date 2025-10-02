
using UnityEngine;

public class AirDivePlayerState :PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.verticalVelocity=Vector3.zero; //清空竖直速度
        player.lateralVelocity = player.transform.forward *
                                 player.stats.current.airDiveForwardForce;//向前施加俯冲速度
    }

    protected override void OnExit(Player player)
    {
        
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.Jump();

        //开启坡度修正时，根据坡度调整俯冲力
        /*if (player.stats.current.applyDiveSlopeFactor)
        {
            player.SlopeFactor(
                player.stats.current.slopeUpwardForce,
                player.stats.current.slopeDownwardForce);
        }*/
        
        player.FaceDirection(player.lateralVelocity);

        if (player.isGrounded)//落地处理
        {
            var inputDirection = player.inputs.GetMovementCameraDirection();
            
            var localInputDirection = player.transform.InverseTransformDirection(inputDirection);
            var rotation = localInputDirection.x
                           * player.stats.current.airDiveRotationSpeed
                           * Time.deltaTime;
            player.lateralVelocity = Quaternion.Euler(0, rotation, 0) * player.lateralVelocity;

            /*if (player.OnSlopingGround())
            {
                player.Decelerate(player.stats.current.airDiveSlopeFriction);
            }
            else
            {*/
                player.Decelerate(player.stats.current.airDiveFriction);

                if (player.lateralVelocity.magnitude == 0)
                {
                    player.verticalVelocity=Vector3.up
                        *player.stats.current.airDiveGroundLeapHeight;
                    
                    player.states.Change<FallPlayerState>();
                }
            /*}*/
        }
    }

    public override void OnContact(Player player, Collider other)
    {
        if(!player.isGrounded)
            player.WallDrag(other);
    }
}
