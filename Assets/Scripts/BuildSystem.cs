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
    public float floorBaseY = 0f;

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

        Vector2 buildPosition = GetBuildPosition(mouseWorldPos);

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

    Vector2 GetBuildPosition(Vector2 mouseWorldPos)
    {
        float snappedX = Mathf.Round(mouseWorldPos.x / horizontalSnap) * horizontalSnap;

        float floorY = Mathf.Round((mouseWorldPos.y - floorBaseY) / floorHeight) * floorHeight + floorBaseY;

        float colliderHeight = previewCollider.size.y * Mathf.Abs(previewShop.transform.localScale.y);
        float colliderOffsetY = previewCollider.offset.y * previewShop.transform.localScale.y;

        float finalY = floorY + (colliderHeight / 2f) - colliderOffsetY;

        return new Vector2(snappedX, finalY);
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

            Vector2 checkPos = new Vector2(checkX, bottomY - 0.03f);

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
        if (moneyText != null)
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

        SetLayerRecursively(previewShop, LayerMask.NameToLayer("Default"));

        previewRenderer = previewShop.GetComponent<SpriteRenderer>();
        previewCollider = previewShop.GetComponent<BoxCollider2D>();

        if (previewRenderer == null || previewCollider == null || currentShopData == null)
        {
            Debug.LogError("Shop prefabýnda SpriteRenderer, BoxCollider2D veya ShopData eksik!");
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