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
    public float floorHeight = 2.85f;
    public int supportCheckPoints = 8;

    private GameObject previewShop;
    private SpriteRenderer previewRenderer;
    private BoxCollider2D previewCollider;
    private ShopData currentShopData;

    private bool buildMode = false;

    void Start()
    {
        UpdateMoneyUI();
    }

    void Update()
    {
        if (!buildMode || previewShop == null)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            previewShop.SetActive(false);
            return;
        }

        previewShop.SetActive(true);

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float snappedX = Mathf.Round(mouseWorldPos.x / horizontalSnap) * horizontalSnap;
        float snappedY = Mathf.Round(mouseWorldPos.y / floorHeight) * floorHeight;

        Vector2 buildPosition = new Vector2(snappedX, snappedY);

        previewShop.transform.position = buildPosition;
        Physics2D.SyncTransforms();

        bool canBuild = CanBuild();

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

    bool CanBuild()
    {
        Bounds bounds = previewCollider.bounds;

        if (buildArea != null && !buildArea.OverlapPoint(bounds.center))
            return false;

        Collider2D hit = Physics2D.OverlapBox(
            bounds.center,
            bounds.size * 0.95f,
            0f,
            shopLayer
        );

        if (hit != null)
            return false;

        float leftX = bounds.min.x;
        float rightX = bounds.max.x;
        float bottomY = bounds.min.y;

        for (int i = 0; i < supportCheckPoints; i++)
        {
            float t = supportCheckPoints == 1 ? 0.5f : (float)i / (supportCheckPoints - 1);
            float checkX = Mathf.Lerp(leftX, rightX, t);

            Vector2 checkPos = new Vector2(checkX, bottomY - 0.05f);

            Collider2D support = Physics2D.OverlapBox(
                checkPos,
                new Vector2(0.25f, 0.25f),
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
        if (previewRenderer == null)
            return;

        color.a = 0.45f;
        previewRenderer.color = color;
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "Money: $" + money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public void SelectShop(GameObject shopPrefab)
    {
        selectedShopPrefab = shopPrefab;
        currentShopData = shopPrefab.GetComponent<ShopData>();

        if (previewShop != null)
            Destroy(previewShop);

        previewShop = Instantiate(shopPrefab);
        previewShop.name = "Preview Shop";

        SetLayerRecursively(previewShop, LayerMask.NameToLayer("Default"));

        previewRenderer = previewShop.GetComponentInChildren<SpriteRenderer>();
        previewCollider = previewShop.GetComponent<BoxCollider2D>();

        if (previewRenderer == null)
        {
            Debug.LogError("Preview Shop içinde SpriteRenderer yok.");
            return;
        }

        if (previewCollider == null)
        {
            Debug.LogError("Preview Shop ana objesinde BoxCollider2D yok.");
            return;
        }

        if (currentShopData == null)
        {
            Debug.LogError("Shop prefabýnda ShopData yok.");
            return;
        }

        buildMode = true;
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}