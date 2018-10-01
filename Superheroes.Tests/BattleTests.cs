using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Superheroes.Tests
{
    public class BattleTests
    {
        [Fact]
        public async Task CanGetHeros()
        {
            var charactersProvider = new FakeCharactersProvider();

            var startup = new WebHostBuilder()
                            .UseStartup<Startup>()
                            .ConfigureServices(x => 
                            {
                                x.AddSingleton<ICharactersProvider>(charactersProvider);
                            });
            var testServer = new TestServer(startup);
            var client = testServer.CreateClient();

            charactersProvider.FakeResponse(new CharactersResponse
            {
                Items = new []
                {
                    new CharacterResponse
                    {
                        Name = "Batman",
                        Score = 8.3,
                        Type = "hero"
                    },
                    new CharacterResponse
                    {
                        Name = "Joker",
                        Score = 8.2,
                        Type = "villain"
                    }
                }
            });

            var response = await client.GetAsync("battle?hero=Batman&villain=Joker");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<JObject>(responseJson);

            responseObject.Value<string>("name").Should().Be("Batman");
        }

        [Fact]
        public async void When_WeDonotHave_Hero_and_Villain_ShouldReturn404()
        {
            var charactersProvider = new FakeCharactersProvider();

            var startup = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(x =>
                {
                    x.AddSingleton<ICharactersProvider>(charactersProvider);
                });
            var testServer = new TestServer(startup);
            var client = testServer.CreateClient();

            charactersProvider.FakeResponse(new CharactersResponse
            {
                Items = new[]
                {
                    new CharacterResponse
                    {
                        Name = "Batman",
                        Score = 8.3,
                        Type = "hero"
                    },
                    new CharacterResponse
                    {
                        Name = "Joker",
                        Score = 8.2,
                        Type = "villain"
                    }
                }
            });

            var response = await client.GetAsync("battle?hero=superman&villain=testVillain");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void When_heroCharacterdoesnotexists_and_villain_exists_should_return_badRequest()
        {
            var charactersProvider = new FakeCharactersProvider();

            var startup = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(x =>
                {
                    x.AddSingleton<ICharactersProvider>(charactersProvider);
                });
            var testServer = new TestServer(startup);
            var client = testServer.CreateClient();

            charactersProvider.FakeResponse(new CharactersResponse
            {
                Items = new[]
                {
                    new CharacterResponse
                    {
                        Name = "Batman",
                        Score = 8.3,
                        Type = "hero"
                    },
                    new CharacterResponse
                    {
                        Name = "Joker",
                        Score = 8.2,
                        Type = "villain"
                    }
                }
            });

            var response = await client.GetAsync("battle?hero=superman&villain=Joker");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task When_HeroFightsWithVillain_AndHasWeaknessAgaintsThatVillain()
        {
            var charactersProvider = new FakeCharactersProvider();

            var startup = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(x =>
                {
                    x.AddSingleton<ICharactersProvider>(charactersProvider);
                });
            var testServer = new TestServer(startup);
            var client = testServer.CreateClient();

            charactersProvider.FakeResponse(new CharactersResponse
            {
                Items = new[]
                {
                    new CharacterResponse
                    {
                        Name = "Batman",
                        Score = 8.3,
                        Type = "hero",
                        Weakness = "Joker"
                    },
                    new CharacterResponse
                    {
                        Name = "Joker",
                        Score = 8.2,
                        Type = "villain"
                    }
                }
            });

            var response = await client.GetAsync("battle?hero=Batman&villain=Joker");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<JObject>(responseJson);

            responseObject.Value<string>("name").Should().Be("Joker");
        }
    }
}
