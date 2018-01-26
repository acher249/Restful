using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Player.onEnemyHit += Damage; // on start we subscribe the OnEnemyHit event to the Damage method
                                     // The Damage method can only be subscribed because it matches the delegate signature.

        Debug.Log("onEnemyHit Event is subscribed to the Damage method");
	}

	void Damage (Color color) // find the event and subscribe the damage function to it..
                              //This method matches the change enemy color delegate signature..
    {
        transform.GetComponent<Renderer>().material.color = color;
	}
}
