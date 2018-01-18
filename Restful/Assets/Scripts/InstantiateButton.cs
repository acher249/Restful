using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class InstantiateButton : MonoBehaviour {

    public Transform Button; // this is the button prefab
    public GameObject LayoutGroupParent;

    void Start()
    {
        int buttonHeight = 90;
        
        string json = File.ReadAllText(@"C:\Users\adam.chernick\Documents\GitHub\Restful\Restful\Assets\Scripts\Json\googdrive.json"); //need this to port over the json to deserialize
        Response result = JsonConvert.DeserializeObject<Response>(json);

        List<SomeButton> buttons = result.Buttons;
        foreach (SomeButton btn in buttons)
        {
            Sprite sprite = null;
            if (!string.IsNullOrEmpty(btn.Image))
            {
                byte[] bytes = Convert.FromBase64String(btn.Image);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);

                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            }

            Transform button = Instantiate(Button, transform.position, transform.rotation);
            button.transform.SetParent(LayoutGroupParent.gameObject.transform);

            // (Konrad) GetComponent not GetComponents, notice "s" or lack of it at the end
            // This call will return Text object inside of that prefab, since there is only one
            // We will get the right one. In case there were more than one we would get first encountered.
            Text buttonText = button.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "buttonText");
            if(buttonText != null) buttonText.text = btn.Name;

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

        //use button height multiplied by the parentCount (number of buttons) plus some numer (20 pixels?) to get total height that 
        //the Layout group should be. This will size the layout group correctly based on number of buttons.

    }
}
