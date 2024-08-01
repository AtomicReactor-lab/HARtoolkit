using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    public GameObject highlight;
    public GameObject relines;
    public GameObject meanline;

    private GameObject[] objects;
    private int currentIndex = -1;

    void Start()
    {
        // 初始化 GameObject 数组
        objects = new GameObject[] { highlight, relines, meanline };

        // 开始时隐藏所有对象
        HideAllObjects();
    }

    void Update()
    {
        // 检测 L 键的按下
        if (Input.GetKeyDown(KeyCode.L))
        {
            // 更新索引
            currentIndex = (currentIndex + 1) % objects.Length;

            // 显示下一个对象并隐藏其他对象
            ShowOnlyCurrentObject();
        }
    }

    private void HideAllObjects()
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void ShowOnlyCurrentObject()
    {
        HideAllObjects();
        if (objects[currentIndex] != null)
        {
            objects[currentIndex].SetActive(true);
        }
    }
}

