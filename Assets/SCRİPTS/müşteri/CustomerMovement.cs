using UnityEngine;
using System.Collections;

public class CustomerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float doorMoveSpeed = 3f;

    [Range(0f, 1f)]
    public float enterShopChance = 0.35f;

    public float doorEnterDistance = 1f;

    private int direction = 1;
    private bool isShopping = false;

    private float fixedY;

    private SpriteRenderer spriteRenderer;
    private ShopEntrance targetShop;
    private BuildSystem buildSystem;

    void Start()
    {
        fixedY = transform.position.y;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        buildSystem = FindObjectOfType<BuildSystem>();
    }

    void Update()
    {
        if (isShopping)
            return;

        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;

        if (targetShop != null && targetShop.doorPoint != null)
        {
            float distanceToDoor = Vector2.Distance(transform.position, targetShop.doorPoint.position);

            if (distanceToDoor <= doorEnterDistance)
            {
                StartCoroutine(VisitShop(targetShop));
                targetShop = null;
            }
        }
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection >= 0 ? 1 : -1;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ShopEntrance shop = other.GetComponentInParent<ShopEntrance>();

        if (shop == null)
            return;

        if (Random.value > enterShopChance)
            return;

        targetShop = shop;
    }

    IEnumerator VisitShop(ShopEntrance shop)
    {
        isShopping = true;

        Vector3 continuePos = transform.position;

        Vector3 doorPos = shop.doorPoint != null
            ? shop.doorPoint.position
            : shop.transform.position;

        doorPos.y = fixedY;

        while (Vector2.Distance(transform.position, doorPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                doorPos,
                doorMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(shop.shopData.visitTime);

        if (buildSystem != null)
        {
            buildSystem.AddMoney(shop.shopData.incomePerCustomer);
        }

        transform.position = continuePos;

        spriteRenderer.enabled = true;
        isShopping = false;
    }
}