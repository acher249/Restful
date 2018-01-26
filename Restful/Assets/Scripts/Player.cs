using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //public Enemy[] enemies;

    #region Singleton
    private static Player _instance; //underscore because it is private..
    //public static Player Instance { get; set; }
    #endregion

    public delegate void ChangeEnemyColor(Color color); //pass in a reference top a color
    public static event ChangeEnemyColor onEnemyHit;

    // Use this for initialization
    void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        
    }

    void Update() // everytime the enemy is hit we want to call the damage function
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //damage enemy
            if (onEnemyHit != null) // if its not null that means that there is somthing listening for this event..
            {
                onEnemyHit(Color.red);
            }
        }
    }

    //// Update is called once per frame *************Not Using Delegates and Events*************
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        ChangeColor();
    //    }

    //}

    //private void ChangeColor()
    //{
    //    foreach (var enemy in enemies)
    //    {
    //        enemy.GetComponent<Renderer>().material.color = Color.red;
    //    }
    //}
}