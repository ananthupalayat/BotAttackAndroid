using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    GameObject parent;

    private void Awake()
    {
        parent = transform.root.gameObject;
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            parent.GetComponent<Client>().DisconnectLine();
        }
    }
}
