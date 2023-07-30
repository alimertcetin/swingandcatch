using TheGame.HealthSystems;
using TheGame.Interfaces;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.HealthSystems;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    public class PlayerFSMDamageHandler : MonoBehaviour, IDamageHandler
    {
        [SerializeField] HealthSO healthSO;
        public float damageImmuneDuration = 5f;
        public Health health;
        bool damageImmune;
        public FloatChannelSO updatePlayerHealthChannel;
        public FloatChannelSO cameraShakeChannel;

        void Awake()
        {
            health = healthSO.GetHealth();
        }

        bool IDamageable.CanReceiveDamage()
        {
            return damageImmune == false;
        }

        void IDamageable.ReceiveDamage(float amount)
        {
            if (damageImmune) return;
            health.DecreaseCurrentHealth(amount);
            cameraShakeChannel.RaiseEvent(10f);
            updatePlayerHealthChannel.RaiseEvent(health.normalized);
        }
        
        Health IDamageHandler.GetHealth() => health;
        void IDamageHandler.SetImmuneState(bool value) => damageImmune = value;
        bool IDamageHandler.IsImmune() => damageImmune;
        float IDamageHandler.GetImmuneDuration() => damageImmuneDuration;
    }
}