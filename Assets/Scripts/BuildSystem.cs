using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BuildSystem : MonoBehaviour
{
    public GameObject selectedShopPrefab;
    public Collider2D buildArea;
    public LayerMask shopLayer;
    public LayerMask supportLayer;
    public TextMeshProUGUI moneyText;

    public int money = 1000;

    public float horizontalSnap = 0.5f;
    public float floorHeight = 3f;

    public int supportCheckPoints = 8;

    private GameObject previewShop;
    private SpriteRenderer previewRenderer;
    private ShopData currentShopData;
    private bool buildMode = false;

    void Start()
    {
        UpdateMoneyUI();
    }

    void Update()
    {
        if (!buildMode || previewShop == null) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            previewShop.SetActive(false);
            return;
        }

        previewShop.SetActive(true);

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float snappedX = Mathf.Round(worldPos.x / horizontalSnap) * horizontalSnap;
        float snappedY = Mathf.Round(worldPos.y / floorHeight) * floorHeight;

        Vector2 buildPosition = new Vector2(snappedX, snappedY);

        previewShop.transform.position = buildPosition;

        bool canBuild = CanBuild(buildPosition);

        if (money < currentShopData.price)
            canBuild = false;

        SetPreviewColor(canBuild ? Color.green : Color.red);

        if (Input.GetMouseButtonDown(0) && canBuild)
        {
            Instantiate(selectedShopPrefab, buildPosition, Quaternion.identity);

            money -= currentShopData.price;
            UpdateMoneyUI();

            buildMode = false;
            previewShop.SetActive(false);
        }
    }

    bool CanBuild(Vector2 position)
    {
        if (!buildArea.OverlapPoint(position))
            return false;

        Vector2 shopSize = previewRenderer.bounds.size;

        Collider2D hit = Physics2D.OverlapBox(
            position,
            shopSize * 0.95f,
            0f,
            shopLayer
        );

        if (hit != null)
            return false;

        float bottomY = position.y - shopSize.y / 2f;
        float leftX = position.x - shopSize.x / 2f;
        float rightX = position.x + shopSize.x / 2f;

        for (int i = 0; i < supportCheckPoints; i++)
        {
            float t = supportCheckPoints == 1 ? 0.5f : (float)i / (supportCheckPoints - 1);
            float checkX = Mathf.Lerp(leftX, rightX, t);

            Vector2 checkPos = new Vector2(checkX, bottomY - 0.05f);

            Collider2D support = Physics2D.OverlapBox(
                checkPos,
                new Vector2(0.2f, 0.2f),
                0f,
                supportLayer
            );

            if (support == null)
                return false;
        }

        return true;
    }

    void SetPreviewColor(Color color)
    {
        color.a = 0.45f;
        previewRenderer.color = color;
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "Money: $" + money;
    }

    public void SelectShop(GameObject shopPrefab)
    {
        selectedShopPrefab = shopPrefab;
        currentShopData = shopPrefab.GetComponent<ShopData>();

        if (previewShop != null)
            Destroy(previewShop);

        previewShop = Instantiate(shopPrefab);
        previewShop.name = "Preview Shop";

        foreach (Collider2D col in previewShop.GetComponents<Collider2D>())
            col.enabled = false;

        previewRenderer = previewShop.GetComponent<SpriteRenderer>();
        buildMode = true;
    }
}