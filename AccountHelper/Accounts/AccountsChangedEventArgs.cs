using AccountHelper.Models;
using System;

namespace AccountHelper.Accounts
{
    public class AccountsChangedEventArgs : EventArgs
    {
        public EditableAccount Account { get; set; }
        public bool EditMode { get; set; }
        public bool ChangeCanceled { get; set; }
    }
}
