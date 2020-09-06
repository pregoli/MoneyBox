using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var sourceAccount = _accountRepository.GetAccountById(fromAccountId);
            var targetAccount = _accountRepository.GetAccountById(toAccountId);

            if (sourceAccount.CanWithdraw(amount) && targetAccount.CanReceive(amount))
            { 
                sourceAccount.WithDraw(amount);
                targetAccount.Receive(amount);

                _accountRepository.Update(sourceAccount);
                _accountRepository.Update(targetAccount);
            }

            //If the condition is met We could emit an event to a consumer notifies the source account
            if (sourceAccount.LowFundsWarning)
                _notificationService.NotifyFundsLow(sourceAccount.User.Email);

            //If the condition is met We could emit an event to a consumer notifies the target account
            if (targetAccount.ApproachingPayInLimitWarning)
                _notificationService.NotifyApproachingPayInLimit(targetAccount.User.Email);
        }
    }
}
