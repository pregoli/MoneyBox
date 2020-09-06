using Moneybox.App;
using NUnit.Framework;
using System;

namespace MoneyBox.App.Tests.Domain
{
    [TestFixture(Author = "Paolo Regoli", Category = "Domain", TestOf = typeof(Account))]
    public class AccountTests
    {
        private static readonly User _fakeUser = User.Load("paolo", "paolo.regoli@gmail.com");

        #region Account Load

        [TestCase(30, 20, 10)]
        public void AccountShouldBeLoaded(decimal balance, decimal withdrawn, decimal paidIn)
        {
            //Act
            var account = Account.New(_fakeUser, balance, withdrawn, paidIn);

            //Assert
            Assert.AreEqual(account.Balance, balance);
            Assert.AreEqual(account.Withdrawn, withdrawn);
            Assert.AreEqual(account.PaidIn, paidIn);
            Assert.AreEqual(account.User, _fakeUser);
        }

        #endregion

        #region Account Withdrawl

        [TestCase(10,11)]
        [TestCase(0,0)]
        public void ShouldThrowAnExceptionForAnInvalidWithdrawl(decimal balance, decimal amount)
        {
            //Arrange
            var account = Account.New(_fakeUser, balance, 0, 0);

            //Act + Assert
            Assert.Throws<InvalidOperationException>(() => account.CanWithdraw(amount));
        }

        [TestCase(11,10)]
        [TestCase(2,1)]
        public void ShouldValidateTheWithdrawl(decimal balance, decimal amount)
        {
            //Arrange
            var account = Account.New(_fakeUser, balance, 0, 0);

            //Act + Assert
            Assert.True(account.CanWithdraw(amount));
        }
        
        [TestCase(100, 50, 10, 90, 40)]
        [TestCase(10, 7, 2, 8, 5)]
        public void Withdrawl(
            decimal balance,
            decimal withdrawn,
            decimal amount,
            decimal expectedBalance,
            decimal expectedWithdrawn)
        {
            //Arrange
            var account = Account.New(_fakeUser, balance, withdrawn, 0);

            // Act
            account.WithDraw(amount);

            //Act + Assert
            Assert.AreEqual(account.Balance, expectedBalance);
            Assert.AreEqual(account.Withdrawn, expectedWithdrawn);
        }

        [TestCase(499)]
        [TestCase(10)]
        public void ShouldReceiveALowFundsNotification(decimal balance)
        {
            //Arrange
            var account = Account.New(_fakeUser, balance, 0, 0);

            //Act + Assert
            Assert.True(account.LowFundsWarning);
        }

        [TestCase(500)]
        [TestCase(501)]
        public void ShouldNotReceiveALowFundsNotification(decimal balance)
        {
            //Arrange
            var account = Account.New(_fakeUser, balance, 0, 0);

            //Act + Assert
            Assert.False(account.LowFundsWarning);
        }

        #endregion

        #region Account PaidIn

        [TestCase(3000, 1001)]
        [TestCase(0, 4001)]
        public void ShouldThrowAnExceptionForAnInvalidPayIn(decimal paidIn, decimal amount)
        {
            //Arrange
            var account = Account.New(_fakeUser, 0, 0, paidIn);

            //Act + Assert
            Assert.Throws<InvalidOperationException>(() => account.CanReceive(amount));
        }
        
        [TestCase(3000, 1000)]
        [TestCase(0, 4000)]
        public void ShouldValidateThePayIn(decimal paidIn, decimal amount)
        {
            //Arrange
            var account = Account.New(_fakeUser, 0, 0, paidIn);

            //Act + Assert
            Assert.True(account.CanReceive(amount));
        }

        [TestCase(100, 50, 10, 110, 60)]
        [TestCase(10, 7, 2, 12, 9)]
        public void Receive(
            decimal balance,
            decimal paidIn,
            decimal amount,
            decimal expectedBalance,
            decimal expectedPaidIn)
        {
            //Arrange
            var account = Account.New(_fakeUser, balance, 0, paidIn);

            // Act
            account.Receive(amount);

            //Act + Assert
            Assert.AreEqual(account.Balance, expectedBalance);
            Assert.AreEqual(account.PaidIn, expectedPaidIn);
        }

        [TestCase(3501)]
        [TestCase(4000)]
        public void ShouldReceiveAApproachingPayInLimitNotification(decimal paidIn)
        {
            //Arrange
            var account = Account.New(_fakeUser, 0, 0, paidIn);

            //Act + Assert
            Assert.True(account.ApproachingPayInLimitWarning);
        }

        [TestCase(3500)]
        [TestCase(1000)]
        public void ShouldNotReceiveAApproachingPayInLimitNotification(decimal paidIn)
        {
            //Arrange
            var account = Account.New(_fakeUser, 0, 0, paidIn);

            //Act + Assert
            Assert.False(account.ApproachingPayInLimitWarning);
        }

        #endregion
    }
}
