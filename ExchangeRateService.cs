using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_BankSystem
{
    public class ExchangeRateService
    {
        private readonly HttpClient httpClient;
        private const string ExchangeRateApiUrl = "https://v6.exchangerate-api.com/v6/2d8754c1bf6d68b8bbea954d/latest/OMR";

        public ExchangeRateService()
        {
            httpClient = new HttpClient();
        }

        public async Task<ExchangeRateData> GetExchangeRatesAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(ExchangeRateApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ExchangeRateData exchangeRateData = JsonConvert.DeserializeObject<ExchangeRateData>(responseBody);
                    return exchangeRateData;
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to retrieve data from the API: {ex.Message}");
                return null;
            }
        }
    }
    public class CurrencyConverter
    {
        private readonly ExchangeRateService exchangeRateService;

        public CurrencyConverter()
        {
            exchangeRateService = new ExchangeRateService();
        }

        public async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            // Get the latest exchange rates
            ExchangeRateData exchangeRateData = await exchangeRateService.GetExchangeRatesAsync();

            if (exchangeRateData != null && exchangeRateData.conversion_rates.ContainsKey(fromCurrency) && exchangeRateData.conversion_rates.ContainsKey(toCurrency))
            {
                // Convert the amount from 'fromCurrency' to 'toCurrency'
                decimal fromCurrencyRate = exchangeRateData.conversion_rates[fromCurrency];
                decimal toCurrencyRate = exchangeRateData.conversion_rates[toCurrency];

                // Convert the amount
                decimal convertedAmount = amount * (toCurrencyRate / fromCurrencyRate);
                return convertedAmount;
            }
            else
            {
                Console.WriteLine("Invalid currency codes or exchange rate data unavailable.");
                return -1; // Or you can throw an exception or handle the error as needed.
            }
        }
    }

    public class ExchangeRateData
    {
        public string base_code { get; set; }
        public Dictionary<string, decimal> conversion_rates { get; set; }
    }
}
