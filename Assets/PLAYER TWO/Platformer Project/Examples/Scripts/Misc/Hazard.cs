using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Examples/Scripts/Misc/Hazard")]
public class Hazard : MonoBehaviour,IEntityContact
{
    /// <summary>
    /// 是否实心，true物理阻挡，false：仅伤害
    /// </summary>
    public bool isSolide;

    /// <summary>
    /// 是否只能从上方攻击玩家
    /// </summary>
    public bool damageOnlyFromAbove;
    /// <summary>
    /// 每次的伤害值
    /// </summary>
    public int damage=1;
    
    protected Collider m_collider;

    protected virtual void Awake()
    {
        //将标签设置为陷阱类
        tag = GameTags.Hazard;
    }

    protected virtual void TryToApplyDamage(Player player)
    {
        if (!damageOnlyFromAbove || player.velocity.y <= 0 && player.isPointUnderStep(m_collider.bounds.max))
        {
            player.ApplyDamage(damage, transform.position);
        }
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            TryToApplyDamage(player);
        }
    }
    
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                TryToApplyDamage(player);
            }
        }
    }
}
