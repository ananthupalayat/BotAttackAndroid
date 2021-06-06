using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class Client : MonoBehaviour
{

    

    [SerializeField]
    GameObject goodPacket;

    [SerializeField]
    GameObject badPacket;

    [SerializeField]
    GameObject goodPacketBurst;

    [SerializeField]
    GameObject badPacketBurst;

    [SerializeField]
     float speed=2f;

    [SerializeField]
     float maxSpeed = 8f;
    

    [SerializeField]
    float currentPacketSpawnRate = 10f;

    [SerializeField]
    float minimumPacketSpawnRate = 1f;

    float elapsedTime;
    float bigCountDown = 10; 
    float currentBigTime = 0;

    Vector3 startPos;
    Vector3 endPos;

    bool someOneOnLine = false;

    [SerializeField]
    GameObject linePrefab;
    GameObject line;

     int offset;
     MeshRenderer lineMaterial;

    [SerializeField]
    Color BadConnectionColor=Color.red;
    [SerializeField]
    Color GoodConnectionColor=Color.green;
    [SerializeField]
    Color NullConnectionColor=Color.black;

    bool lineActive=true;

    
    PacketPool packetPool;


    public static event Action recieveGoodPacket;
    public static event Action DestroyedBadPacket;
    public static event Action VirusDamage;

    Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Entry");
        endPos = GameObject.Find("Server").transform.position;
        startPos = transform.position;
        CreateCylinderBetweenPoints(startPos, endPos, 0.5f);

        lineMaterial=line.GetComponent<MeshRenderer>();
        lineMaterial.material.color = GoodConnectionColor;
    }


    void Start()
    {
        packetPool = FindObjectOfType<PacketPool>();
        CreatePacket();
    }


    // Update is called once per frame
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
            if (currentPacketSpawnRate == minimumPacketSpawnRate || speed==maxSpeed )
            {
                speed = maxSpeed;
                currentPacketSpawnRate = minimumPacketSpawnRate; 
            }
            else
            {
                currentPacketSpawnRate -= 0.5f;
                speed += 0.5f;
            }
            currentBigTime = 0;
        }


    }


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
        
    }

    IEnumerator MovePacket(GameObject packetIn)
    {
        GameObject burst;
        if (!lineActive)
        {
            yield return null;
        } 
        
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

    IEnumerator ReconnectLine()
    {
        yield return new WaitForSeconds(5);
        someOneOnLine = false;
        lineActive = true;
        lineMaterial.material.color = GoodConnectionColor;
        
    }

    void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);

        line = Instantiate(linePrefab, position, Quaternion.identity,transform);
        line.transform.up = offset;
        line.transform.localScale = scale;
    }

    public void DisconnectLine()
    {
        lineActive = false;
        StartCoroutine(ReconnectLine());
        lineMaterial.material.color = NullConnectionColor;
    }
    
    IEnumerator TurnOffParticleSystem(GameObject particleSystem)
    {
        yield return new WaitForSeconds(2);
        particleSystem.SetActive(false);
    }
    
}



