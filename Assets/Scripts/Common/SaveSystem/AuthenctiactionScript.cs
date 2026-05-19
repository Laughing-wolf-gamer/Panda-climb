using UnityEngine;
using Solitaire.Common.SaveSystem;

public class AuthenctiactionScript : MonoBehaviour
{
	[SerializeField] private GameObject loginView;
	[SerializeField] private string username;
	[SerializeField] private SaveSystemController saveSystemController;
	private IUsernameService _usernameService;
	private void Awake()
	{
		var usernameStore = new PlayerPrefsUsernameStore(saveSystemController._config);
		_usernameService = new UsernameService(usernameStore);
	}
	public void SetUserName(string value)
	{
		username = value;
	}

	private void Start()
	{
		username = _usernameService.GetUsername();
		ToggleLoginView(string.IsNullOrWhiteSpace(username));
	}

	public void SaveUsername()
	{
		if (string.IsNullOrEmpty(username))
			return;

		_usernameService.SetUsername(username);
		ToggleLoginView(false);
	}

	public string GetSavedUsername()
	{
		return _usernameService.GetUsername();
	}

	public void ClearSavedUsername()
	{
		_usernameService.ClearUsername();
		username = string.Empty;
		ToggleLoginView(true);
	}

	private void ToggleLoginView(bool isVisible)
	{
		if (loginView != null)
			loginView.SetActive(isVisible);
	}
}
