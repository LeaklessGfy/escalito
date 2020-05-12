namespace Cash.CashIn
{
    public interface IInvestment
    {
        string Investor { get; }
        bool CheckRequirements();
    }
}