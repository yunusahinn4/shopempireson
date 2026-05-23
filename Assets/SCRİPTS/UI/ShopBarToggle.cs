using UnityEngine;

public class ShopBarToggle : MonoBehaviour
{
    public RectTransform shopBar;

    public float closedX = -180f;
    public float openX = 0f;
    public float speed = 10f;

    private bool isOpen = true;
    private Vector2 targetPosition;

    void Start()
    {
        targetPosition = shopBar.anchoredPosition;
    }

    void Update()
    {
        shopBar.anchoredPosition = Vector2.Lerp(
            shopBar.anchoredPosition,
            targetPosition,
            Time.deltaTime * speed
        );
    }

    public void ToggleShopBar()
    {
        isOpen = !isOpen;

        float targetX = isOpen ? openX : closedX;
        targetPosition = new Vector2(targetX, shopBar.anchoredPosition.y);
    }
}