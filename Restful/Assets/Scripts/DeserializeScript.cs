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

        [JsonProperty("buckets")]
        public List<SomeBucket> Buckets { get; set; }

        [JsonConstructor]
        public SomeProject()
        {
        }
    }

    public class SomeBucket
    {
        [JsonProperty("bucketName")]
        public string BucketName { get; set; }

        [JsonProperty("bucketDescription")]
        public string BucketDescription { get; set; }

        [JsonProperty("bucketImages")]
        public List<SomeBucketImage> BucketImages { get; set; }

        [JsonConstructor]
        public SomeBucket() // everytime it deserializes it uses this..
        {
        }
    }

    public class SomeBucketImage
    {
        [JsonProperty("bucketImageName")]
        public string BucketImageName { get; set; }

        [JsonProperty("bucketImage")]
        public string BucketImage { get; set; }

        [JsonConstructor]
        public SomeBucketImage() // everytime it deserializes it uses this..
        {
        }
    }


    // Starting Functions

    public GameObject ButtonPrefab;
    public GameObject Canvas;
    public GameObject ProjectPanelsParent;
    public GameObject BucketPanelsParent;
    public GameObject ProjectPanelPrefab;
    public GameObject BucketPanelPrefab;
    public GameObject AllProjectsPanel; // Will always be there.


    void Start()
    {

        string json = File.ReadAllText(@"C:\Users\adam.chernick\Documents\GitHub\Restful\Restful\Assets\Scripts\Json\googdrive.json"); //need this to port over the json to deserialize
        Response result = JsonConvert.DeserializeObject<Response>(json);

        List<SomeProject> projects = result.Projects;

        RectTransform buttonRectTranform = ButtonPrefab.GetComponent<RectTransform>();
        var buttonHeight = buttonRectTranform.rect.height;

        int buttonCount = projects.Count;

        RectTransform rt = AllProjectsPanel.GetComponent<RectTransform>();
        var vlg = AllProjectsPanel.GetComponent<VerticalLayoutGroup>();
        var vlgSpacing = vlg.spacing;

        // (Adam) This sets the anchors for the LayoutGroup. so that when it grows it grows down, to allocate 
        //for addition or subtraction of buttons.
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);


        var layoutGroupHeight = (buttonHeight * buttonCount) + (vlgSpacing * buttonCount) - 20;
        Debug.Log("Layout Group Height Needs To Be " + layoutGroupHeight + " Pixels high");


        rt.sizeDelta = new Vector2(300, layoutGroupHeight); //change height of panel based on how many buttons are in the list


        //float buckLeft = 0;
        //float buckRight = 0;
        //float buckTop = 0;
        //float buckBottom = 0;

        //RectTransform buckRT = ProjectPanelPrefab.GetComponent<RectTransform>();
        //var buckLeft = buckRT.rect.xMin;
        //var buckRight = buckRT.rect.xMax;
        //var buckTop = buckRT.rect.yMin;
        //var buckBottom = buckRT.rect.yMax;

        //Debug.Log("Left" + buckLeft);
        //Debug.Log("Right" + buckRight);
        //Debug.Log("Top" + buckTop);
        //Debug.Log("Bottom" + buckBottom);

        // (Adam) Need new loop for buck in buckets .. will instantiante and parent to different 
        // GameObject. Need to add click event with script to turn off GameObject and on 
        // others. If click project1 button (first in array) turn off AllProjectsPanel and
        // turn on ProjectOnePanel gameObject. Then need loop for buckImg in bucketImages.

        //******Projects******//
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

            GameObject projectButton = Instantiate(ButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            projectButton.transform.SetParent(AllProjectsPanel.gameObject.transform);

            // (Konrad) GetComponent not GetComponents, notice "s" or lack of it at the end
            // This call will return Text object inside of that prefab, since there is only one
            // We will get the right one. In case there were more than one we would get first encountered.
            Text projectButtonText = projectButton.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "buttonText");
            if (projectButtonText != null) projectButtonText.text = proj.ProjectName;

            if (sprite == null)
            {
                continue;
            }
            

            // (Konrad) GetComponents, notice "s" at the end. This gets all of the children components of the button that
            // are of type Image. This will be an array Image[]. 
            // System.Linq is a utility library that makes working with Lists, arrays and sets etc., much easier. 
            // FirstOrDefault() will take first item that matches the rule defined inside (). If no object in the array
            // matches the rule, the default returned is null. 
            // x is the object in the array. basically It is finding the first object in thhe array with the name buttonImage
            Image projectButtonImage = projectButton.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "buttonImage");
            if (projectButtonImage == null)
            {
                continue; 
            }

            projectButtonImage.sprite = sprite;

            //(Adam) Must instantiate the ProjectPanelPrefab itself before you can instantiate buttons onto it..
            GameObject projPanelPrefab = Instantiate(ProjectPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            projPanelPrefab.transform.SetParent(ProjectPanelsParent.gameObject.transform);

            // (Konrad) Reset the transform offsets to 0.
            projPanelPrefab.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // left, bottom
            projPanelPrefab.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // right, top

            Transform bpp = null;
            foreach (Transform t in projPanelPrefab.transform)
            {
                if (t.name == "BucketPanelsParent") bpp = t;
            }
            if (bpp == null) continue;


            var buckLayoutGroup = projPanelPrefab.gameObject.transform.GetChild(0);


            //**** Buckets*****//
            foreach (SomeBucket buck in proj.Buckets)
            {

                GameObject buckPanelPrefab = Instantiate(BucketPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                buckPanelPrefab.transform.SetParent(bpp);

                //(Adam)Then instantiate button onto it..
                GameObject bucketButton = Instantiate(ButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                //bucketButton.transform.SetParent(projPanelLayoutGroup.gameObject.transform);
                bucketButton.transform.SetParent(buckLayoutGroup.gameObject.transform);

                Text bucketButtonText = bucketButton.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "buttonText");
                if (bucketButtonText != null) bucketButtonText.text = buck.BucketName;

                if (sprite == null)
                {
                    continue;
                }

                //foreach (SomeBucketImage buckImg in buck.BucketImages) // buck is the object that we are currently on
                //{

                //}
            }
        }
    }
}
