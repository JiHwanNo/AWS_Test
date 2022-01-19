using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Button LoginBtn;
    public Button TokenBtn;
    

    private AuthenticationManager _authenticationManager;
    ApiManager _apiManager;
    private void Awake()
    {
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
        _apiManager = FindObjectOfType<ApiManager>();
    }
    void Start()
    {
        LoginBtn.onClick.AddListener(OpenLogin);
        TokenBtn.onClick.AddListener(Token_linkAPI);
        RefreshToken();
    }

    void Token_linkAPI()
    {
        _apiManager.CallTestApi();
    }

    void OpenLogin()
    {
        string loginUrl = _authenticationManager.GetLoginUrl();
        Application.OpenURL(loginUrl);
    }
    private async void RefreshToken()
    {
        bool successfulRefresh = await _authenticationManager.CallRefreshTokenEndpoint(); // ������ ��ū�� ����Ǿ� �ִٸ� �װ��� ��Ȱ���Ѵ�.
    }

  
}
