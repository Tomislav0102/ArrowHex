using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    Vector3 _dir;
    public float moveSpeed = 10f;
    public float rotSpeed = 100f;
    public GameObject[] effects;
    int _counter;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.position += v * moveSpeed * Time.deltaTime * transform.forward;
        transform.Rotate(rotSpeed * h * Time.deltaTime * Vector3.up);

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            if (Input.GetKeyDown(KeyCode.E)) _counter = (1 + _counter) % effects.Length;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _counter--;
                if (_counter < 0) _counter = effects.Length - 1;
            }
            Utils.ActivateOneArrayElement(effects, _counter);
            effects[_counter].GetComponent<PSMeshRendererUpdater>().UpdateMeshEffect();
        }
    }
}
