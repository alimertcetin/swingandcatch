namespace TheGame.UISystems.Shop
{
    public interface IShopUIItemListener
    {
        void OnSelect(int itemIndex);
        void OnBuy(int itemIndex);
    }
}
