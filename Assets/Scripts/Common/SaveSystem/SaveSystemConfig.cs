using System;

[Serializable]
public class SaveSystemConfig
{
	public string UsernameStorageKey = "Username";
	public string BaseUrl = "https://example.com";
	public string SubmitScoreRoute = "/api/scores";
	public string SubmitScoreUrl = "";
	public string AuthorizationHeader = "Authorization";
	public string AuthorizationScheme = "Bearer";
	public string AuthorizationToken = "";
	public int TimeoutSeconds = 15;
	public bool IncludeSaveDataFlag = true;
}
