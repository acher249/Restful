using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CallbackSystem : MonoBehaviour
{
    public delegate void OnMessageRecieved(); // this is the signature of the delegate
    public event OnMessageRecieved onComplete; //event must match teh signature of the the delegate
                                               // now we will subscribe onComplete to other functions..

	// Use this for initialization
	void Start ()
    {
        onComplete += WriteMessage; // += adds a method
        onComplete += Message; // += adds a method

        onComplete(); // now when you call onComplete it is going to call any methods that you have subscribed to it.
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void WriteMessage() // this signature matches the signature of the delegate...
    {
        Debug.Log("WriteMessage() is Called!");
    }

    void Message() // this signature matches the signature of the delegate...
    {
        Debug.Log("Message() is Called!");
    }
}
