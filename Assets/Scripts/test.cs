using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class test : MonoBehaviour
{
    Queue<int> queue = new Queue<int>();
    Stack<int> stack = new Stack<int>();
    public int num = 3;

    [Button]
    void ClickMethodEnqueue()
    {
        for (int i = 0; i < num; i++)
        {
            queue.Enqueue(i);
            stack.Push(i);
        }
        DisplayQueue();
        print("-----");
    }

    [Button]
    void ClickMethodDequeue()
    {
        queue.Dequeue();
        stack.Pop();
        DisplayQueue();
    }

    void DisplayQueue()
    {
        foreach (int item in queue)
        {
            print($"queue {item}");
        }
        // foreach (int item in stack)
        // {
        //     print($"stack {item}");
        // }
    }

    public void M1()
    {
        print("clicked");
    }
    public void M2()
    {
        print("c2");
    }
    public void M3()
    {
        print("c3");
    }
}

// public List<RectTransform> group;
// public int moveX = 250;
//     
// [Button]
// void ClickMethod()
// {
//     for (int i = 0; i < group.Count; i++)
//     {
//         group[i].anchoredPosition = new Vector2(group[i].anchoredPosition.x + moveX, group[i].anchoredPosition.y);
//     }
//     group.Clear();
// }


// public Texture2D texture;
// public Image image;
//
//
// [Button]
// void ClickMethod()
// {
//     image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
// }

