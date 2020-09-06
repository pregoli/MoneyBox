using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this._accountRepository = accountRepository;
            this._notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var account = _accountRepository.GetAccountById(fromAccountId);
            if (account.CanWithdraw(amount))
            {
                account.WithDraw(amount);
                _accountRepository.Update(account);
            }
            
            //If the condition is met We could emit an event to a consumer notifies the source account
            if (account.LowFundsWarning)
                _notificationService.NotifyFundsLow(account.User.Email);
        }
    }
}
