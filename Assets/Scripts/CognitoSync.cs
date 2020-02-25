using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.Runtime;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using System;
using System.Text;

public class CognitoSync : MonoBehaviour
{
    // us-east-2:f6463b82-d4da-4230-ae9d-675d286fc8f6
    // Regions.US_EAST_2

    [HideInInspector]
    public CognitoAWSCredentials credentials;

    CognitoSyncManager manager;
    Dataset playerInfo;

    [SerializeField]
    InputField nameIF, healthIF, experienceIF;

    string playerName;
    int playerHealth, playerExperience;

    bool sync = false;

    // Start is called before the first frame update
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        credentials = new CognitoAWSCredentials(
            "us-east-2:cd8a1e15-89bb-44b1-95b4-9225913c86b6",
            RegionEndpoint.USEast2
        );
        manager = new CognitoSyncManager(credentials, RegionEndpoint.USEast2);
        playerInfo = manager.OpenOrCreateDataset("playerInfo");
        playerInfo.OnSyncSuccess += PlayerInfo_OnSyncSuccess;
        UpdateInformation();
        


    }

    private void PlayerInfo_OnSyncSuccess(object sender, SyncSuccessEventArgs e)
    {
        List<Record> newRecords = e.UpdatedRecords;
        for (int k = 0; k < newRecords.Count; k++)
        {
            Debug.Log(newRecords[k].Key + " was updated: " + newRecords[k].Value);
        }
        UpdateInformation();
        sync = false;
    }

    private void UpdateInformation()
    {
        // Name
        if (!string.IsNullOrEmpty(playerInfo.Get("name")))
        {
            playerName = playerInfo.Get("name");
            nameIF.text = playerName;
        }
        else
            nameIF.text = "";
        // Health
        if (!string.IsNullOrEmpty(playerInfo.Get("health")))
        {
            playerHealth = int.Parse(playerInfo.Get("health"));
            healthIF.text = playerHealth.ToString();
        }
        else
            healthIF.text = "";
        // Experience
        if (!string.IsNullOrEmpty(playerInfo.Get("experience")))
        {
            playerExperience = int.Parse(playerInfo.Get("experience"));
            experienceIF.text = playerExperience.ToString();
        }
        else
            experienceIF.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Synchronize()
    {
        sync = true;
        playerInfo.SynchronizeOnConnectivity();
    }

    public void ChangeName(string newName)
    {
        try
        {
            playerName = MD5Hash(newName);
            playerInfo.Put("name", playerName);
        }
        catch { }
    }

    public void ChangeHealth(string newHealth)
    {
        try
        {
            playerHealth = int.Parse(newHealth);
            playerInfo.Put("health", newHealth);
        }
        catch { }
    }

    public void ChangeExperience(string newExperience)
    {
        try
        {
            playerExperience = int.Parse(newExperience);
            playerInfo.Put("experience", newExperience);
        }
        catch { }
    }

    public static string MD5Hash(string text)
    {
        MD5 md5 = new MD5CryptoServiceProvider();

        //compute hash from the bytes of text  
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

        //get hash result after compute it  
        byte[] result = md5.Hash;

        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            //change it into 2 hexadecimal digits  
            //for each byte  
            strBuilder.Append(result[i].ToString("x2"));
        }

        return strBuilder.ToString();
    }






}
