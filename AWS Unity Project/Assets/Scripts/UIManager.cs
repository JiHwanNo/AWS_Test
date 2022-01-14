using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Button LoginBtn;
    public Button TokenBtn;
    Text tokentext;

    private AuthenticationManager _authenticationManager;
    private void Awake()
    {
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
        tokentext = TokenBtn.transform.GetChild(0).GetComponent<Text>();
    }
    void Start()
    {
        LoginBtn.onClick.AddListener(OpenLogin);
        TokenBtn.onClick.AddListener(CheckToken);
        RefreshToken();
    }

    void CheckToken()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);
    }

    void OpenLogin()
    {
        string loginUrl = _authenticationManager.GetLoginUrl();
        Application.OpenURL(loginUrl);
    }
    private async void RefreshToken()
    {
        bool successfulRefresh = await _authenticationManager.CallRefreshTokenEndpoint(); // 기존의 토큰이 저장되어 있다면 그것을 재활용한다.
    }

  
}
