using TheGame.HealthSystems;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.HealthSystems;
using UnityEngine;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.PlayerSystems
{
    public class PlayerFSMDamageHandler : MonoBehaviour, IDamageable
    {
        [SerializeField] HealthSO healthSO;
        [SerializeField] float damageImmuneDuration = 0.75f;
        [SerializeField] FloatChannelSO updatePlayerHealthChannel;
        [SerializeField] FloatChannelSO cameraShakeChannel;

        bool damageImmune;
        Health health;
        
        void Awake()
        {
            health = healthSO.GetHealth();
        }

        bool IDamageable.CanReceiveDamage() => damageImmune == false;

        void IDamageable.ReceiveDamage(float amount)
        {
            if (damageImmune) return;
            damageImmune = true;
            XIVEventSystem.SendEvent(new InvokeAfterEvent(damageImmuneDuration).OnCompleted(() => damageImmune = false));
            health.DecreaseCurrentHealth(amount);
            cameraShakeChannel.RaiseEvent(10f);
            updatePlayerHealthChannel.RaiseEvent(health.normalized);
        }
        
        Health IDamageable.GetHealth() => health;
    }
}