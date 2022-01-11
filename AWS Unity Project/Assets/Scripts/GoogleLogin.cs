// Text UI 사용
// 구글 플레이 연동
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.Extensions.CognitoAuthentication;

public class GoogleLogin : MonoBehaviour
{

    // Create an Identity Provider

    // Create an Identity Provider
    AmazonCognitoIdentityProviderClient provider;

    string token = string.Empty;

    void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
           .Builder()
           .RequestEmail()                 // 이메일 요청    
           .RequestIdToken()               // 토큰 요청
           .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        UnityEngine.Debug.Log("Social.localUser.Authenticate");

        OnLogin();

    }


    public void OnLogin()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool bSuccess) =>
            {
                if (bSuccess)
                {
                    UnityEngine.Debug.Log("Social.Login Successful");
                    
                    StartCoroutine(LoginByGoogle());
                    Debug.Log("token " + token);
                    //Debug.LogError(CredentialsManager._credentials.AccountId);
                    //Debug.LogError("Step 1");
                    //CredentialsManager._credentials.ContainsProvider("accounts.google.com");
                    
                    ////CredentialsManager._credentials.AddLogin("accounts.google.com", token); // providername, token
                    //Debug.LogError("Step 2");
                    //provider = new AmazonCognitoIdentityProviderClient(CredentialsManager._credentials, CredentialsManager._region);
                    //Debug.LogError("Step 3");
                    //Debug.LogError(CredentialsManager._credentials.IdentityPoolId);
                    //Debug.LogError(CredentialsManager._credentials.CurrentLoginProviders[0]);
                    //Debug.LogError("Step 4");
                    Signup();
                }
                else
                {
                    Debug.Log("Fall");
                }
            });
        }
    }

    public void OnLogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }


    public IEnumerator LoginByGoogle()
    {
        Debug.Log("Google Login ");
        token = PlayGamesPlatform.Instance.GetIdToken();

        Debug.Log("first token  " + token);
        while (string.IsNullOrEmpty(token))
        {
            token = PlayGamesPlatform.Instance.GetIdToken();
            yield return new WaitForSeconds(0.5f);
        }

    }
    public void Signup()
    {
        _ = Signup_Method_Async();
    }

    private async Task Signup_Method_Async()
    {
        string userName = PlayGamesPlatform.Instance.GetUserDisplayName();
        string passWord = token.Split('0')[0] + "!";
        string email = PlayGamesPlatform.Instance.GetUserEmail();

        SignUpRequest signUpRequest = new SignUpRequest()
        {
            ClientId = "Google_" + PlayGamesPlatform.Instance.GetUserId(),
            Username = email,
            Password = passWord
        };

        List<AttributeType> attributes = new List<AttributeType>()
        {
            new AttributeType(){Name = "email", Value = email}
        };

        signUpRequest.UserAttributes = attributes;
        try
        {
            SignUpResponse request = await provider.SignUpAsync(signUpRequest);
            Debug.LogError("Sign up worked");

            // Send Login Event
            Events.Call_Signup();
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e);
            return;
        }
    }
    string jwt;
    bool loginSuccessful;
    //Method that signs in Cognito user 
    private async Task Login_User()
    {
        string userName = PlayGamesPlatform.Instance.GetUserDisplayName();
        string passWord = token.Split('0')[0] + "!";

        CognitoUserPool userPool = new CognitoUserPool(CredentialsManager.userPoolId, CredentialsManager.appClientId, provider);
        CognitoUser user = new CognitoUser(userName, CredentialsManager.appClientId, userPool, provider);

        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = passWord
        };

        try
        {
            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

            GetUserRequest getUserRequest = new GetUserRequest();
            getUserRequest.AccessToken = authResponse.AuthenticationResult.AccessToken;

            Debug.Log("User Access Token: " + getUserRequest.AccessToken);
            jwt = getUserRequest.AccessToken;

            // User is logged in
            loginSuccessful = true;


        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);
            return;
        }

        if (loginSuccessful == true)
        {

            string subId = await Get_User_Id();
            CredentialsManager.userid = subId;

            // Send Login Event
            Events.Call_Login();

            // Print UserID
            Debug.Log("Response - User's Sub ID from Cognito: " + CredentialsManager.userid);

        }
    }
    private async Task<string> Get_User_Id()
    {
        Debug.Log("Getting user's id...");

        string subId = "";

        Task<GetUserResponse> responseTask =
            provider.GetUserAsync(new GetUserRequest
            {
                AccessToken = jwt
            });

        GetUserResponse responseObject = await responseTask;

        // Set User ID
        foreach (var attribute in responseObject.UserAttributes)
        {
            if (attribute.Name == "sub")
            {
                subId = attribute.Value;
                break;
            }
        }

        return subId;
    }
}
