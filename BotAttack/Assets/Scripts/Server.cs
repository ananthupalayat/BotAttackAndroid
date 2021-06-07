using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public  class Server : MonoBehaviour
{


    //Maximum Health of the player
    [SerializeField]
    float MaxHealth = 10f;

    //Maximum Good Packets to capture to get shield
    [SerializeField]
    int MaxAntivirusBoost = 5;

    //Maximum Bad Packets to destroy to create a new client
    [SerializeField]
     int MaxVirusTermination = 5;

    //health of player
    float currentHealth ;

    bool shieldUp = false;

    [SerializeField]
    GameObject shield;

    //variables to check for achievments
    int tempTerminationPackages=0;
    int tempGoodPackets = 0;

    //Total good packets Captured
    int totalGoodPackets = 0;
    //Total bad packets terminated
    int totalBadPacketsTerminated = 0;

    Animator animator;


    public static event Action UpClient;
    public static event Action<int> VirusTerminated;
    public static event Action<int> GoodPacketRecieved;
    public static event Action<float,float> HealthUpdate;
    public static event Action<float,int> AntiVirusUpdate;
    public static event Action<float, float> CameraEvent;



    private void OnEnable()
    {
        Client.recieveGoodPacket += UpdateGoodPacket;
        Client.DestroyedBadPacket += UpdateVirusPackets;
        Client.VirusDamage += RecieveDamage;
        Client.recieveGoodPacket += ServerJiggle;
        Client.VirusDamage += ServerJiggle;
    }

    private void OnDisable()
    {
        Client.recieveGoodPacket -= UpdateGoodPacket;
        Client.DestroyedBadPacket -= UpdateVirusPackets;
        Client.VirusDamage -= RecieveDamage;
        Client.recieveGoodPacket -= ServerJiggle;
        Client.VirusDamage -= ServerJiggle;
    }

    // Start is called before the first frame update
    void Start()
    {
        tempGoodPackets = 0;
        currentHealth = MaxHealth;
        HealthUpdate?.Invoke(currentHealth,MaxHealth);
        AntiVirusUpdate?.Invoke(tempGoodPackets,MaxAntivirusBoost);
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Entry");
    }

    // Update is called once per frame
    void Update()
    {
        if (tempTerminationPackages >= MaxVirusTermination)
        {
            tempTerminationPackages = 0;
            UpClient?.Invoke();
            //CamersShake.Instance.WidenRange(ClientManager.Instance.generation);
        }

        
        if (tempGoodPackets >= MaxAntivirusBoost)
        {
            tempGoodPackets = 0;
            shieldUp = true;
            StartCoroutine(ShieldUp());
        }

        //Player Dies if Health Reaches Zero
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

   

    void UpdateGoodPacket()
    {
        totalGoodPackets += 1;
        GoodPacketRecieved?.Invoke(totalGoodPackets);
        if (!shieldUp)
        {
            tempGoodPackets += 1;
            AntiVirusUpdate?.Invoke(tempGoodPackets,MaxAntivirusBoost);
        }
    }

    void UpdateVirusPackets()
    {
        totalBadPacketsTerminated += 1;
        tempTerminationPackages +=1;
        VirusTerminated?.Invoke(totalBadPacketsTerminated);
    }

    void RecieveDamage()
    {
        if (shieldUp) return;
        currentHealth -= 1;
        CameraEvent?.Invoke(5f, 0.3f);
        HealthUpdate?.Invoke(currentHealth,MaxHealth);
    }

    IEnumerator ShieldUp()
    {
        shield.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        shieldUp = false;
        AntiVirusUpdate?.Invoke(tempGoodPackets,MaxAntivirusBoost);
        shield.gameObject.SetActive(false);
    }

    void ServerJiggle()
    {
        animator.SetTrigger("Jiggle");
    }
}
