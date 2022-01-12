using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Button LoginBtn;
    private AuthenticationManager _authenticationManager;

    private void Awake()
    {
        _authenticationManager = FindObjectOfType<AuthenticationManager>();

    }
    void Start()
    {
        LoginBtn.onClick.AddListener(OpenLogin);
        RefreshToken();
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
