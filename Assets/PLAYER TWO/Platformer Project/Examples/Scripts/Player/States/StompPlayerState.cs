using UnityEngine;
[AddComponentMenu("PLAYER TWO/Platformer Project/Examples/Scripts/Player/States/StompPlayerState")]
public class StompPlayerState :PlayerState
{
    //空中计时器，用于计算蓄力时间
    protected float m_airTimer;
    //落地计时器，控制落地停留时间
    protected  float m_groundTimer;

    protected bool m_falling;

    protected bool m_landed;
    protected override void OnEnter(Player player)
    {
        m_landed = m_falling = false;
        m_airTimer = m_groundTimer = 0;
        player.velocity=Vector3.zero;
        player.playerEvents.OnStompStarted?.Invoke();
    }

    protected override void OnExit(Player player)
    {
        player.playerEvents.OnStompEnding?.Invoke();
    }

    protected override void OnStep(Player player)
    {
        if (!m_falling)
        {
            m_airTimer+=Time.deltaTime;
            if (m_airTimer >= player.stats.current.stompAirTime)
            {
                m_falling = true;
                player.playerEvents.OnStompFalling?.Invoke();
            }
        }
        else
        {
            player.verticalVelocity += Vector3.down * player.stats.current.stompDownwardForce;
        }

        if (player.isGrounded)
        {
            if (!m_landed)
            {
                m_landed = true;
                player.playerEvents.OnStompLanding?.Invoke();
            }

            if (m_groundTimer >= player.stats.current.stompAirTime)
            {
                //落地时间结束，小跳反弹并切换到下落状态
                player.verticalVelocity = Vector3.up * player.stats.current.stompGroundTime;
                player.states.Change<FallPlayerState>();
            }
            else
            {
                m_groundTimer += Time.deltaTime;
            }
        }
    }

    public override void OnContact(Player player, Collider other)
    {
        
    }
}
