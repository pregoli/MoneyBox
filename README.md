
# MoneyBox.App

Refactored <a href="https://github.com/pregoli/MoneyBox/blob/master/src/Moneybox.App/Features/TransferMoney.cs" target="_blank">TransferMoney</a> and <a href="https://github.com/pregoli/MoneyBox/blob/master/src/Moneybox.App/Features/WithdrawMoney.cs" target="_blank">WithdrawMoney</a> features.

<a href="https://github.com/pregoli/MoneyBox/blob/master/src/Moneybox.App/Domain/Account.cs" target="_blank">Account</a> entity representation moved from Anemic to Rich Domain Model performing businesslogic and maintaining its internal state. 
</br>Kept free of any persistence or service layer dependency.

# MoneyBox.App.Tests
The <a href="https://github.com/pregoli/MoneyBox/tree/master/src/MoneyBox.App.Tests" target="_blank">Test project</a> runs unit tests against Domain and Services layer.

## Dependencies

   * <a href="https://nunit.org/" target="_blank">NUnit</a>
   * <a href="https://github.com/moq/moq4" target="_blank">Moq</a>


