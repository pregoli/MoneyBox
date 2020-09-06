using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using System;

namespace MoneyBox.App.Tests.Features
{
    [TestFixture(Author = "Paolo Regoli", Category = "Features", TestOf = typeof(WithdrawMoney))]
    public class WithdrawMoneyTests
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<INotificationService> _notificationService;
        private WithdrawMoney _feature;

        [SetUp]
        public void SetUp()
        { 
            _accountRepository = new Mock<IAccountRepository>();
            _notificationService = new Mock<INotificationService>();
            _feature = new WithdrawMoney(_accountRepository.Object, _notificationService.Object);
        }

        [TestCase(1000, 700, 100)]
        [TestCase(500, 200, 50)]
        public void ShouldExecuteSuccessfully(decimal balance, decimal withdrawn, decimal amount)
        { 
            // Arrange
            var account = AccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(account.Id))
                .Returns(account);

            // Act
            _feature.Execute(account.Id, amount);

            // Assert
            _accountRepository.Verify(x => x.Update(account), Times.Exactly(1));
        }
        
        [TestCase(1000, 400, 1100)]
        [TestCase(500, 200, 501)]
        public void ShouldExecuteUnsuccessfully(decimal balance, decimal withdrawn, decimal amount)
        { 
            // Arrange
            var account = AccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(account.Id))
                .Returns(account);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => _feature.Execute(account.Id, amount));
            _accountRepository.Verify(x => x.Update(account), Times.Never);
        }

        [TestCase(200, 100, 100)]
        [TestCase(500, 200, 50)]
        public void ShouldReceiveALowFundsNotification(decimal balance, decimal withdrawn, decimal amount)
        { 
            // Arrange
           var account = AccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(account.Id))
                .Returns(account);

            // Act
            _feature.Execute(account.Id, amount);

            // Assert
            _notificationService.Verify(x => x.NotifyFundsLow(account.User.Email), Times.Exactly(1));
        }

        private Account AccountFake(decimal balance, decimal withdrawn) =>
            Account.New(User.Load("paolo","paolo.regoli@gmail.com"), balance, withdrawn, 0);
    }
}
