using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class DeserializeScript : MonoBehaviour
{

    public class Response
    {
        [JsonConstructor]
        public Response()
        {
        }

        [JsonProperty("projects")]
        public List<SomeProject> Projects { get; set; }
    }

    public class SomeProject
    {
        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("thumbnailImage")]
        public string ThumbnailImage { get; set; }

        [JsonConstructor]
        public SomeProject()
        {
        }

        public SomeProject(string name, string image, string desc)
        {
            ProjectName = name;
            ThumbnailImage = image;
        }
    }


    // Starting Functions

    public Transform buttonPrefab; // this is the button prefab
    public GameObject LayoutGroupParent;

    void Start()
    {

        string json = File.ReadAllText(@"C:\Users\adam.chernick\Documents\GitHub\Restful\Restful\Assets\Scripts\Json\googdrive.json"); //need this to port over the json to deserialize
        Response result = JsonConvert.DeserializeObject<Response>(json);

        List<SomeProject> projects = result.Projects;

        RectTransform buttonRectTranform = buttonPrefab.GetComponent<RectTransform>();
        var buttonHeight = buttonRectTranform.rect.height;

        int buttonCount = projects.Count;

        RectTransform rt = LayoutGroupParent.GetComponent<RectTransform>();
        var vlg = LayoutGroupParent.GetComponent<VerticalLayoutGroup>();
        var vlgSpacing = vlg.spacing;

        // (Adam) This sets the anchors for the LayoutGroup. so that when it grows it grows down, to allocate 
        //for addition or subtraction of buttons.
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);


        var layoutGroupHeight = (buttonHeight * buttonCount) + (vlgSpacing * buttonCount) - 20;
        Debug.Log("Layout Group Height Needs To Be " + layoutGroupHeight + " Pixels high");


        rt.sizeDelta = new Vector2(300, layoutGroupHeight); //change height of panel based on how many buttons are in the list

        foreach (SomeProject proj in projects)
        {
            Sprite sprite = null;
            if (!string.IsNullOrEmpty(proj.ThumbnailImage))
            {
                byte[] bytes = Convert.FromBase64String(proj.ThumbnailImage);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);

                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            }

            Transform button = Instantiate(buttonPrefab, transform.position, transform.rotation);
            button.transform.SetParent(LayoutGroupParent.gameObject.transform);

            // (Konrad) GetComponent not GetComponents, notice "s" or lack of it at the end
            // This call will return Text object inside of that prefab, since there is only one
            // We will get the right one. In case there were more than one we would get first encountered.
            Text buttonText = button.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "buttonText");
            if (buttonText != null) buttonText.text = proj.ProjectName;

            if (sprite == null) continue;

            // (Konrad) GetComponents, notice "s" at the end. This gets all of the children components of the button that
            // are of type Image. This will be an array Image[]. 
            // System.Linq is a utility library that makes working with Lists, arrays and sets etc., much easier. 
            // FirstOrDefault() will take first item that matches the rule defined inside (). If no object in the array
            // matches the rule, the default returned is null. 
            // x is the object in the array. basically It is finding the first object in thhe array with the name buttonImage
            Image buttonImage = button.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "buttonImage");
            if (buttonImage == null) continue;

            buttonImage.sprite = sprite;
        }
    }
}
