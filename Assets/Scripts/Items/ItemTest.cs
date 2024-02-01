using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour, IInteractable, IProduct
{
    public string ProductName { get => "Test Omg";}

    private ParticleSystem _particleSystem;

    public void Initialize()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem?.Stop();
        _particleSystem?.Play();
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
