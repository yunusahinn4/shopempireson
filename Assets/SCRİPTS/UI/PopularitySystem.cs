using UnityEngine;
using TMPro;

public class PopularitySystem : MonoBehaviour
{
    public RectTransform fillBar;

    public TextMeshProUGUI popularityText;

    [Range(0f, 100f)]
    public float popularity = 0;

    public float maxWidth = 450f;

    void Update()
    {
        float width =
            (popularity / 100f) * maxWidth;

        fillBar.sizeDelta =
            new Vector2(width, fillBar.sizeDelta.y);

        popularityText.text =
            "Popularity " + Mathf.RoundToInt(popularity) + "%";
    }

    public void AddPopularity(float amount)
    {
        popularity += amount;

        popularity = Mathf.Clamp(popularity, 0, 100);
    }
}