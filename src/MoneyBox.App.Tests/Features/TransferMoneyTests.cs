using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using System;

namespace MoneyBox.App.Tests.Features
{
    [TestFixture(Author = "Paolo Regoli", Category = "Features", TestOf = typeof(TransferMoney))]
    public class TransferMoneyTests
    {
        private static readonly User _fakeSourceAccountUser = User.Load("paolo", "paolo.regoli@gmail.com");
        private static readonly User _fakeTargetAccountUser = User.Load("john", "john.regoli@gmail.com");

        private Mock<IAccountRepository> _accountRepository;
        private Mock<INotificationService> _notificationService;
        private TransferMoney _feature;

        [SetUp]
        public void SetUp()
        { 
            _accountRepository = new Mock<IAccountRepository>();
            _notificationService = new Mock<INotificationService>();
            _feature = new TransferMoney(_accountRepository.Object, _notificationService.Object);
        }

        [TestCase(1000, 700, 100, 100)]
        [TestCase(500, 200, 20, 50)]
        public void ShouldExecuteSuccessfully(decimal balance, decimal withdrawn, decimal paidIn, decimal amount)
        { 
            // Arrange
            var sourceAccount = SourceAccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(sourceAccount.Id))
                .Returns(sourceAccount);

            var targetAccount = TargetAccountFake(balance, paidIn);
            _accountRepository.Setup(x => x.GetAccountById(targetAccount.Id))
                .Returns(targetAccount);

            // Act
            _feature.Execute(sourceAccount.Id, targetAccount.Id, amount);

            // Assert
            _accountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
        }

        [TestCase(1000, 400, 100, 1100)]
        [TestCase(500, 200, 4000, 501)]
        public void ShouldExecuteUnsuccessfully(decimal balance, decimal withdrawn, decimal paidIn, decimal amount)
        { 
            // Arrange
            var sourceAccount = SourceAccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(sourceAccount.Id))
                .Returns(sourceAccount);

            var targetAccount = TargetAccountFake(balance, paidIn);
            _accountRepository.Setup(x => x.GetAccountById(targetAccount.Id))
                .Returns(targetAccount);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => _feature.Execute(sourceAccount.Id, targetAccount.Id, amount));
            _accountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [TestCase(1000, 700, 100, 1000)]
        [TestCase(500, 200, 20, 500)]
        public void SourceAccountShouldReceiveALowFundsNotification(decimal balance, decimal withdrawn, decimal paidIn, decimal amount)
        { 
            // Arrange
            var sourceAccount = SourceAccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(sourceAccount.Id))
                .Returns(sourceAccount);

            var targetAccount = TargetAccountFake(balance, paidIn);
            _accountRepository.Setup(x => x.GetAccountById(targetAccount.Id))
                .Returns(targetAccount);

            // Act
            _feature.Execute(sourceAccount.Id, targetAccount.Id, amount);

            // Assert
            _notificationService.Verify(x => x.NotifyFundsLow(sourceAccount.User.Email), Times.Exactly(1));
        }
        
        //[TestCase(1000, 700, 3401, 100)]
        [TestCase(500, 300, 3501, 190)]
        public void TargetAccountShouldReceiveALowFundsNotification(decimal balance, decimal withdrawn, decimal paidIn, decimal amount)
        { 
            // Arrange
            var sourceAccount = SourceAccountFake(balance, withdrawn);
            _accountRepository.Setup(x => x.GetAccountById(sourceAccount.Id))
                .Returns(sourceAccount);

            var targetAccount = TargetAccountFake(balance, paidIn);
            _accountRepository.Setup(x => x.GetAccountById(targetAccount.Id))
                .Returns(targetAccount);

            // Act
            _feature.Execute(sourceAccount.Id, targetAccount.Id, amount);

            // Assert
            _notificationService.Verify(x => x.NotifyApproachingPayInLimit(targetAccount.User.Email), Times.Exactly(1));
        }

        private Account SourceAccountFake(decimal balance, decimal withdrawn) =>
            Account.New(_fakeSourceAccountUser, balance, withdrawn, 0);
        
        private Account TargetAccountFake(decimal balance, decimal paidIn) =>
            Account.New(_fakeTargetAccountUser, balance, 0, paidIn);
    }
}
