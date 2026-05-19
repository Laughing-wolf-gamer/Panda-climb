using UnityEngine;

public class PlayerPrefsUsernameStore : ILocalUsernameStore
{
	private readonly SaveSystemConfig _config;

	public PlayerPrefsUsernameStore(SaveSystemConfig config)
	{
		_config = config;
	}

	public string GetUsername()
	{
		return PlayerPrefs.GetString(_config.UsernameStorageKey, string.Empty);
	}

	public void SetUsername(string username)
	{
		if (string.IsNullOrWhiteSpace(username))
			return;

		PlayerPrefs.SetString(_config.UsernameStorageKey, username.Trim());
		PlayerPrefs.Save();
	}

	public void ClearUsername()
	{
		PlayerPrefs.DeleteKey(_config.UsernameStorageKey);
		PlayerPrefs.Save();
	}

	public bool HasUsername()
	{
		return !string.IsNullOrWhiteSpace(GetUsername());
	}
}
