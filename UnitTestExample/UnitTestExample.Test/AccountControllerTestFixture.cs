using Moq;
using NUnit.Framework;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnitTestExample.Abstractions;
using UnitTestExample.Controllers;
using UnitTestExample.Entities;


namespace UnitTestExample.Test
{
    public class AccountControllerTestFixture
    {
        [Test]
        [TestCase("abcd1234", false)]             
        [TestCase("irf@uni-corvinus", false)]     
        [TestCase("irf.uni-corvinus.hu", false)]  
        [TestCase("irf@uni-corvinus.hu", true)]   
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
        [TestCase("nincsszam", false)]    
        [TestCase("NINCSKISBETU1", false)]    
        [TestCase("nincsnagybetu1", false)]   
        [TestCase("Rövid1", false)]            
        [TestCase("TokeletesJelszo123", true)] 

        public void TestValidatePassword(string password, bool expectedResult)
        {
            var accountController = new AccountController();

            var actualResult = accountController.ValidatePassword(password);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        [TestCase("irf@uni-corvinus.hu", "TokeletesJelszo123")]
        [TestCase("tesztelek@gmail.com", "HosszuJelszo99")]
        public void TestRegisterHappyPath(string email, string password)
        {
            var accountController = new AccountController();

            var actualResult = accountController.Register(email, password);

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
            var accountController = new AccountController();

            try
            {
                var actualResult = accountController.Register(email, password);
                Assert.Fail(); 
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ValidationException>(ex);
            }
        }
        [Test]
        [TestCase("irf@uni-corvinus.hu", "TokeletesJelszo123")]
        public void TestRegisterWithMock(string email, string password)
        {
            var accountManagerMock = new Mock<IAccountManager>(MockBehavior.Strict);

           accountManagerMock
                .Setup(m => m.CreateAccount(It.IsAny<Account>()))
                .Returns<Account>(a => a);

            var accountController = new AccountController();
            accountController.AccountManager = accountManagerMock.Object;

            var actualResult = accountController.Register(email, password);

            Assert.AreEqual(email, actualResult.Email);
            Assert.AreEqual(password, actualResult.Password);
            Assert.AreNotEqual(Guid.Empty, actualResult.ID);

            accountManagerMock.Verify(m => m.CreateAccount(actualResult), Times.Once);
        }
    }
}
