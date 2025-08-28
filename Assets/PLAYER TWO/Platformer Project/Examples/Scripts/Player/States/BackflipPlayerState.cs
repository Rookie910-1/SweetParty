
using UnityEngine;

public class BackflipPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
       
    }

    protected override void OnExit(Player player)
    {
        
    }

    protected override void OnStep(Player player)
    {
        var inputDeriction = player.inputs.GetMovementDirection();
        player.Gravity(player.stats.current.backflipGravity);
        player.BackflipAcceleration();

        if(player.isGrounded)
        {
            player.lateralVelocity = Vector3.zero;
            player.states.Change<IdelPlayerState>();
        }
        else
        {

        }
    }
}

