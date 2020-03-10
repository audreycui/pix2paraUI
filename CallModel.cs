using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;
using System.ComponentModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using System.Text;
using System.Threading.Tasks;

public class CallModel : MonoBehaviour
{

    public Button ScreenshotButton;
    public GameObject LabelText; 
    public string label; 
  
    void Start()
    {
        ScreenshotButton.onClick.AddListener(takeScreenshot);
        LabelText.GetComponent<Text>().text = ""; 
    }

    public void takeScreenshot()
    {
        //Camera myCamera = GameObject.GetComponent<ARCamera>(); 
        Texture2D tex = new Texture2D(Screen.width, Screen.height); //captures camera content as a texture
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0); //gets pixel content
        
        byte[] byteArray = tex.EncodeToPNG(); //encodes pixel content into bytes format
        Debug.Log("len: " + byteArray.GetLength(0));
        File.WriteAllBytes(Application.dataPath + "/screenshot.png", byteArray); //debugging
       

        InvokeRequestResponseService(byteArray, LabelText);
   
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static async Task InvokeRequestResponseService(byte[] img, GameObject LabelText) //calls Azure Webservice
    {


        string output = "hi";
        using (var client = new HttpClient())
        {

            

            client.BaseAddress = new Uri("http://17ebffef-bd8b-43dc-bb83-4649983647cc.westus.azurecontainer.io/score"); //webservice address



            var request = new Dictionary<string, byte[]>() { { "data", img } }; 
            var scoreJson = JsonConvert.SerializeObject(request);
            File.WriteAllText(Application.dataPath + "/scoreJson.txt", scoreJson);
            //Debug.Log("here!");
          
            Debug.Log("scoreRequest: " + scoreJson);
            var content = new StringContent(scoreJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("", content); //calls webservice


            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Debug.Log("Result: " + result);
                output = result;
            }
            else
            {
                Debug.Log(string.Format("The request failed with status code: {0}", response.StatusCode));

                Debug.Log(response.Headers.ToString());

                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.Log(responseContent);
                output = "failed :(";
            }

            Debug.Log("output: " + output);
            LabelText.GetComponent<Text>().text = output; //displays label on ui 
        }

    }

}

class VGGStringTable
{
    [JsonProperty("data")]
    internal byte[] ImageArray { get; set; }
    // public string[,] Values { get; set; }
}

