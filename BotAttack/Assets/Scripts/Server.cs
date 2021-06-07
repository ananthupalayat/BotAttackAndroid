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

    //duration of shield time
    [SerializeField]
    float shieldDuration = 10f;

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

    #region Events
    public static event Action UpClient;
    public static event Action<int> VirusTerminated;
    public static event Action<int> GoodPacketRecieved;
    public static event Action<float,float> HealthUpdate;
    public static event Action<float ,int,float> AntiVirusUpdate;
    public static event Action<float, float> CameraEvent;
    #endregion


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

    
    void Start()
    {
        tempGoodPackets = 0;
        currentHealth = MaxHealth;
        HealthUpdate?.Invoke(currentHealth,MaxHealth);
        AntiVirusUpdate?.Invoke(tempGoodPackets,MaxAntivirusBoost,shieldDuration);
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Entry");
    }

  
    void Update()
    {
        if (tempTerminationPackages >= MaxVirusTermination)
        {
            tempTerminationPackages = 0;
            UpClient?.Invoke();
        }

        
        if (tempGoodPackets >= MaxAntivirusBoost)
        {
            tempGoodPackets = 0;
            shieldUp = true;
            StartCoroutine(ShieldUp(shieldDuration));
        }

        //Player Dies if Health Reaches Zero
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

   
    /// <summary>
    /// Increments the good packet server has collected
    /// </summary>
    void UpdateGoodPacket()
    {
        totalGoodPackets += 1;
        GoodPacketRecieved?.Invoke(totalGoodPackets);
        if (!shieldUp)
        {
            tempGoodPackets += 1;
            AntiVirusUpdate?.Invoke(tempGoodPackets,MaxAntivirusBoost,shieldDuration);
        }
    }

    /// <summary>
    /// Updates bad packets terminated by server
    /// </summary>
    void UpdateVirusPackets()
    {
        totalBadPacketsTerminated += 1;
        tempTerminationPackages +=1;
        VirusTerminated?.Invoke(totalBadPacketsTerminated);
    }

    /// <summary>
    /// Decreases health of the server
    /// </summary>
    void RecieveDamage()
    {
        if (shieldUp) return;
        currentHealth -= 1;
        CameraEvent?.Invoke(5f, 0.3f);
        HealthUpdate?.Invoke(currentHealth,MaxHealth);
    }

    /// <summary>
    /// Shields up when antivirus bar is full
    /// </summary>
    /// <returns></returns>
    IEnumerator ShieldUp(float duration)
    {
        shield.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration+1);
        shieldUp = false;
        AntiVirusUpdate?.Invoke(tempGoodPackets,MaxAntivirusBoost,duration);
        shield.gameObject.SetActive(false);
    }

    /// <summary>
    /// Makes server shake when recieving packet
    /// </summary>
    void ServerJiggle()
    {
        animator.SetTrigger("Jiggle");
    }
}
