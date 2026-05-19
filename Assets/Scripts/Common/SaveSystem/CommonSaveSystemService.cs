using System.Threading.Tasks;
public class CommonSaveSystemService : ICommonSaveSystem
{
	private readonly IScoreApiClient _scoreApiClient;
	private readonly ILocalUsernameStore _usernameStore;

	public CommonSaveSystemService(ILocalUsernameStore usernameStore, IScoreApiClient scoreApiClient)
	{
		_usernameStore = usernameStore;
		_scoreApiClient = scoreApiClient;
	}

	public string GetUsername()
	{
		return _usernameStore.GetUsername();
	}

	public void SetUsername(string username)
	{
		_usernameStore.SetUsername(username);
	}

	public void ClearUsername()
	{
		_usernameStore.ClearUsername();
	}

	public bool HasUsername()
	{
		return _usernameStore.HasUsername();
	}

	public Task<bool> SubmitScoreAsync(int score)
	{
		var username = _usernameStore.GetUsername();
		return SubmitScoreAsync(username, score);
	}

	public Task<bool> SubmitScoreAsync(string username, int score)
	{
		return _scoreApiClient.SendScoreAsync(username, score);
	}
}
