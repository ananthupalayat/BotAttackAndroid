using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class Client : MonoBehaviour
{

    [SerializeField]
    Transform TransformOne;

    private Transform TransformTwo;



    [SerializeField]
    GameObject goodPacket;

    [SerializeField]
    GameObject badPacket;


    public float speed;

    
    public int offset;

    [SerializeField]
    float currentSpawnRate = 5f;
    [SerializeField]
    float elapsedTime;

    float bigCountDown = 10; 
    float currentBigTime = 0;

    Vector3 startPos;
    Vector3 endPos;

    bool someOneOnLine = false;

    public static event Action recieveGoodPacket;
    public static event Action DestroyedBadPacket;
    public static event Action VirusDamage;


    [SerializeField]
    GameObject cylinderPrefab;

    GameObject cylinder;

    bool lineActive=true;

    [SerializeField]
    Material BadConnection;
    [SerializeField]
    Material GoodConnection;
    [SerializeField]
    Material NullConnection;

    private MeshRenderer lineMaterial;

    private PacketPool packetPool;

    private void Awake()
    {
        TransformOne = GameObject.Find("Server").transform;
        TransformTwo = transform;
        startPos = TransformTwo.position;
        endPos = TransformOne.position;
        CreateCylinderBetweenPoints(startPos, endPos, 0.5f);
        lineMaterial=cylinder.GetComponent<MeshRenderer>();
        lineMaterial.material = GoodConnection;
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

        if (elapsedTime >= currentSpawnRate && someOneOnLine==false && lineActive)
        {
            CreatePacket();
            elapsedTime = 0;
        }

        if (currentBigTime >= bigCountDown)
        {
            if (currentSpawnRate == 1)
            {
                currentSpawnRate = 1; 
            }
            else
            {
                currentSpawnRate -= 1f;
            }
            currentBigTime = 0;
        }


    }


    void CreatePacket()
    {
        if (!lineActive) return;
        int id = UnityEngine.Random.Range(0, 2);
        GameObject packetSpawn;
        if (id == 1)
        {
            lineMaterial.material = BadConnection;
            packetSpawn = packetPool.GetPacket(badPacket);
            
        }
        else
        {
            lineMaterial.material = GoodConnection;
            packetSpawn = packetPool.GetPacket(goodPacket);
        }
        packetSpawn.transform.position = startPos;
        someOneOnLine = true;
        StartCoroutine(TestMove(packetSpawn));
        
    }

    IEnumerator TestMove(GameObject packetIn)
    {
        if (!lineActive) yield return null;

        while (packetIn.transform.position != endPos)
        {
            if (!lineActive)
            {
                someOneOnLine = false;
                if (packetIn.name == "BadPacket")
                {
                    
                    DestroyedBadPacket?.Invoke();
                    packetIn.SetActive(false);
                }
                else
                {
                   
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
            VirusDamage?.Invoke();
        }
        else
        {
            recieveGoodPacket?.Invoke();
        }
        
        packetIn.SetActive(false);
    }

    IEnumerator ReconnectLine()
    {
        yield return new WaitForSeconds(5);
        //cylinder.SetActive(true);
        someOneOnLine = false;
        lineActive = true;
        lineMaterial.material = GoodConnection;
        
    }

    void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);

        cylinder = Instantiate(cylinderPrefab, position, Quaternion.identity,transform);
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
    }

    public void DisconnectLine()
    {
        lineActive = false;
        StartCoroutine(ReconnectLine());
        lineMaterial.material = NullConnection;
    }
    
    
    
}



