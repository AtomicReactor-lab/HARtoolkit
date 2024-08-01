using UnityEngine;

public class HideTargetObject : MonoBehaviour
{
    public GameObject targetObject; // Drag the object you want to hide/show here in the Inspector
    private bool isHidden = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ToggleVisibility();
        }
    }

    void ToggleVisibility()
    {
        if (targetObject != null)
        {
            isHidden = !isHidden;
            targetObject.SetActive(!isHidden);
        }
    }
}
