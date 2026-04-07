using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestExample.Controllers;
using System.Text.RegularExpressions;
using System.Activities;


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

        [Test]
        [TestCase("nincsszam", false)]         // Nincs szám
        [TestCase("NINCSKISBETU1", false)]     // Nincs kisbetű
        [TestCase("nincsnagybetu1", false)]    // Nincs nagybetű
        [TestCase("Rövid1", false)]            // Túl rövid (kevesebb mint 8 karakter)
        [TestCase("TokeletesJelszo123", true)] // Megfelelő jelszó

        public void TestValidatePassword(string password, bool expectedResult)
        {
            // Arrange - Előkészítés
            var accountController = new AccountController();

            // Act - Cselekvés
            var actualResult = accountController.ValidatePassword(password);

            // Assert - Ellenőrzés
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        [TestCase("irf@uni-corvinus.hu", "TokeletesJelszo123")]
        [TestCase("tesztelek@gmail.com", "HosszuJelszo99")]
        public void TestRegisterHappyPath(string email, string password)
        {
            // Arrange - Előkészítés
            var accountController = new AccountController();

            // Act - Cselekvés
            var actualResult = accountController.Register(email, password);

            // Assert - Ellenőrzés
            Assert.AreEqual(email, actualResult.Email); // Egyezik az e-mail?
            Assert.AreEqual(password, actualResult.Password); // Egyezik a jelszó?
            Assert.AreNotEqual(Guid.Empty, actualResult.ID); // Kapott azonosítót (nem üres)?
        }

        [Test]
        [TestCase("irf@uni-corvinus", "Abcd1234")]       // Rossz e-mail
        [TestCase("irf.uni-corvinus.hu", "Abcd1234")]    // Rossz e-mail
        [TestCase("irf@uni-corvinus.hu", "abcd1234")]    // Rossz jelszó (nincs nagybetű)
        [TestCase("irf@uni-corvinus.hu", "ABCD1234")]    // Rossz jelszó (nincs kisbetű)
        [TestCase("irf@uni-corvinus.hu", "abcdABCD")]    // Rossz jelszó (nincs szám)
        [TestCase("irf@uni-corvinus.hu", "Ab1234")]      // Rossz jelszó (rövid)
        public void TestRegisterValidateException(string email, string password)
        {
            // Arrange
            var accountController = new AccountController();

            // Act
            try
            {
                var actualResult = accountController.Register(email, password);
                Assert.Fail(); // Ha idáig eljut hiba nélkül, akkor megbukott a teszt!
            }
            catch (Exception ex)
            {
                // Assert
                Assert.IsInstanceOf<ValidationException>(ex);
            }
        }
    }
}
