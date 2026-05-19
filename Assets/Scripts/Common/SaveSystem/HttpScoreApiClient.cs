using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public class HttpScoreApiClient : IScoreApiClient
{
	private readonly SaveSystemConfig _config;

	public HttpScoreApiClient(SaveSystemConfig config)
	{
		_config = config;
	}

	public async Task<bool> SendScoreAsync(string username, int score)
	{
		var endpoint = BuildSubmitScoreEndpoint();

		Debug.Log($"Submitting score: {username} - {score} - {endpoint}");
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(endpoint))
			return false;

		var requestBody = new ScoreRequest
		{
			username = username,
			score = score,
			saveData = _config.IncludeSaveDataFlag
		};

		var json = JsonUtility.ToJson(requestBody);
		var rawBody = Encoding.UTF8.GetBytes(json);

		using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
		request.uploadHandler = new UploadHandlerRaw(rawBody);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.timeout = Mathf.Max(_config.TimeoutSeconds, 1);
		request.SetRequestHeader("Content-Type", "application/json");

		if (!string.IsNullOrWhiteSpace(_config.AuthorizationToken))
		{
			var headerValue = string.IsNullOrWhiteSpace(_config.AuthorizationScheme)
				? _config.AuthorizationToken
				: $"{_config.AuthorizationScheme} {_config.AuthorizationToken}";

			request.SetRequestHeader(_config.AuthorizationHeader, headerValue);
		}

		await request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
			return true;

		Debug.LogWarning($"Score submit failed ({request.responseCode}): {request.error}");
		return false;
	}

	private string BuildSubmitScoreEndpoint()
	{
		if (!string.IsNullOrWhiteSpace(_config.SubmitScoreUrl))
			return _config.SubmitScoreUrl.Trim();

		if (string.IsNullOrWhiteSpace(_config.BaseUrl))
			return string.Empty;

		var baseUrl = _config.BaseUrl.TrimEnd('/');
		var route = (_config.SubmitScoreRoute ?? string.Empty).Trim();

		if (!route.StartsWith("/"))
			route = "/" + route;

		return baseUrl + route;
	}

	[Serializable]
	private class ScoreRequest
	{
		public string username;
		public int score;
		public bool saveData;
	}
}
