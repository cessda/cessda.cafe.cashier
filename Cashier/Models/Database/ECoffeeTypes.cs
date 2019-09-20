namespace Cashier.Models.Database
{
    /// <summary>
    /// List of allowed coffees
    /// </summary>
    public enum ECoffeeTypes : byte
    {
#pragma warning disable CS1591
        COFFEE,
        STRONG_COFFEE,
        CAPPUCCINO,
        MOCCACHINO,
        COFFEE_WITH_MILK,
        ESPRESSO,
        ESPRESSO_CHOCOLATE,
        KAKAO,
        HOT_WATER
#pragma warning restore CS1591
    }
}
