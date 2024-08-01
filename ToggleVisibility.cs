using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    // 在Inspector中显示的GameObject数组，用于存放要控制的物体
    public GameObject[] objectsToToggle;
    // 记录当前的显示状态
    private bool isVisible = false;

    void Update()
    {
        // 检测是否按下了Shift键
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 切换显示状态
            isVisible = !isVisible;

            // 遍历指定的物体，设置它们的显示状态
            foreach (GameObject obj in objectsToToggle)
            {
                // 检查物体是否有Renderer组件，如果有，则设置其可见性
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = isVisible;
                }

                // 额外的，如果想要控制物体的子物体的显示状态，可以遍历子物体
                // 并对每个子物体执行相同的可见性设置
                foreach(Transform child in obj.transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.enabled = isVisible;
                    }
                }
            }
        }
    }
}
