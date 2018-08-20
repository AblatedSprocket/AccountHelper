using System;
using System.Collections.Generic;

namespace AccountHelper.Models
{
    public enum AccountType
    {
        All,
        Bill,
        Personal,
        Work
    }
    public class AccountTypeProvider
    {
        public static IEnumerable<AccountType> Filter()
        {
            foreach (AccountType type in Enum.GetValues(typeof(AccountType)))
            {
                if (type != AccountType.All)
                {
                    yield return type;
                }
            }
        }
    }
}
