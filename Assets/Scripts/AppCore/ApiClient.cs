using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using VContainer;

public class ApiClient : MonoBehaviour
{
    [SerializeField] private string testUsername;
    [SerializeField] private string testPassword;
    [Space(10)]
    [SerializeField] private int testLevel;
    [SerializeField] private int testSublevel;
    [Space(10)]
    [SerializeField] private int testExperience;
    [SerializeField] private int testGold;
    [SerializeField] private int testDiamond;
    [Space(10)]
    [SerializeField] private string testMatchToken;
    [SerializeField] private int testPlayer2;
    [SerializeField] private int testPlayer1Score;
    [SerializeField] private int testPlayer2Score;
    [SerializeField] private int testPlayer1League;
    [SerializeField] private int testPlayer2League;
    [SerializeField] private int testNextLeague;
    [SerializeField] private string testBotName;

    [FoldoutGroup(FoldoutGroupNames.DynamicDebug)]
    [Inject] private IAuthService authService;

    [FoldoutGroup(FoldoutGroupNames.DynamicDebug)]
    [Inject] private ICampaignService campaignService;

    [FoldoutGroup(FoldoutGroupNames.DynamicDebug)]
    [Inject] private IPlayerStatsService playerStatsService;

    [FoldoutGroup(FoldoutGroupNames.DynamicDebug)]
    [Inject] private IMultiplayerService multiplayerService;

    [FoldoutGroup(FoldoutGroupNames.DynamicDebug)]
    [Inject] private ApiAccessToken apiAccessToken;

    private string username;
    private string password;

