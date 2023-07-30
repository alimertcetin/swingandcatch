using TheGame.HealthSystems;

namespace TheGame.Interfaces
{
    public interface IDamageHandler : IDamageable
    {
        Health GetHealth();
        void SetImmuneState(bool value);
        bool IsImmune();
        float GetImmuneDuration();
    }
}