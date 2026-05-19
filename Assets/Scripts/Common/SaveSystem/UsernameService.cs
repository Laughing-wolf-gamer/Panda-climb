namespace Solitaire.Common.SaveSystem
{
    /// <summary>
    /// Implementation of IUsernameService that delegates to ILocalUsernameStore.
    /// Provides a focused interface for username management operations.
    /// </summary>
    public class UsernameService : IUsernameService
    {
        private readonly ILocalUsernameStore _usernameStore;

        public UsernameService(ILocalUsernameStore usernameStore)
        {
            _usernameStore = usernameStore;
        }

        public string GetUsername()
        {
            return _usernameStore.GetUsername();
        }

        public void SetUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return;

            _usernameStore.SetUsername(username);
        }

        public void ClearUsername()
        {
            _usernameStore.ClearUsername();
        }
    }
}
