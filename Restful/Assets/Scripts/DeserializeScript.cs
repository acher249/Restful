using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine.UI;


public class Response
{
    [JsonConstructor]
    public Response()
    {
    }

    [JsonProperty("kind")] // tells json deserializer that this is the name in json file
    public string Kind { get; set; } 

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("etag")]
    public string Etag { get; set; }

    /// <summary>
    /// Add here....
    /// </summary>

    [JsonProperty("labels")]
    public Dictionary<string, bool> Labels { get; set; }

    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; } //get set allows us to get information to deserialize and also push information back.. set may not be necesarry in our case.

    [JsonProperty("modifiedDate")]
    public DateTime ModifiedDate { get; set; }

    [JsonProperty("modifiedByMeDate")]
    public DateTime ModifiedByMeDate { get; set; }

    [JsonProperty("lastViewedByMeDate")]
    public DateTime LastViewedByMeDate { get; set; }

    [JsonProperty("markedViewedByMeDate")]
    public DateTime MarkedViewedByMeDate { get; set; }

    [JsonProperty("version")]
    public int Version { get; set; }

    [JsonProperty("parents")]
    public List<Parent> Parents { get; set; } // Ties to parent below..

    [JsonProperty("buttons")]
    public List<SomeButton> Buttons { get; set; }
}


public class Parent //created this to tie back to Parent List above
{
    [JsonConstructor] //still dont understand why this is useful
    public Parent()
    {
    }

    [JsonProperty("kind")] // tells json deserializer that this is the name in json file
    public string Kind { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("selfLink")]
    public string SelfLink { get; set; }

    [JsonProperty("parentLink")]
    public string ParentLink { get; set; }

    [JsonProperty("isRoot")]
    public bool IsRoot { get; set; }
}

public class SomeButton
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonConstructor]
    public SomeButton()
    {
    }

    public SomeButton(string name, string image, string desc)
    {
        Name = name;
        Image = image;
        Description = desc;
    }
}





public class DeserializeScript : MonoBehaviour
{

    public Text textBoxDateTime;
    public GameObject image1;
    public GameObject image2;

    // Use this for initialization
    void Start () {

        var json = File.ReadAllText(@"C:\Users\adam.chernick\Documents\GitHub\Restful\Restful\Assets\Scripts\Json\googdrive.json");
        if (string.IsNullOrEmpty(json)) return; // if json is empty or null, it failed reading it, it could fail because file was open/locked by another process

        var result = JsonConvert.DeserializeObject<Response>(json); //Dersializes the Json.. then stores it in variable result

        if (textBoxDateTime != null && result != null)
        {
            textBoxDateTime.text = result.ModifiedDate.ToString(); // convert datetime object to string, text object expects a string type
        }
    }
}
