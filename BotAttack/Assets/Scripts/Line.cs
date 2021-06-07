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

    /// <summary>
    /// Use to register taping on Line to parent
    /// </summary>
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            parent.GetComponent<Client>().DisconnectLine();
        }
    }
}
