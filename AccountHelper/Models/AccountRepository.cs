using AccountHelper.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace AccountHelper.Models
{
    public class AccountRepository : IAccountRepository, INotifyPropertyChanged
    {
        private ObservableCollection<Account> _accountCollection;
        private readonly string _saveDirectory;
        public ObservableCollection<Account> AccountCollection
        {
            get { return _accountCollection; }
            set { OnPropertyChanged(ref _accountCollection, value); }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public AccountRepository(string user)
        {
            _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", $"{user}.acc");
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
            string serializedData = Encryptor.DecryptFile(_saveDirectory);
            if (serializedData != null)
            {
                List<Account> accounts = DataSerializer.DeserializeFromXml<List<Account>>(serializedData);
                int i = 0;
                foreach (Account account in accounts)
                {
                    account.Id = ++i;
                }
                AccountCollection = new ObservableCollection<Account>(accounts);
                //AccountCollection = new ObservableCollection<Account>(DataSerializer.DeserializeFromXml<List<Account>>(serializedData));
            }
            else
            {
                AccountCollection = new ObservableCollection<Account>();
            }
            AccountCollection.CollectionChanged += OnCollectionChanged;
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SaveAccountData();
        }
        private void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private void SaveAccountData()
        {
            
            Encryptor.WriteEncryptedFile(_saveDirectory, DataSerializer.SerializeToXml(new List<Account>(_accountCollection)));
        }
    }
}
