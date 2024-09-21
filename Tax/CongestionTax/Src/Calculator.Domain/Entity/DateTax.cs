namespace Calculator.Domain.Entity
{
    public class DateTax
    {
        public DateTime Date { get; set; }
        public int Fee { get; set; }
        public CurrencyUnitEnum Unit { get; set; }
    }
}
