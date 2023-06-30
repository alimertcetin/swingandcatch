namespace TheGame.HealthSystems
{
    public interface IHealthListener
    {
        void OnHealthChanged(ref HealthChange healthChange);
        void OnHealthDepleted(ref HealthChange healthChange);
    }
}