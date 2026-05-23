using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;

    private int direction;

    void Start()
    {
        // Rastgele yön seç
        direction = Random.value > 0.5f ? 1 : -1;

        // Sprite yönünü çevir
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void Update()
    {
        // Hareket
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
    }
}