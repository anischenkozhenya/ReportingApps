using System;
using System.Threading.Tasks;

namespace Northwind.CurrencyServices.CountryCurrency
{
    public class CurrencyExchangeService
    {
        private readonly string accessKey;

        public CurrencyExchangeService(string accesskey)
        {
            this.accessKey = !string.IsNullOrWhiteSpace(accesskey) ? accesskey : throw new ArgumentException("Access key is invalid.", nameof(accesskey));
        }

        public async Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency)
        {
            throw new NotImplementedException("Implement GetCurrencyExchangeRate.");
        }
    }
}
