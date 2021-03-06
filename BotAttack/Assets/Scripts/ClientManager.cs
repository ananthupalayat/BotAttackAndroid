using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClientManager : MonoBehaviour
{
    //To track position of each client
    List<Transform> clientTransforms=new List<Transform>();    
   

    [SerializeField]
    GameObject client;

    //count of client created
    int generation = 0;

    //First position the client needs to be spawned
    [SerializeField]
    GameObject startinPosition;

    public static event Action RowCreated;


    private void OnEnable()
    {
        Server.UpClient += CreateClient;
    }

    private void OnDisable()
    {
        Server.UpClient -= CreateClient;
    }

    void Start()
    {
        StartCoroutine(StartGame());
    }

    /// <summary>
    /// Creates 3 clients during start of the game
    /// </summary>
    /// <returns></returns>
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        var start = Instantiate(client, startinPosition.transform.position, Quaternion.identity);
        clientTransforms.Add(start.transform);
        for (int i = 0; i < 2; i++)
        {
            CreateClient();
        }
    }


    /// <summary>
    /// Creates a new client
    /// </summary>
    int direction = 1;
    void CreateClient()
    {
        generation += 1;
        Vector3 position = clientTransforms[generation-1].position;
        Vector3 offset;
        int mod = generation % 3;
        Vector3 random;
        if (mod == 0)
        {
            Vector3 pos = Vector3.right * UnityEngine.Random.Range(-3f, 3f);
            offset = Vector3.back * 10+pos;
            direction *= -1;
            RowCreated?.Invoke();
        }
        else
        {
            offset = direction*Vector3.right*10;
        }
        position += offset;
        
        var prefab = Instantiate(client, position,Quaternion.identity);
        clientTransforms.Add(prefab.transform);
    }
}
