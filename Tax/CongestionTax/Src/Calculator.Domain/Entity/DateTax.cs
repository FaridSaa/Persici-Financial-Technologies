namespace Calculator.Domain.Entity
{
    using Calculator.Domain.Entity.Enum;

    public class DateTax
    {
        public DateTime Date { get; set; }
        public int Fee { get; set; }
        public CurrencyUnitEnum Unit { get; set; }
    }
}
