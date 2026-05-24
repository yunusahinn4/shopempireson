using UnityEngine;

public class ShopEntrance : MonoBehaviour
{
    public ShopData shopData;
    public Transform doorPoint;

    void Awake()
    {
        if (shopData == null)
            shopData = GetComponent<ShopData>();
    }
}