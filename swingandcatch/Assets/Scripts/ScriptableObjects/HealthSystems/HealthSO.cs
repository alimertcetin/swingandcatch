using TheGame.HealthSystems;
using UnityEngine;

namespace TheGame.ScriptableObjects.HealthSystems
{
    [CreateAssetMenu(menuName = MenuPaths.HEALTH_SYSTEM_MENU + nameof(HealthSO))]
    public class HealthSO : ScriptableObject
    {
        [SerializeField] float maxHealth;
        [SerializeField] float startHealth;

        public Health GetHealth() => new Health(maxHealth, startHealth);
    }
}