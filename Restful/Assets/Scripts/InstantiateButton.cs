using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine.UI;

public class InstantiateButton : MonoBehaviour {

    public Transform Button; // this is the button prefab

    public GameObject LayoutGroupParent;

    void Start()
    {
        var buttonHeight = 60;
        
        var json = File.ReadAllText(@"C:\Users\adam.chernick\desktop\googdrive.json"); //need this to port over the json to deserialize
        var result = JsonConvert.DeserializeObject<Response>(json);

        var parent = result.Parents; // counting the list of parent objects..
        int parentCount = parent.Count;

        Debug.Log("Parent Count is" + parentCount); // now take parent count and instantiate that many button..

        var button1 = Instantiate(Button, transform.position, transform.rotation); 
        var button2 = Instantiate(Button, transform.position, transform.rotation);
        var button3 = Instantiate(Button, transform.position, transform.rotation);

        button1.transform.SetParent(LayoutGroupParent.gameObject.transform); // set new parent for instantianted object..
        button2.transform.SetParent(LayoutGroupParent.gameObject.transform); 
        button3.transform.SetParent(LayoutGroupParent.gameObject.transform);

        GameObject buttonText = GameObject.Find("buttonText"); // figure out how to dive into the second instantiate button.
        var buttonTextText = buttonText.GetComponent<Text>();
        buttonTextText.text = result.Id.ToString();

        //use button height multiplied by the parentCount (number of buttons) plus some numer (20 pixels?) to get total height that 
        //the Layout group should be. This will size the layout group correctly based on number of buttons.

    }
}
