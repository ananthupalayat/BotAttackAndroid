using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class Client : MonoBehaviour
{


    #region prefab support for pooling system
    [SerializeField]
    GameObject goodPacket;

    [SerializeField]
    GameObject badPacket;

    [SerializeField]
    GameObject goodPacketBurst;

    [SerializeField]
    GameObject badPacketBurst;
    #endregion

    [SerializeField]
     float speed=3f;

    [SerializeField]
    float currentPacketSpawnRate = 10f;

    [SerializeField]
    float minimumPacketSpawnRate = 1f;

    float elapsedTime;
    float bigCountDown = 10; 
    float currentBigTime = 0;

    bool someOneOnLine = false;

    [SerializeField]
    GameObject linePrefab;
    GameObject line;
    int offset;
    MeshRenderer lineMaterial;
    Vector3 startPos;
    Vector3 endPos;


    #region Color Support for lines
    [SerializeField]
    Color BadConnectionColor=Color.red;
    [SerializeField]
    Color GoodConnectionColor=Color.green;
    [SerializeField]
    Color NullConnectionColor=Color.black;
    #endregion

    bool lineActive =true;

    PacketPool packetPool;

    #region event support
    public static event Action recieveGoodPacket;
    public static event Action DestroyedBadPacket;
    public static event Action VirusDamage;
    #endregion

    Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Entry");
        endPos = GameObject.Find("Server").transform.position;
        startPos = transform.position;
        CreateLineBetweenPoints(startPos, endPos, 0.5f);

        lineMaterial=line.GetComponent<MeshRenderer>();
        lineMaterial.material.color = GoodConnectionColor;
    }


    void Start()
    {
        packetPool = FindObjectOfType<PacketPool>();
        CreatePacket();
    }



    void Update()
    {
        elapsedTime += Time.deltaTime;
        currentBigTime += Time.deltaTime;

        if (elapsedTime >= currentPacketSpawnRate && someOneOnLine==false && lineActive)
        {
            CreatePacket();
            elapsedTime = 0;
        }

        if (currentBigTime >= bigCountDown && lineActive)
        {
            if (currentPacketSpawnRate == minimumPacketSpawnRate  )
            {
                
                currentPacketSpawnRate = minimumPacketSpawnRate; 
            }
            else
            {
                currentPacketSpawnRate -= 0.5f;
                
            }
            currentBigTime = 0;
        }


    }


    /// <summary>
    /// Creates a new packet randomly
    /// </summary>
    void CreatePacket()
    {
        animator.SetTrigger("Pop");
        if (!lineActive) return;
        int id = UnityEngine.Random.Range(0, 2);
        GameObject packetSpawn;
        if (id == 1)
        {
            lineMaterial.material.color = BadConnectionColor;
            packetSpawn = packetPool.GetPacket(badPacket);
            
        }
        else
        {
            lineMaterial.material.color = GoodConnectionColor;
            packetSpawn = packetPool.GetPacket(goodPacket);
        }
        packetSpawn.transform.position = startPos;
        someOneOnLine = true;
        StartCoroutine(MovePacket(packetSpawn));
        packetSpawn = null;
    }

    /// <summary>
    /// Moves the packet from Client to Server
    /// </summary>
    /// <param name="packetIn: The packet moving on this line"></param>
    /// <returns></returns>
    IEnumerator MovePacket(GameObject packetIn)
    {
        GameObject burst;
        if (!lineActive)
        {
            yield return null;
        } 
        
        //Packet Moving to end of line
        while (packetIn.transform.position != endPos)
        {
            if (!lineActive)
            {
                someOneOnLine = false;
                if (packetIn.name == "BadPacket")
                {
                    burst= packetPool.GetPacket(badPacketBurst);
                    burst.transform.position = packetIn.transform.position;
                    StartCoroutine(TurnOffParticleSystem(burst));
                    DestroyedBadPacket?.Invoke();
                    packetIn.SetActive(false);
                }
                else
                {
                    burst = packetPool.GetPacket(goodPacketBurst);
                    burst.transform.position = packetIn.transform.position;
                    StartCoroutine(TurnOffParticleSystem(burst));
                    packetIn.SetActive(false);
                    
                }
                
                yield break;
            }
            else
            {
                packetIn.transform.position = Vector3.MoveTowards(packetIn.transform.position,
                                                endPos,
                                                speed * Time.deltaTime);
                yield return null;
            }
            
        }

        //Packet reached end of line
        someOneOnLine = false;
        if (packetIn.name == "BadPacket")
        {
            burst = packetPool.GetPacket(badPacketBurst);
            burst.transform.position = packetIn.transform.position;
            StartCoroutine(TurnOffParticleSystem(burst));
            VirusDamage?.Invoke();
            
        }
        else
        {
            burst = packetPool.GetPacket(goodPacketBurst);
            burst.transform.position = packetIn.transform.position;
            StartCoroutine(TurnOffParticleSystem(burst));
            recieveGoodPacket?.Invoke();
        }
        packetIn.SetActive(false);
    }

    /// <summary>
    /// Reconnects the line after few seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator ReconnectLine()
    {
        yield return new WaitForSeconds(4);
        someOneOnLine = false;
        lineActive = true;
        
    }

    /// <summary>
    /// Creates a line between Client and server
    /// </summary>
    /// <param name="start: starting position "></param>
    /// <param name="end: destination position"></param>
    /// <param name="width: width of the line"></param>
    void CreateLineBetweenPoints(Vector3 start, Vector3 end, float width)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);

        line = Instantiate(linePrefab, position, Quaternion.identity,transform);
        line.transform.up = offset;
        line.transform.localScale = scale;
    }

    /// <summary>
    /// Disconnects the line when clicked
    /// </summary>
    public void DisconnectLine()
    {
        lineActive = false;
        lineMaterial.material.color = NullConnectionColor;
        StartCoroutine(ReconnectLine());
    }
    
    IEnumerator TurnOffParticleSystem(GameObject particleSystem)
    {
        yield return new WaitForSeconds(2);
        particleSystem.SetActive(false);
    }
    
}



