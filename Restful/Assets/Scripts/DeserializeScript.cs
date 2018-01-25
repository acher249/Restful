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
    public GameObject AllProjectsPanelLayoutGroup; // Will always be there.


    delegate void MyDelegate();
    MyDelegate myDelegate;

    void Start()
    {

        string json = File.ReadAllText(@"C:\Users\adam.chernick\Documents\GitHub\Restful\Restful\Assets\Scripts\Json\googdrive.json"); //need this to port over the json to deserialize
        Response result = JsonConvert.DeserializeObject<Response>(json);

        List<SomeProject> projects = result.Projects;
        //List<SomeProject> buckets = result.Projects.Buckets;

        RectTransform buttonRectTranform = ButtonPrefab.GetComponent<RectTransform>();
        var buttonHeight = buttonRectTranform.rect.height;

        int projButtonCount = projects.Count;
        

        RectTransform AllProjPanelsRT = AllProjectsPanelLayoutGroup.GetComponent<RectTransform>();
        var vlg = AllProjectsPanelLayoutGroup.GetComponent<VerticalLayoutGroup>();
        var vlgSpacing = vlg.spacing;


        var layoutGroupHeight = (buttonHeight * projButtonCount) + (vlgSpacing * projButtonCount) - 20;
        //Debug.Log("Layout Group Height Needs To Be " + layoutGroupHeight + " Pixels high");


        AllProjPanelsRT.sizeDelta = new Vector2(300, layoutGroupHeight); //change height of panel based on buttons

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
            projectButton.transform.SetParent(AllProjectsPanelLayoutGroup.gameObject.transform);

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

            Transform buckPanelParent = null;
            foreach (Transform t in projPanelPrefab.transform)
            {
                if (t.name == "BucketPanelsParent") buckPanelParent = t;
            }
            if (buckPanelParent == null) continue;

            var projPanelLayoutGroup = projPanelPrefab.gameObject.transform.GetChild(0);

            //(Adam) Turn the panels off. But still exist to be turned on later.
            projPanelPrefab.SetActive(false);


            //********Anchoring and sizing the LayoutGroup*********//
            RectTransform ProjPanelLayoutGroupRT = projPanelLayoutGroup.GetComponent<RectTransform>();
            var projPanelVLG = ProjPanelLayoutGroupRT.GetComponent<VerticalLayoutGroup>();
            var projPanelVLGSpacing = projPanelVLG.spacing;

            int buckButtonCount = proj.Buckets.Count;

            Debug.Log("Bucket Button Count = " + buckButtonCount);

            var projPanelLayoutGroupHeight = (buttonHeight * buckButtonCount) + (projPanelVLGSpacing * buckButtonCount) - 20;

            Debug.Log(projPanelLayoutGroupHeight);

            // (Adam)Resize Project Panel Layout Group
            ProjPanelLayoutGroupRT.sizeDelta = new Vector2(300, projPanelLayoutGroupHeight);

            // (Konrad) Reset the transform offsets to 0. This gives write ability..
            projPanelPrefab.GetComponent<RectTransform>().offsetMin = new Vector2(0, 20); // left, bottom
            projPanelPrefab.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // right, top

            foreach (SomeBucket butt in proj.Buckets) // make click buttons do things.
            {
     
                //var btn = projectButton.GetComponent<Button>(); //turn on first project panel and turn off AllProjectsPanel.
                //Utility at bottom         ***********NEED EVENT SYSTEM*************
                //btn.onClick.AddListener(TaskOnClick);
            }
            

            //**** Buckets*****//
            foreach (SomeBucket buck in proj.Buckets)
            {

                GameObject buckPanelPrefab = Instantiate(BucketPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                buckPanelPrefab.transform.SetParent(buckPanelParent);

                buckPanelPrefab.SetActive(false);

                //(Adam)Then instantiate button onto it..
                GameObject bucketButton = Instantiate(ButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                bucketButton.transform.SetParent(projPanelLayoutGroup.gameObject.transform);

                Text bucketButtonText = bucketButton.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "buttonText");
                if (bucketButtonText != null) bucketButtonText.text = buck.BucketName;

                buckPanelPrefab.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // left, bottom
                buckPanelPrefab.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // right, top

                if (sprite == null)
                {
                    continue;
                }

                int buckImageButtonCount = buck.BucketImages.Count;

                foreach (SomeBucketImage buckImg in buck.BucketImages) // buck is the object that we are currently on
                {
                    var buckPanelLayoutGroup = buckPanelPrefab.gameObject.transform.GetChild(0);

                    RectTransform buckPanelLayoutGroupRT = buckPanelLayoutGroup.GetComponent<RectTransform>();
                    var buckPanelVLG = buckPanelLayoutGroupRT.GetComponent<VerticalLayoutGroup>();
                    var buckPanelVLGSpacing = buckPanelVLG.spacing;

                    var buckPanelLayoutGroupHeight = (buttonHeight * buckImageButtonCount) + (buckPanelVLGSpacing * buckImageButtonCount) - 20;

                    Debug.Log("Bucket Panel Height = " + buckPanelLayoutGroupHeight);

                    // (Adam)Resize Project Panel Layout Group
                    buckPanelLayoutGroupRT.sizeDelta = new Vector2(300, buckPanelLayoutGroupHeight);

                    GameObject bucketImageButton = Instantiate(ButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    bucketImageButton.transform.SetParent(buckPanelLayoutGroup.gameObject.transform);

                    Text bucketImageButtonText = bucketImageButton.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "buttonText");
                    if (bucketImageButtonText != null) bucketImageButtonText.text = buckImg.BucketImageName;
                }
            }
        }
    }
}
