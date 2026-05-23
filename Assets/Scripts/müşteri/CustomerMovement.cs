using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Animator animator;

    private int direction = 1;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (Random.value > 0.5f)
        {
            direction = -1;
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void Update()
    {
        transform.Translate(
            Vector2.right * direction * moveSpeed * Time.deltaTime
        );

        animator.SetBool("isWalking", true);
    }
}