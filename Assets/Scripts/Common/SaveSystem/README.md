# Common Save System (Username + Score API)

This folder is a reusable module for:
- Saving username locally (PlayerPrefs, WebGL-compatible)
- Sending score to API with auth token header
- DI-friendly architecture with Zenject support

## Included Components

### Interfaces
- `ILocalUsernameStore` - Local username storage contract
- `IScoreApiClient` - Score submission API contract
- `ICommonSaveSystem` - Combined username + score management
- `IUsernameService` - Username-specific operations

### Implementations
- `SaveSystemConfig` - Configuration for API and storage
- `PlayerPrefsUsernameStore` - Local storage using PlayerPrefs
- `HttpScoreApiClient` - HTTP client for score submission
- `CommonSaveSystemService` - Implements ICommonSaveSystem
- `UsernameService` - Implements IUsernameService
- `SaveSystemController` - MonoBehaviour facade for non-DI projects
- `SaveSystemInstaller` - Zenject installer for DI setup

## Request Body Sent To API
```json
{
  "username": "PlayerName",
  "score": 1234,
  "saveData": true
}
```

## Header
- `Authorization: Bearer <token>` by default
- Header name and scheme are configurable in `SaveSystemConfig`

## Usage Patterns

### Option 1: Using IUsernameService (DI)
```csharp
public class LoginUI : MonoBehaviour
{
    [Inject] private IUsernameService _usernameService;

    public void OnLoginButtonClicked(string username)
    {
        _usernameService.SetUsername(username);
    }

    private void Start()
    {
        string savedUsername = _usernameService.GetUsername();
        if (!string.IsNullOrEmpty(savedUsername))
            Debug.Log($"Welcome back, {savedUsername}!");
    }
}
```

### Option 2: Using ICommonSaveSystem (DI)
```csharp
public class GameManager : MonoBehaviour
{
    [Inject] private ICommonSaveSystem _saveSystem;

    public async void SubmitScore(int score)
    {
        bool success = await _saveSystem.SubmitScoreAsync(score);
        if (success)
            Debug.Log("Score submitted!");
    }
}
```

### Option 3: Using SaveSystemController (Non-DI)
```csharp
public class GameUI : MonoBehaviour
{
    [SerializeField] private SaveSystemController saveController;

    public async void OnSubmitScore(int score)
    {
        await saveController.SubmitScoreAsync(score);
    }
}
```

### Option 4: DI Setup with SaveSystemInstaller
```csharp
// In your scene, add SaveSystemInstaller MonoBehaviour to a GameObject
// and configure the SaveSystemConfig in the inspector

// Then inject into your scripts:
[Inject] private IUsernameService _usernameService;
[Inject] private ICommonSaveSystem _saveSystem;
```

## Quick Start

### Non-DI Usage (SaveSystemController)
1. Add `SaveSystemController` MonoBehaviour to a GameObject
2. Assign `SaveSystemConfig` in the inspector
3. Reference it in your scripts:
```csharp
[SerializeField] private SaveSystemController saveController;
saveController.SaveUsername("PlayerName");
await saveController.SubmitScoreAsync(100);
```

### DI Usage (Zenject)
1. Create `SaveSystemInstaller` on a GameObject in your scene
2. Assign `SaveSystemConfig` in the installer
3. Inject interfaces into your scripts:
```csharp
[Inject] private IUsernameService _usernameService;
[Inject] private ICommonSaveSystem _saveSystem;

// Use them in your code
_usernameService.SetUsername("PlayerName");
await _saveSystem.SubmitScoreAsync(100);
```

## Unity Export Package

## Unity Export Package
1. In Unity, open `Assets > Export Package...`
2. Select this folder: `Assets/Scripts/Common/SaveSystem`
3. Include dependencies if needed (`UniTask`, `Zenject` adapters in your project)
4. Export `.unitypackage`

## Notes
- Username is the only local data persisted.
- Do not store secrets in PlayerPrefs.
- Configure API route and token in `GameConfigInstaller` via `SaveSystemConfig`.
