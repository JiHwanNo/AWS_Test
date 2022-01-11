using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Login_Test : MonoBehaviour
{
    
    public static string CachePath;

    // In production, should probably keep these in a config file
    private const string AppClientID = "4bcnhb47vji4ps4q91ou3vhb5b"; // App client ID, found under App Client Settings
    private const string AuthCognitoDomainPrefix = "jihwan-world-test"; // Found under App Integration -> Domain Name. Changing this means it must be updated in all linked Social providers redirect and javascript origins
    private const string RedirectUrl = "https://www.google.com";
    private const string Region = "ap-northeast-2"; // Update with the AWS Region that contains your services

    private const string AuthCodeGrantType  = "authorization_code";
    private const string RefreshTokenGrantType = "refresh_token";
    private const string CognitoAuthUrl = ".auth." + Region + ".amazoncognito.com";
    private const string TokenEndpointPath = "/oauth2/token";


    // Token Holder
    public static string jwt;
    public static bool loginSuccessful;
    // Create an Identity Provider
    AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient
        (new Amazon.Runtime.AnonymousAWSCredentials(), CredentialsManager._region);

    void Start()
    {
        string loginUrl = GetLoginUrl();
        Application.OpenURL(loginUrl);
    }

    public string GetLoginUrl()
    {
        // DOCS: https://docs.aws.amazon.com/cognito/latest/developerguide/login-endpoint.html
        string loginUrl = "https://" + AuthCognitoDomainPrefix + CognitoAuthUrl
           + "/login?response_type=code&client_id="
           + AppClientID + "&redirect_uri=" + RedirectUrl;
        return loginUrl;
    }

    public async void ProcessDeepLink(string deepLinkUrl)
    {
        // TODO: add some validation

        // Debug.Log("UIInputManager.ProcessDeepLink: " + deepLinkUrl);
        bool exchangeSuccess = await ExchangeAuthCodeForAccessToken(deepLinkUrl);

    }
    public async Task<bool> ExchangeAuthCodeForAccessToken(string rawUrlWithGrantCode)
    {
        // Debug.Log("rawUrlWithGrantCode: " + rawUrlWithGrantCode);

        // raw url looks like https://somedomain.com/?code=c91d8bf4-1cb6-46e5-b43a-8def466f3c55
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
            // Debug.Log("ID token: " + authenticationResultType.id_token);

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
