using CustomPresentationControls.Utilities;

namespace AccountHelper.Models
{
    public class EditableAccount : ValidatableObservableObject
    {
        private int _id;
        private string _provider;
        private string _username;
        private string _password;
        private string _url;
        private AccountType _type;
        public int Id
        {
            get { return _id; }
            set { OnPropertyChanged(ref _id, value); }
        }
        public string Provider
        {
            get { return _provider; }
            set { OnPropertyChanged(ref _provider, value); }
        }
        public string Username
        {
            get { return _username; }
            set { OnPropertyChanged(ref _username, value); }
        }
        public string Password
        {
            get { return _password; }
            set { OnPropertyChanged(ref _password, value); }
        }
        public string Url
        {
            get { return _url; }
            set { OnPropertyChanged(ref _url, value); }
        }
        public AccountType Type
        {
            get { return _type; }
            set { OnPropertyChanged(ref _type, value); }
        }
    }
}
