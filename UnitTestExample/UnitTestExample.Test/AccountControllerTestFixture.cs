using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestExample.Controllers;

namespace UnitTestExample.Test
{
    public class AccountControllerTestFixture
    {
        [Test]
        [TestCase("abcd1234", false)]             // Jelszó e-mail helyett
        [TestCase("irf@uni-corvinus", false)]     // Hiányzó domain
        [TestCase("irf.uni-corvinus.hu", false)]  // Hiányzó @ jel
        [TestCase("irf@uni-corvinus.hu", true)]   // Helyes e-mail
        public void TestValidateEmail(string email, bool expectedResult)
        {
            // Arrange - Előkészítés
            var accountController = new AccountController();

            // Act - Cselekvés
            var actualResult = accountController.ValidateEmail(email);

            // Assert - Ellenőrzés
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
