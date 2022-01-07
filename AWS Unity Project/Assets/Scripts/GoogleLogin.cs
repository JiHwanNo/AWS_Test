// Text UI 사용
// 구글 플레이 연동
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using UnityEngine;

public class GoogleLogin : MonoBehaviour
{

    string token = string.Empty;

    void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
           .Builder()
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
                    CredentialsManager.credentials.AddLogin("accounts.google.com", token); // providername, token

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


}
