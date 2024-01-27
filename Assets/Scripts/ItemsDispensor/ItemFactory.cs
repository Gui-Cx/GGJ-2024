using UnityEngine;

public abstract class ItemFactory : MonoBehaviour
{
    public abstract IProduct GetProduct(Vector3 position);
}
