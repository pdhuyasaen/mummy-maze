namespace Dev.Scripts.InAppPurchasing
{
    public interface IProduct
    {
        event System.Action<IProduct> OnPurchasedEvent;
        bool CanPurchase { get; }
        bool HasPurchased { get; }
        void Purchase();
    }
}