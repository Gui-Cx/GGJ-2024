using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestFactory : ItemFactory
{

    public bool doStuff;
    public bool doMusic;
    private bool wasdoinMusic;
    [SerializeField] private ItemTest productPrefab;

    public override IProduct GetProduct(Vector3 position)
    {
        GameObject instance = Instantiate(productPrefab.gameObject, position, Quaternion.identity);
        ItemTest newProduct = instance.GetComponent<ItemTest>();

        // each product contains its own logic
        newProduct.Initialize();

        return newProduct;
    }

    private void Update()
    {
        if(doStuff) { GetProduct(this.transform.position + new Vector3(2, 0, 0)); }
        if(doMusic != wasdoinMusic)
        {
            wasdoinMusic = doMusic;
            AudioManager.Instance.SetMusicVersion(doMusic ? 0.7f : 0.3f);
        }
    }

}
