using System;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
public class CredentialsManager
{


    // Region - A game may need multiple region endpoints if services
    // are in multiple regions or different per service
    
    // Cognito Credentials Variables
    public const string _identityPool = "ap-northeast-2:263ea1bd-6812-4412-a5f7-062e9a7f1cc7";
    public static string _userPoolId = "ap-northeast-2_VbhCkEbua";
    public static string _appClientId = "4bcnhb47vji4ps4q91ou3vhb5b";

    public static RegionEndpoint _region = RegionEndpoint.APNortheast2;

    // Initialize the Amazon Cognito credentials provider
    public static CognitoAWSCredentials _credentials = new CognitoAWSCredentials(_identityPool, _region);
    // User's Cognito ID once logged in becomes set here
    public static string _userid = "";
    public static string _id_token = "";
    public static string _access_Token = "";

    public static string jwt = "";
}
