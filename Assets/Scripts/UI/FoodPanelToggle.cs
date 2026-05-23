using UnityEngine;

public class FoodPanelToggle : MonoBehaviour
{
    public GameObject foodPanel;

    public void ToggleFoodPanel()
    {
        foodPanel.SetActive(!foodPanel.activeSelf);
    }
}