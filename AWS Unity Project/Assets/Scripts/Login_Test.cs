using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Login_Test : MonoBehaviour
{
    
    public static string CachePath;

    // In production, should probably keep these in a config file
    private const string AppClientID = "4bcnhb47vji4ps4q91ou3vhb5b"; // App client ID, found under App Client Settings
    private const string AuthCognitoDomainPrefix = "jihwan-world-test"; // Found under App Integration -> Domain Name. Changing this means it must be updated in all linked Social providers redirect and javascript origins
    private const string RedirectUrl = "https://www.google.com";
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

    void Start()
    {
        string loginUrl = GetLoginUrl();
        Debug.LogError(loginUrl);
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


}
