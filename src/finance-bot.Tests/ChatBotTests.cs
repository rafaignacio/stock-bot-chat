using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using finance_bot.Chatbot.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using stock_bot.API.Controllers;
using Xunit;
using stock_bot.API.Models;

namespace finance_bot.Tests
{
    public class ChatBotTests 
    {
        Dictionary<string, string> configs = new Dictionary<string,string>{
            { "SecretKey", "secret-423189nfdsk209348" },
        };

        [Fact]
        public async Task LoginSucessfulTest()
        {

            var repo = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<LoginController>>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configs).Build();

            repo.Setup( r => r.Login("user", "password", CancellationToken.None) ).ReturnsAsync("123");

            var controller = new LoginController(configuration, logger.Object, repo.Object);
            var token = await controller.Login(new stock_bot.API.Models.LoginModel{
                Username = "user",
                Password = "password"
            });

            var result = token.Result as OkObjectResult;

            Assert.NotNull(result?.Value);
        }

        [Fact]
        public async Task LoginUnsucessfulTest()
        {

            var repo = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<LoginController>>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configs).Build();

            repo.Setup( r => r.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()) )
                .ReturnsAsync(() => null);

            var controller = new LoginController(configuration, logger.Object, repo.Object);
            var token = await controller.Login(new stock_bot.API.Models.LoginModel{
                Username = "user",
                Password = "password"
            });

            var result = token.Result as UnauthorizedResult;

            Assert.NotNull(result);
        }
    }
}
