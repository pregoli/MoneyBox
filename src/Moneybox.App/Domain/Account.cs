using System;

namespace Moneybox.App
{
    public sealed class Account
    {
        private Account() { }

        public Guid Id { get; private set; }

        public User User { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public void Receive(decimal amount)
        {
            Balance += amount;
            PaidIn += amount;
        }

        public bool CanReceive(decimal amount) => 
            PaidIn + amount > PayInLimit ? 
            throw new InvalidOperationException("Account pay in limit reached") : 
            true;

        public void WithDraw(decimal amount)
        {
            Balance -= amount;
            Withdrawn -= amount;
        }

        public bool CanWithdraw(decimal amount) =>
            Balance <= 0m || Balance - amount < 0m ? 
            throw new InvalidOperationException("Insufficient funds to make transfer") : 
            true;

        public bool LowFundsWarning => Balance < 500m; 
        public bool ApproachingPayInLimitWarning => PayInLimit - PaidIn < 500m;

        private const decimal PayInLimit = 4000m;

        public static Account New(User user, decimal balance, decimal withdrawn, decimal paidIn) => 
            new Account
            { 
                Id = Guid.NewGuid(),
                User = user,
                Balance = balance,
                Withdrawn = withdrawn,
                PaidIn = paidIn
            };
    }
}
