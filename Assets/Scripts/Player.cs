using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainClown : MonoBehaviour
{
    public double horizontalSpeed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMove(InputValue context)
    {
        Debug.LogFormat("Cx : Direction is {0}", context.Get<float>());
        horizontalSpeed = context.Get<float>();   
    }
}
