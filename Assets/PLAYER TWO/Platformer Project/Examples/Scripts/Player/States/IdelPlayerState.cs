
using UnityEngine;

public class IdelPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {

    }

    protected override void OnExit(Player player)
    {

    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Jump();
        player.Fall();
        player.Friction();

        var inputDirection = player.inputs.GetMovementDirection();

        if(inputDirection.sqrMagnitude >0 || player.lateralVelocity.sqrMagnitude >0 )
        {
            player.states.Change<WalkPlayerState>();
        }
    }

    public override void OnContact(Player entity, Collider other)
    {
        
    }
}
