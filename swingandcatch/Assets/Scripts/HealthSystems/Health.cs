using UnityEngine;
using XIV.Core.Collections;

namespace TheGame.HealthSystems
{
    public class Health
    {
        float maxHealth;
        float currentHealth;
        
        DynamicArray<IHealthListener> listeners;
        public float normalized => currentHealth / maxHealth;

        public Health(float maxHealth, float currentHealth)
        {
            listeners = new DynamicArray<IHealthListener>();
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
        }

        public Health(float maxHealth) : this(maxHealth, maxHealth)
        {
        }

        public void IncreaseCurrentHealth(float amount) => ChangeValue(ref currentHealth, amount);
        public void DecreaseCurrentHealth(float amount)
        {
            ChangeValue(ref currentHealth, -amount);
            if (currentHealth - Mathf.Epsilon < Mathf.Epsilon) InformListenersOnHealthDepleted();
        }

        public void IncreaseMaxHealth(float amount) => ChangeValue(ref maxHealth, amount);
        public void DecreaseMaxHealth(float amount) => ChangeValue(ref maxHealth, -amount);

        public void AddListener(IHealthListener listener)
        {
            if (listeners.Contains(ref listener) == false)
            {
                listeners.Add() = listener;
            }
        }

        public bool RemoveListener(IHealthListener listener)
        {
            int index = listeners.IndexOf(ref listener);
            if (index == -1) return false;
            listeners.RemoveAt(index);
            return true;
        }

        void InformListenersOnHealthChange()
        {
            var change = new HealthChange(maxHealth, currentHealth);
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnHealthChanged(ref change);
            }
        }

        void InformListenersOnHealthDepleted()
        {
            var change = new HealthChange(maxHealth, currentHealth);
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnHealthDepleted(ref change);
            }
        }

        void ChangeValue(ref float value, float amount)
        {
            var newValue = Mathf.Clamp(value + amount, 0, maxHealth);
            var diff = Mathf.Abs(newValue - value);
            value = newValue;
            if (diff > 0) InformListenersOnHealthChange();
        }
    }
}