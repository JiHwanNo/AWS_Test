using System;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
public class CredentialsManager
{


    // Region - A game may need multiple region endpoints if services
    // are in multiple regions or different per service
    
    // Cognito Credentials Variables
    public const string identityPool = "ap-northeast-2:263ea1bd-6812-4412-a5f7-062e9a7f1cc7";
    public static string userPoolId = "ap-northeast-2_VbhCkEbua";
    public static string appClientId = "4bcnhb47vji4ps4q91ou3vhb5b";

    private static RegionEndpoint region = null;

    public  static RegionEndpoint _region
    {
        get
        {
            if (region == null)
                region = RegionEndpoint.APNortheast2;
            return region;
        }
    }

    // Initialize the Amazon Cognito credentials provider
    private static CognitoAWSCredentials credentials;
    
    public static CognitoAWSCredentials _credentials
    {
        get 
        {
            if (credentials == null)
                credentials = new CognitoAWSCredentials(identityPool, region);

            return credentials;
        }
    }
    // User's Cognito ID once logged in becomes set here
    public static string userid = "";

}
