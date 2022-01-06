
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using UnityEngine;
// Text UI ���
using UnityEngine.UI;
// ���� �÷��� ����
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Amazon;
using Amazon.CognitoIdentity;
public class GoogleLogin : MonoBehaviour
{
    public Text text;

    void Awake()
    {
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        text.text = "no Login";
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
                    string token = PlayGamesPlatform.Instance.GetIdToken();

                    CredentialsManager.credentials.AddLogin("accounts.google.com", token);
                    
                    text.text = CredentialsManager.credentials.GetIdentityId();
                }
                else
                {
                    Debug.Log("Fall");
                    text.text = "Fail";
                }
            });
        }
    }

    public void OnLogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        text.text = "Logout";
    }



 
}
