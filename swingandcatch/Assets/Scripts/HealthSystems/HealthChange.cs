namespace TheGame.HealthSystems
{
    public readonly struct HealthChange
    {
        public readonly float maxHealth;
        public readonly float currentHealth;

        public HealthChange(float maxHealth, float currentHealth)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
        }
    }
}