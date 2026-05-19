using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// MonoBehaviour facade for the save system.
/// Provides non-DI access to save/load operations.
/// </summary>
public class SaveSystemController : MonoBehaviour
{
	public SaveSystemConfig _config = new();

	private ICommonSaveSystem _saveSystem;


	private void Awake()
	{
		var store = new PlayerPrefsUsernameStore(_config);
		var apiClient = new HttpScoreApiClient(_config);
		_saveSystem = new CommonSaveSystemService(store, apiClient);
	}

	public void SaveUsername(string username)
	{
		_saveSystem.SetUsername(username);
	}

	public string GetUsername()
	{
		return _saveSystem.GetUsername();
	}

	public void ClearUsername()
	{
		_saveSystem.ClearUsername();
	}

	public bool HasUsername()
	{
		return _saveSystem.HasUsername();
	}

	public Task<bool> SubmitScoreAsync(int score)
	{
		return _saveSystem.SubmitScoreAsync(score);
	}

	public Task<bool> SubmitScoreAsync(string username, int score)
	{
		return _saveSystem.SubmitScoreAsync(username, score);
	}
}
