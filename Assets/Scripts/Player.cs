using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    // Left = -1; None = 0; Right = 1
    private int movementDirection = 0;

    private Rigidbody2D rigidbody2d;
    private Interactor interactor;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        interactor = GetComponent<Interactor>();
    }    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody2d.AddForce(Vector2.right*movementDirection);
    }
    
    void OnGrab(InputValue context)
    {
        Debug.LogFormat("Cx : A pressed, Grab");
    }

    void OnInteract(InputValue context)
    {
        Debug.LogFormat("Cx : E pressed, Interact");
        if (interactor.currentInteractable != null) interactor.currentInteractable.Interact(interactor);
    }

    void OnMove(InputValue context)
    {
        Debug.LogFormat("Cx : Direction is {0}", context.Get<float>());
        movementDirection = (int)Math.Round(context.Get<float>());
    }
}
