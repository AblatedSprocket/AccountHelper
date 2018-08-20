using AccountHelper.Models;
using CustomPresentationControls.Utilities;
using System;

namespace AccountHelper.Accounts
{
    public class AddEditAccountViewModel : ViewModel
    {
        private bool _editMode;
        private EditableAccount _account;
        public bool EditMode
        {
            get { return _editMode; }
            set { OnPropertyChanged(ref _editMode, value); }
        }
        public EditableAccount Account
        {
            get { return _account; }
            set { OnPropertyChanged(ref _account, value); }
        }
        public event EventHandler<AccountsChangedEventArgs> ReturnToMain = delegate { };
        public RelayCommand CancelCommand { get; }
        public RelayCommand CommitCommand { get; }
        public AddEditAccountViewModel()
        {
            CancelCommand = new RelayCommand(OnCancel);
            CommitCommand = new RelayCommand(OnCommit);
        }
        private void OnCancel()
        {
            ReturnToMain(this, new AccountsChangedEventArgs
            {
                ChangeCanceled = true
            });
        }
        private void OnCommit()
        {
            ReturnToMain(this, new AccountsChangedEventArgs
            {
                Account = Account,
                ChangeCanceled = false,
                EditMode = EditMode
            });
        }
    }
}
