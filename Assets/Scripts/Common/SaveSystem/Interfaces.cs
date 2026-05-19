
using System.Threading.Tasks;
/// <summary>
/// Interface for local username storage operations.
/// </summary>
public interface ILocalUsernameStore
{
	string GetUsername();
	void SetUsername(string username);
	void ClearUsername();
	bool HasUsername();
}

public interface IScoreApiClient
{
	Task<bool> SendScoreAsync(string username, int score);
}

public interface ICommonSaveSystem
{
	string GetUsername();
	void SetUsername(string username);
	void ClearUsername();
	bool HasUsername();
	Task<bool> SubmitScoreAsync(int score);
	Task<bool> SubmitScoreAsync(string username, int score);
}

/// <summary>
/// Service for username-specific operations.
/// Provides a focused interface for username management.
/// </summary>
public interface IUsernameService
{
	string GetUsername();
	void SetUsername(string username);
	void ClearUsername();
}