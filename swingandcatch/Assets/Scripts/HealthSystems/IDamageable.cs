namespace TheGame.HealthSystems
{
    public interface IDamageable
    {
        bool CanReceiveDamage();
        void ReceiveDamage(float amount);
        Health GetHealth();
    }
}