    #region Login
    [Button]
    public void Click_LogInTest()
    {
        //LoadingScreen.SetActive(true);
        //BTN_LogIn.interactable = false;
        //TXT_ErrorMessage.gameObject.SetActive(false);
        username = testUsername;
        password = testPassword;

        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_LogInTest)} attempting to log in with " +
                  $"username = {username}, password = {password}");

        LoginAsync(new LoginModel(username)).Forget();
    }

    private async UniTaskVoid LoginAsync(LoginModel loginModel)
    {
        // TODO: use usecase
        var (success, errorInfo) = await authService.LoginAsync(loginModel);
        if (success)
        {
            FinishLogIn(username).Forget();
        }
        else
        {
            ShowError(errorInfo.DisplayMessage);
        }
    }

    private async UniTaskVoid FinishLogIn(string loggedInUsername)
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(FinishLogIn)} username = {loggedInUsername}");

        #region Extra user data (not using in this app)
        //ClientPreferences.UserName = loggedInUsername;
        //ClientController.Instance.ClientGameController.User.UserData.UserName = ClientPreferences.DisplayName;

        //var memberData = await authService.GetMemberData(loggedInUsername);

        //if (memberData != null)
        //{
        //    ClientPreferences.FirstName = memberData.FirstName;
        //    ClientPreferences.LastName = memberData.LastName;
        //    ClientPreferences.Title = memberData.Title;
        //    ClientPreferences.Email = memberData.Email;
        //}

        //Debug.Log($"{nameof(ApiClient)}::{nameof(FinishLogIn)} memberData = {memberData}");

        //loginSaver.SetLogin(username, password);

        //var rememberUsername = username;
        //ClientPreferences.RememberMe = rememberMeToggle.isOn;

        //if (!rememberMeToggle.isOn)
        //{
        //    rememberUsername = string.Empty;
        //}

        //ClientPreferences.LoginUsername = rememberUsername;
        #endregion

        //LoadingScreen.SetActive(false);
        //BTN_LogIn.interactable = true;
        //mainContentManager.SetMainMenu();
    }
    #endregion

    #region Campaign
    #region POST
    [Button]
    public void Click_CampaignPostTest()
    {
        CampaignData campaignData = new CampaignData(testLevel, testSublevel);
        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_CampaignPostTest)} campaignData= {campaignData}");

        PostCampaignAsync(campaignData).Forget();
    }

    private async UniTaskVoid PostCampaignAsync(CampaignData campaignData)
    {
        // TODO: use usecase
        var (success, errorInfo) = await campaignService.PostProgressAsync(campaignData);

        if (success)
        {
            FinishPostCampaign();
        }
        else
        {
            ShowError(errorInfo.DisplayMessage);
        }
    }

    private void FinishPostCampaign()
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(FinishPostCampaign)} campaign progress posted");
    }
    #endregion

    #region GET
    [Button]
    public async void Click_ProgressGetTest()
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_ProgressGetTest)}");

        ProgressData progressData = await GetProgressAsync();
        Debug.Log($"Progress data= {progressData}");
    }

    private async UniTask<ProgressData> GetProgressAsync()
    {
        return await campaignService.GetProgressAsync();
    }
    #endregion
    #endregion

    #region Player Stats
    #region POST
    [Button]
    public void Click_PlayerStatsPostTest()
    {
        PlayerStatsData playerStatsData = new PlayerStatsData(testExperience, testGold, testDiamond);
        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_CampaignPostTest)} playerStatsData= {playerStatsData}");

        UpdatePlayerStatsAsync(playerStatsData).Forget();
    }

    private async UniTaskVoid UpdatePlayerStatsAsync(PlayerStatsData playerStatsData)
    {
        // TODO: use usecase
        var (success, errorInfo) = await playerStatsService.UpdateStatsAsync(playerStatsData);

        if (success)
        {
            FinishUpdatePlayerStats();
        }
        else
        {
            ShowError(errorInfo.DisplayMessage);
        }
    }

    private void FinishUpdatePlayerStats()
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(FinishUpdatePlayerStats)} player stats updated");
    }
    #endregion

    #region GET
    [Button]
    public async void Click_PlayerStatsGetTest()
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_PlayerStatsGetTest)}");

        PlayerStatsData playerStatsData = await GetPlayerStatsAsync();
        Debug.Log($"Player stats data= {playerStatsData}");
    }

    private async UniTask<PlayerStatsData> GetPlayerStatsAsync()
    {
        return await playerStatsService.GetStatsAsync();
    }
    #endregion
    #endregion

    #region Multiplayer
    #region POST
    [Button]
    public void Click_MultiplayerMatchPostTest()
    {
        MultiplayerMatchData multiplayerMatchData = new MultiplayerMatchData(testMatchToken, 0, testPlayer1Score, testPlayer2Score, testPlayer1League, testPlayer2League, testNextLeague, testBotName);
        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_CampaignPostTest)} multiplayerMatchData= {multiplayerMatchData}");

        PostMultiplayerMatchAsync(multiplayerMatchData).Forget();
    }

    private async UniTaskVoid PostMultiplayerMatchAsync(MultiplayerMatchData multiplayerMatchData)
    {
        // TODO: use usecase
        var (success, errorInfo) = await multiplayerService.PostMultiplayerMatchAsync(multiplayerMatchData);

        if (success)
        {
            FinishPostMultiplayerMatch();
        }
        else
        {
            ShowError(errorInfo.DisplayMessage);
        }
    }

    private void FinishPostMultiplayerMatch()
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(FinishPostCampaign)} multiplayer match posted");
    }
    #endregion

    #region GET
    [Button]
    public async void Click_MultiplayerStatsGetTest()
    {
        Debug.Log($"{nameof(ApiClient)}::{nameof(Click_MultiplayerStatsGetTest)}");

        MultiplayerStatsData multiplayerStatsData = await GetMultiplayerStatsAsync();
        Debug.Log($"MultiplayerStats data= {multiplayerStatsData}");
    }

    private async UniTask<MultiplayerStatsData> GetMultiplayerStatsAsync()
    {
        return await multiplayerService.GetMultiplayerStatsAsync();
    }
    #endregion
    #endregion

    private void ShowError(string errorMessage)
    {
        //LoadingScreen.SetActive(false);
        //BTN_LogIn.interactable = true;
        //TXT_ErrorMessage.text = errorMessage;
        //TXT_ErrorMessage.gameObject.SetActive(true);
    }
}
