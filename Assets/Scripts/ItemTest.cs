using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour, IInteractable, IProduct
{
    public string ProductName { get => "Test Omg";}

    private ParticleSystem particleSystem;

    public void Initialize()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem?.Stop();
        particleSystem?.Play();
    }
    public bool Interact(Interactor interactor)
    {
        Debug.LogWarning("OMG j'interagis");
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(0.01f, 0, 0);
    }
}
