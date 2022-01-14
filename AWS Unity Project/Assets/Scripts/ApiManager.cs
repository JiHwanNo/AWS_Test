using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private const string Api = "https://qqz3a5ix7b.execute-api.ap-northeast-2.amazonaws.com/AWS_2022-01-13";
    
    public async void CallTestApi()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(Api);

        // https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-invoke-api-integrated-with-cognito-user-pool.html
        webRequest.SetRequestHeader("Authorization", AuthenticationManager.jwt);

        await webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("API call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
        }
        else
        {
            Debug.Log("Success, API call complete!");
            Debug.Log(webRequest.downloadHandler.text);
        }
        webRequest.Dispose();
    }

}
