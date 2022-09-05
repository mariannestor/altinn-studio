using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Studio.Designer.Configuration;
using Altinn.Studio.Designer.Controllers;
using Altinn.Studio.Designer.Services.Interfaces;
using Designer.Tests.Mocks;
using Designer.Tests.Utils;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Designer.Tests.Controllers
{
    public class TextsControllerTests : IClassFixture<WebApplicationFactory<TextsController>>
    {
        private readonly WebApplicationFactory<TextsController> _factory;
        private readonly string _versionPrefix = "designer/api/v2";

        public TextsControllerTests(WebApplicationFactory<TextsController> factory)
        {
            _factory = factory;
            TestSetupUtils.SetupDirtyHackIfLinux();
        }

        [Fact]
        public async Task GetText_ReturnsNbText()
        {
            HttpClient client = GetTestClient();
            string dataPathWithData = $"{_versionPrefix}/ttd/new-resource-format/texts/nb";
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, dataPathWithData);
            await AuthenticationUtil.AddAuthenticateAndAuthAndXsrFCookieToRequest(client, httpRequestMessage);

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JsonDocument responseDocument = JsonDocument.Parse(responseBody);
            Dictionary<string, string> responseText = JsonSerializer.Deserialize<Dictionary<string, string>>(responseDocument.RootElement.ToString());

            Assert.Equal(new Dictionary<string, string> { { "key_1", "value_1" }, { "key_2", "value_2" } }, responseText);
        }

        [Fact]
        public async Task EditText_NewNbText()
        {
            var targetRepository = Guid.NewGuid().ToString();
            await TestDataHelper.CopyRepositoryForTest("ttd", "new-resource-format", "testUser", targetRepository);

            HttpClient client = GetTestClient();
            string dataPathWithData = $"{_versionPrefix}/ttd/{targetRepository}/texts/nb";
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, dataPathWithData);
            httpRequestMessage.Content = JsonContent.Create(new { new_key_1 = "new_value_1", new_key_2 = "new_value_2" });
            await AuthenticationUtil.AddAuthenticateAndAuthAndXsrFCookieToRequest(client, httpRequestMessage);

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();

            try
            {
                string text = File.ReadAllText($"../../../_TestData/Repositories/testUser/ttd/{targetRepository}/App/config/texts/text.nb.json");
                Dictionary<string, string> jsonText = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                Assert.Equal(new Dictionary<string, string> { { "new_key_1", "new_value_1" }, { "new_key_2", "new_value_2" } }, jsonText);
            }
            finally
            {
                TestDataHelper.DeleteAppRepository("ttd", targetRepository, "testUser");
            }
        }

        private HttpClient GetTestClient()
        {
            string unitTestFolder =
                Path.GetDirectoryName(new Uri(typeof(DatamodelsControllerTests).Assembly.Location).LocalPath);
            string projectDir = Directory.GetCurrentDirectory();
            string configPath = Path.Combine(projectDir, "appsettings.json");

            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) => { conf.AddJsonFile(configPath); });

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(configPath)
                    .Build();

                configuration.GetSection("ServiceRepositorySettings:RepositoryLocation").Value =
                    Path.Combine(unitTestFolder, "..", "..", "..", "_TestData", "Repositories");

                IConfigurationSection serviceRepositorySettingSection =
                    configuration.GetSection("ServiceRepositorySettings");

                Mock<IRepository> repositoryMock = new Mock<IRepository>() { CallBase = true, };
                repositoryMock
                    .Setup(r => r.UpdateApplicationWithAppLogicModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

                repositoryMock.Setup(r => r.ReadData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string, string>(async (org, repo, path) =>
                    {
                        string repopath = Path.Combine(unitTestFolder, "..", "..", "..", "_TestData", "Repositories", "testUser", org, repo, path);

                        Stream fs = File.OpenRead(repopath);
                        return await Task.FromResult(fs);
                    });
                repositoryMock.Setup(r => r.DeleteData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Verifiable();
                repositoryMock.Setup(r =>
                        r.WriteData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                    .Verifiable();
                repositoryMock.Setup(r =>
                        r.DeleteMetadataForAttachment(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(true);
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<ServiceRepositorySettings>(serviceRepositorySettingSection);
                    services.AddSingleton<IGitea, IGiteaMock>();

                    services.AddSingleton(repositoryMock.Object);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            return client;
        }
    }
}
