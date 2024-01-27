using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }


    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Error Singleton FMODEvents");
        instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
