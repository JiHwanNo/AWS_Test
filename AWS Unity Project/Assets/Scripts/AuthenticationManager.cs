using Amazon.CognitoIdentityProvider;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class AuthenticationManager : MonoBehaviour
{

    public static string CachePath;

    // In production, should probably keep these in a config file
    private const string AppClientID = "4bcnhb47vji4ps4q91ou3vhb5b"; // App client ID, found under App Client Settings
    private const string AuthCognitoDomainPrefix = "jihwan-world-test"; // Found under App Integration -> Domain Name. Changing this means it must be updated in all linked Social providers redirect and javascript origins
    private const string RedirectUrl = "unitydl://yosulkong.com/";

    private const string Region = "ap-northeast-2"; // Update with the AWS Region that contains your services

    private const string AuthCodeGrantType = "authorization_code";
    private const string RefreshTokenGrantType = "refresh_token";
    private const string CognitoAuthUrl = ".auth." + Region + ".amazoncognito.com";
    private const string TokenEndpointPath = "/oauth2/token";


    // Token Holder
    public static string jwt;
    public static bool loginSuccessful;

    // Create an Identity Provider
    AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient
        (new Amazon.Runtime.AnonymousAWSCredentials(), CredentialsManager._region);

    void Awake()
    {
        CachePath = Application.persistentDataPath;
        //Debug.LogError("CachePath: " + CachePath);
    }


    public string GetLoginUrl()
    {
        // DOCS: https://docs.aws.amazon.com/cognito/latest/developerguide/login-endpoint.html
        string loginUrl = "https://" + AuthCognitoDomainPrefix + CognitoAuthUrl
         + "/login?response_type=code&client_id="
         + AppClientID + "&redirect_uri=" + RedirectUrl +
         "&state=STATE" +
         "&scope=aws.cognito.signin.user.admin+email+openid+phone+profile";

        return loginUrl;
    }



    public async void ProcessDeepLink(string deepLinkUrl)
    {
        // TODO: add some validation

        // Debug.Log("UIInputManager.ProcessDeepLink: " + deepLinkUrl);
        bool exchangeSuccess = await ExchangeAuthCodeForAccessToken(deepLinkUrl); //코드를 토큰으로 바꾸는 함수 호출

    }


    public async Task<bool> CallRefreshTokenEndpoint()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);

        string preservedRefreshToken = "";

        if (userSessionCache != null && userSessionCache._refreshToken != null && userSessionCache._refreshToken != "")
        {
            // DOCS: https://docs.aws.amazon.com/cognito/latest/developerguide/token-endpoint.html
            string refreshTokenUrl = "https://" + AuthCognitoDomainPrefix + CognitoAuthUrl + TokenEndpointPath;
            // Debug.Log(refreshTokenUrl);

            preservedRefreshToken = userSessionCache._refreshToken;

            WWWForm form = new WWWForm();
            form.AddField("grant_type", RefreshTokenGrantType);
            form.AddField("client_id", AppClientID);
            form.AddField("refresh_token", userSessionCache._refreshToken);

            UnityWebRequest webRequest = UnityWebRequest.Post(refreshTokenUrl, form);
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            await webRequest.SendWebRequest();


            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Refresh token call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
                // clear out invalid user session data to force re-authentication
                ClearUserSessionData();
                webRequest.Dispose();
            }
            else
            {
                Debug.Log("Success, Refresh token call complete!");
                // Debug.Log(webRequest.downloadHandler.text);

                BADAuthenticationResultType authenticationResultType = JsonUtility.FromJson<BADAuthenticationResultType>(webRequest.downloadHandler.text);

                // token endpoint to get refreshed access token does NOT return the refresh token, so manually save it from before.
                authenticationResultType.refresh_token = preservedRefreshToken;

                _userid = AuthUtilities.GetUserSubFromIdToken(authenticationResultType.id_token);

                // update session cache
                SaveDataManager.SaveJsonData(new UserSessionCache(authenticationResultType, _userid));
                webRequest.Dispose();
                return true;
            }
        }
        return false;
    }

    private void ClearUserSessionData()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.SaveJsonData(userSessionCache);
    }
    public async Task<bool> ExchangeAuthCodeForAccessToken(string rawUrlWithGrantCode)
    {
        
        string allQueryParams = rawUrlWithGrantCode.Split('?')[1];

        // it's likely there won't be more than one param
        string[] paramsSplit = allQueryParams.Split('&');

        foreach (string param in paramsSplit)
        {
            // Debug.Log("param: " + param);

            // find the code parameter and its value
            if (param.StartsWith("code"))
            {
                string grantCode = param.Split('=')[1];
                string grantCodeCleaned = grantCode.removeAllNonAlphanumericCharsExceptDashes(); // sometimes the url has a # at the end of the string
                return await CallCodeExchangeEndpoint(grantCodeCleaned);
            }
            else
            {
                Debug.Log("Code not found");
            }
        }
        return false;
    }


    private static string _userid = "";
    private async Task<bool> CallCodeExchangeEndpoint(string grantCode)
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", AuthCodeGrantType);
        form.AddField("client_id", AppClientID);
        form.AddField("code", grantCode);
        form.AddField("redirect_uri", RedirectUrl);

        // DOCS: https://docs.aws.amazon.com/cognito/latest/developerguide/token-endpoint.html
        string requestPath = "https://" + AuthCognitoDomainPrefix + CognitoAuthUrl + TokenEndpointPath;

        UnityWebRequest webRequest = UnityWebRequest.Post(requestPath, form);
        await webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Code exchange failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
            webRequest.Dispose();
        }
        else
        {
            Debug.Log("Success, Code exchange complete!");

            BADAuthenticationResultType authenticationResultType = JsonUtility.FromJson<BADAuthenticationResultType>(webRequest.downloadHandler.text);
            
            _userid = AuthUtilities.GetUserSubFromIdToken(authenticationResultType.id_token);

            // update session cache
            SaveDataManager.SaveJsonData(new UserSessionCache(authenticationResultType, _userid));
            webRequest.Dispose();
            return true;
        }
        return false;
    }

    public string GetIdToken()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);
        return userSessionCache.getIdToken();
    }
}
