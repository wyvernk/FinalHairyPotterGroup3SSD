using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Tests
{
    public class RateLimitTests
    {
        // Test for general rate limiting across all endpoints
        [Fact]
        public async Task TestGeneralRateLimit()
        {
            // Arrange
            var factory = new WebApplicationFactory<TestStartup>(); // Create a test web application factory
            var client = factory.CreateClient(); // Create a test client

            // Act - Send requests until the rate limit is exceeded
            for (int i = 0; i < 101; i++)
            {
                var response = await client.GetAsync("/api/test"); // Send GET request
                if (i < 100)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode); // Expect OK for first 100 requests
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode); // Expect TooManyRequests for the 101st request
                }
            }
        }

        // Test for rate limiting on the /checkout endpoint
        [Fact]
        public async Task TestCheckoutRateLimit()
        {
            // Arrange
            var factory = new WebApplicationFactory<TestStartup>(); // Create a test web application factory
            var client = factory.CreateClient(); // Create a test client

            // Act - Send POST requests to /checkout until the rate limit is exceeded
            for (int i = 0; i < 51; i++)
            {
                var response = await client.PostAsync("/checkout", null); // Send POST request
                if (i < 50)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode); // Expect OK for first 50 requests
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode); // Expect TooManyRequests for the 51st request
                }
            }
        }

        // Test for rate limiting on the /my/login endpoint
        [Fact]
        public async Task TestLoginRateLimit()
        {
            // Arrange
            var factory = new WebApplicationFactory<TestStartup>(); // Create a test web application factory
            var client = factory.CreateClient(); // Create a test client

            // Act - Send POST requests to /my/login until the rate limit is exceeded
            for (int i = 0; i < 101; i++)
            {
                var response = await client.PostAsync("/my/login", null); // Send POST request
                if (i < 100)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode); // Expect OK for first 100 requests
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode); // Expect TooManyRequests for the 101st request
                }
            }
        }

        // Test for rate limiting on the /my/register endpoint
        [Fact]
        public async Task TestRegisterRateLimit()
        {
            // Arrange
            var factory = new WebApplicationFactory<TestStartup>(); // Create a test web application factory
            var client = factory.CreateClient(); // Create a test client

            // Act - Send POST requests to /my/register until the rate limit is exceeded
            for (int i = 0; i < 101; i++)
            {
                var response = await client.PostAsync("/my/register", null); // Send POST request
                if (i < 100)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode); // Expect OK for first 100 requests
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode); // Expect TooManyRequests for the 101st request
                }
            }
        }
    }
}
