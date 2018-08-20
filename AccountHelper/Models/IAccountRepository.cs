
using System.Collections.ObjectModel;

namespace AccountHelper.Models
{
    public interface IAccountRepository
    {
        ObservableCollection<Account> AccountCollection { get; }
    }
}