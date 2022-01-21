using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class ApiManager : MonoBehaviour
{
    private const string Api = "https://rj30ctdrv8.execute-api.ap-northeast-2.amazonaws.com/deploy/lambdatest";
    private const string Api2 = "https://rj30ctdrv8.execute-api.ap-northeast-2.amazonaws.com/deploy/lambdatest";

    private const string Api3 = "https://rj30ctdrv8.execute-api.ap-northeast-2.amazonaws.com/deploy/s3";
    private AuthenticationManager _authenticationManager;

    public void CallTestGet()
    {
        CallTestApi();
    }

    public void CallTestPut()
    {
        CallTestApi2();
    }

    public async void CallTestApi()
    {
        string _id = "12345";
        //쿼리
        UnityWebRequest webRequest = UnityWebRequest.Get(Api + "?id=" + _id);

        webRequest.SetRequestHeader("authorizer-dong", CredentialsManager._id_token);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        await webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("API call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
        }
        else
        {
            Debug.Log("Success, API call complete!");
            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log("Data_" + webRequest.downloadHandler.data);


        }
        webRequest.Dispose();


        // https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-invoke-api-integrated-with-cognito-user-pool.html
        Debug.Log("API 요청! _ Id_Token ___" + CredentialsManager._id_token);


    }

    public async void CallTestApi2()
    {

        APITest _apiTest = new APITest
        {
            name = "dongwon",
            value = "yaaaaaaaaaaaaaap"
        };

        string form = JsonUtility.ToJson(_apiTest);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(Api2, form))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(form);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("API 요청! Test!!!!");

            await webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("API call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
            }
            else
            {
                Debug.Log("Success, API call complete!");
                Debug.Log(webRequest.downloadHandler.text);
                Debug.Log("Data_" + webRequest.downloadHandler.data);


            }
            webRequest.Dispose();
        }
    }

    void Awake()
    {
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
        CallTestApi2();
    }


}



public class APITest
{
    public string name;
    public string value;
}
