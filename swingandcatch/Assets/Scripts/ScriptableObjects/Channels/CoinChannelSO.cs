using TheGame.CoinSystems;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(CoinChannelSO))]
    public class CoinChannelSO : XIVChannelSO<Coin>
    {
        
    }
}