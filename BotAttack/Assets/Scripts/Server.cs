using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public  class Server : MonoBehaviour
{

    [SerializeField]
    private int noOfGoodPackets = 0;
    [SerializeField]
    private int noOfBadPacketsTerminated = 0;

    [SerializeField]
    int health = 10;

    bool shieldUp = false;

    [SerializeField]
    GameObject shield;

    int tempGoodPackets = 0;

    public static event Action UpClient;

    public static event Action<int> VirusTerminated;
    public static event Action<int> GoodPacketRecieved;
    public static event Action<int> HealthUpdate;
    public static event Action<int> AntiVirusUpdate;


    [SerializeField]
    int tempTerminationPackages;

    private void OnEnable()
    {
        Client.recieveGoodPacket += UpdateGoodPacket;
        Client.DestroyedBadPacket += UpdateVirusPackets;
        Client.VirusDamage += RecieveDamage;
    }

    private void OnDisable()
    {
        Client.recieveGoodPacket -= UpdateGoodPacket;
        Client.DestroyedBadPacket -= UpdateVirusPackets;
        Client.VirusDamage -= RecieveDamage;
    }

    // Start is called before the first frame update
    void Start()
    {
        tempGoodPackets = 0;
        health = 10;
        HealthUpdate?.Invoke(health);
        AntiVirusUpdate?.Invoke(tempGoodPackets);
    }

    // Update is called once per frame
    void Update()
    {
        if (tempTerminationPackages >= 5)
        {
            tempTerminationPackages = 0;
            UpClient?.Invoke();
        }

        if (tempGoodPackets >= 5)
        {
            tempGoodPackets = 0;
            shieldUp = true;
            StartCoroutine(ShieldUp());
        }
        if (health < 0)
        {
            SceneManager.LoadScene(0);
        }
    }

   

    void UpdateGoodPacket()
    {
        noOfGoodPackets += 1;
        GoodPacketRecieved?.Invoke(noOfGoodPackets);
        if (!shieldUp)
        {
            tempGoodPackets += 1;
            AntiVirusUpdate?.Invoke(tempGoodPackets);
        }
    }

    void UpdateVirusPackets()
    {
        noOfBadPacketsTerminated += 1;
        tempTerminationPackages +=1;
        VirusTerminated?.Invoke(noOfBadPacketsTerminated);
    }

    void RecieveDamage()
    {
        if (shieldUp) return;
        health -= 1;
        HealthUpdate?.Invoke(health);
    }

    IEnumerator ShieldUp()
    {
        shield.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        shieldUp = false;
        AntiVirusUpdate?.Invoke(tempGoodPackets);
        shield.gameObject.SetActive(false);
    }
}
