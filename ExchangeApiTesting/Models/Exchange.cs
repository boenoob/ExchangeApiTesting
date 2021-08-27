namespace ExchangeApiTesting.Models
{
    class Exchange
    {
        public string date { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double amount { get; set; }
        public double exchangeRate { get; set; }
        public double convertResult { get; set; }
    }
}