using ExchangeApiTesting.Models;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace ExchangeApiTesting.Steps
{
    [Binding]
    public class ExchangeCurrencySteps
    {
        private readonly ScenarioContext context;
        public ExchangeCurrencySteps(ScenarioContext context)
        {
            this.context = context;
            var client = new HttpClient();
            string baseUrl = "https://localhost:5001/exchange";
            context.Set<string>(baseUrl,"baseUrl");
            context.Set<HttpClient>(client, "client");
        }

        [Given(@"I do a ""(.*)"" GET request")]
        public void GivenIDoAGETRequest(string route)
        {
            string baseUrl = context.Get<string>("baseUrl");
            string requestUrl = $"{baseUrl}{route}";
            context.Set<string>(requestUrl, "RequestUrl");
        }

        [Given(@"I want to convert (.*) ""(.*)"" to ""(.*)""")]
        public void GivenIWantToConvertTo(int amount, string fromCurrency, string toCurrency)
        {
            string requestUrl = context.Get<string>("RequestUrl");
            string requestBody = $"{requestUrl}?currency1={fromCurrency}&currency2={toCurrency}&amount={amount}";
            context.Set<string>(requestBody, "RequestBody");
            context.Set<int>(amount, "Amount");
        }

        [When(@"I have the following currencies ""(.*)"" and ""(.*)""")]
        public void WhenIHaveTheFollowingCurrencies(string currency1, string currency2)
        {
            string requestUrl = context.Get<string>("RequestUrl");
            string requestBody = $"{requestUrl}?currency1={currency1}&currency2={currency2}";
            context.Set<string>(requestBody, "RequestBody");
        }

        [When(@"I send a request to the API")]
        public async Task WhenISendARequestToApi()
        {
            string requestBody = context.Get<string>("RequestBody");
            var request = new HttpRequestMessage(HttpMethod.Get, requestBody);
            var client = context.Get<HttpClient>("client");
            var response = await client.SendAsync(request).ConfigureAwait(false);
            try
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                context.Set(responseBody, "ResponseBody");
            }
            finally
            {
                // move along, move along
            }
            context.Set((int)response.StatusCode, "ResponseCode");
            context.Set(response.ReasonPhrase, "ResponseReasonPhrase");
        }

        [Then(@"the response code should be (.*) with response message ""(.*)""")]
        public void ThenTheReponseCodeShouldBe(int errorCode, string reasonPhrase)
        {
            context.Get<int>("ResponseCode").Should().Be(errorCode);
            context.Get<string>("ResponseReasonPhrase").Should().Be(reasonPhrase);
        }

        [Then(@"the response code should be (.*) with error message ""(.*)""")]
        public void ThenTheReponseCodeShouldBe2(int errorCode, string errorMessage)
        {
            context.Get<int>("ResponseCode").Should().Be(errorCode);
            context.Get<string>("ResponseBody").Should().Be(errorMessage);
        }
        
        [Then(@"The given input values: (.*) ""(.*)"" ""(.*)"" are the same in response")]
        public void ThenVerifyInput(double amountInput, string fromCurrency, string toCurrency)
        {
            var response = context.Get<string>("ResponseBody");
            Exchange exchange = JsonConvert.DeserializeObject<Exchange>(response);
            amountInput.Should().Be(exchange.amount);
            fromCurrency.Should().Be(exchange.from);
            toCurrency.Should().Be(exchange.to);
        }

        [Then(@"Verify that the response after conversion is valid")]
        public void ThenVerifyThatTheResponseAfterConversionIsValid()
        {
            var response = context.Get<string>("ResponseBody");
            Exchange exchange = JsonConvert.DeserializeObject<Exchange>(response);
            double amountInput = context.Get<int>("Amount");
            double rate = exchange.exchangeRate;
            
            rate.Should().BePositive();
            (amountInput * rate).Should().Be(exchange.convertResult);
            (exchange.amount * rate).Should().Be(exchange.convertResult);  
        }
    }
}
