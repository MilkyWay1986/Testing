using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Infrastructure.Abstract;
using GameStore.WebUI.Models;

namespace GameStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Try_Login_With_Good_Credentials()
        {
            // Организация - создание имитации поставщика аутентификации
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "1")).Returns(true);

            // Организация - создание модели представления
            // с правильными учетными данными
            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "1"
            };

            // Организация - создание контроллера
            AccountController target = new AccountController(mock.Object);

            // Действие - аутентификация
            ActionResult result = target.Login(model, "/MyURL");

            // Утверждение
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Try_Login_With_Bad_Credentials()
        {
            // Организация - создание имитации поставщика аутентификации
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("hacker", "123")).Returns(false);

            // Организация - создание модели представления
            // с некорректными учетными данными
            LoginViewModel model = new LoginViewModel
            {
                UserName = "hacker",
                Password = "123"
            };

            // Организация - создание контроллера
            AccountController target = new AccountController(mock.Object);

            // Действие - аутентификация
            ActionResult result = target.Login(model, "/MyURL");

            // Утверждение
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
