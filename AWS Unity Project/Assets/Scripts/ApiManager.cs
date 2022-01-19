using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private const string Api = " https://z85run4wta.execute-api.ap-northeast-2.amazonaws.com/AWS_2022-01-17/2022-01-17"; //접근 주소.
    
    public async void CallTestApi()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(Api);

        // https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-invoke-api-integrated-with-cognito-user-pool.html
        webRequest.SetRequestHeader("Authorization", Cognito.id_token);

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
