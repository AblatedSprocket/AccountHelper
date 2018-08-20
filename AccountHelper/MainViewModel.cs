using AccountHelper.Accounts;
using AccountHelper.Models;
using AccountHelper.Utilities;
using CustomPresentationControls;
using CustomPresentationControls.FileExplorer;
using CustomPresentationControls.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace AccountHelper
{
    public interface IMainViewModel
    {
        RelayCommand SelectedAccountChangedCommand { get; }
        RelayCommand SelectedGroupingChangedCommand { get; }
    }
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        #region Fields
        private AccountType _selectedType;
        private string _user;
        private ObservableCollection<Account> _allAccounts;
        private IAccountRepository _accountRepository;
        private AddEditAccountViewModel _addEditAccountViewModel;
        private AccountDetailsViewModel _accountDetailsViewModel;
        private Account _selectedAccount;
        private ViewModel _currentViewModel;
        #endregion
        #region Properties
        public string User
        {
            get { return _user; }
            set { OnPropertyChanged(ref _user, value); }
        }
        public AccountType SelectedType
        {
            get { return _selectedType; }
            set { OnPropertyChanged(ref _selectedType, value); }
        }
        public ICollectionView CurrentAccounts
        {
            get
            {
                ICollectionView source = CollectionViewSource.GetDefaultView(_allAccounts);
                source.Filter = f =>
                {
                    return f is Account && (SelectedType == AccountType.All || (f as Account).Type == SelectedType);
                };
                return source;
            }
        }
        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set { OnPropertyChanged(ref _selectedAccount, value); }
        }
        public ViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set { OnPropertyChanged(ref _currentViewModel, value); }
        }
        #endregion
        #region Commands
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand ImportCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand SelectedAccountChangedCommand { get; }
        public RelayCommand SelectedGroupingChangedCommand { get; }
        #endregion
        public MainViewModel(string username)
        {
            User = username;
            _accountRepository = new AccountRepository(User);
            _allAccounts = _accountRepository.AccountCollection;
            _addEditAccountViewModel = new AddEditAccountViewModel();
            _addEditAccountViewModel.ReturnToMain += OnReturnedTo;
            SelectedAccount = _allAccounts.FirstOrDefault();
            _accountDetailsViewModel = new AccountDetailsViewModel();
            _accountDetailsViewModel.Account = SelectedAccount;
            CurrentViewModel = _accountDetailsViewModel;
            AddCommand = new RelayCommand(NavigateAddAccount);
            EditCommand = new RelayCommand(NavigateEditAccount, CanAct);
            ExportCommand = new RelayCommand(OnExport);
            ImportCommand = new RelayCommand(OnImport);
            RemoveCommand = new RelayCommand(RemoveAccount, CanAct);
            SelectedAccountChangedCommand = new RelayCommand(OnSelectedAccountChanged);
            SelectedGroupingChangedCommand = new RelayCommand(UpdateGrouping);
            UpdateGrouping();
        }
        #region Private Methods
        private bool CanAct()
        {
            return SelectedAccount != null && CurrentViewModel != _addEditAccountViewModel;
        }
        private EditableAccount ConvertToEditableACcount(Account account)
        {
            return new EditableAccount
            {
                Id = account.Id,
                Provider = account.Provider,
                Username = account.Username,
                Password = account.Password,
                Url = account.Url,
                Type = account.Type
            };
        }
        private Account ConvertToACcount(EditableAccount account)
        {
            return new Account
            {
                Id = account.Id,
                Provider = account.Provider,
                Username = account.Username,
                Password = account.Password,
                Url = account.Url,
                Type = account.Type
            };
        }
        private void NavigateAddAccount()
        {
            try
            {
                _addEditAccountViewModel.EditMode = false;
                _addEditAccountViewModel.Account = new EditableAccount();
                CurrentViewModel = _addEditAccountViewModel;
                EditCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        private void NavigateEditAccount()
        {
            try
            {
                _addEditAccountViewModel.EditMode = true;
                _addEditAccountViewModel.Account = ConvertToEditableACcount(SelectedAccount);
                CurrentViewModel = _addEditAccountViewModel;
                EditCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        private void OnExport()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                try
                {
                    if (dialog.Path != null && Directory.Exists(Path.GetDirectoryName(dialog.Path)))
                    {
                        string data = DataSerializer.SerializeToCsv(_allAccounts);
                        File.WriteAllText(dialog.Path, data);
                    }
                    CurrentViewModel = _accountDetailsViewModel;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        private void OnImport()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                try
                {
                    if (dialog.Path != null && Directory.Exists(Path.GetDirectoryName(dialog.Path)))
                    {
                        string data = File.ReadAllText(dialog.Path);
                        IEnumerable<Account> accounts = DataSerializer.DeserializeFromCsv<Account>(data);
                    }
                    CurrentViewModel = _accountDetailsViewModel;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        private void OnFileSelected(object sender, ItemSelectedEventArgs e)
        {

        }
        private void OnReturnedTo(object sender, AccountsChangedEventArgs e)
        {
            try
            {
                if (!e.ChangeCanceled)
                {
                    Account account = ConvertToACcount(e.Account);
                    if (e.EditMode)
                    {
                        Account listAccount = _allAccounts.FirstOrDefault(a => a.Id == account.Id);
                        if (listAccount != null)
                        {
                            _allAccounts[_allAccounts.IndexOf(listAccount)] = account;
                            SelectedAccount = account;
                        }
                    }
                    else
                    {
                        if (_allAccounts.Any())
                        {
                            account.Id = _allAccounts.Select(a => a.Id).Max() + 1;
                            _allAccounts.Add(account);
                            SelectedAccount = account;
                        }
                        else
                        {
                            account.Id = 0;
                            _allAccounts.Add(account);
                            SelectedAccount = account;
                        }
                    }
                }
                CurrentViewModel = _accountDetailsViewModel;
                EditCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        private void OnSelectedAccountChanged()
        {
            _accountDetailsViewModel.Account = SelectedAccount;
            CurrentViewModel = _accountDetailsViewModel;
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
        }
        private void RemoveAccount()
        {
            if (SelectedAccount != null)
            {
                _allAccounts.Remove(SelectedAccount);
                EditCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }
        private void UpdateGrouping()
        {
            CurrentAccounts.Refresh();
        }
        #endregion
    }
}
