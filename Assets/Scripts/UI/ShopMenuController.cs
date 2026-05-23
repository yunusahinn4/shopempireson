using UnityEngine;

public class ShopMenuController : MonoBehaviour
{
    public RectTransform shopBar;
    public RectTransform toggleButton;
    public GameObject foodPanel;

    public float openX = 0f;
    public float closedX = -180f;
    public float speed = 10f;

    private bool isOpen = true;
    private Vector2 shopBarTarget;
    private Vector2 buttonTarget;

    void Start()
    {
        shopBarTarget = shopBar.anchoredPosition;
        buttonTarget = toggleButton.anchoredPosition;
    }

    void Update()
    {
        shopBar.anchoredPosition = Vector2.Lerp(
            shopBar.anchoredPosition,
            shopBarTarget,
            Time.deltaTime * speed
        );

        toggleButton.anchoredPosition = Vector2.Lerp(
            toggleButton.anchoredPosition,
            buttonTarget,
            Time.deltaTime * speed
        );
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;

        float targetX = isOpen ? openX : closedX;

        shopBarTarget = new Vector2(targetX, shopBar.anchoredPosition.y);

        buttonTarget = new Vector2(
            targetX + 180f,
            toggleButton.anchoredPosition.y
        );

        if (!isOpen && foodPanel != null)
        {
            foodPanel.SetActive(false);
        }
    }
}