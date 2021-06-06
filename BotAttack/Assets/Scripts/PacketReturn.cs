using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketReturn : MonoBehaviour
{
    PacketPool packetPool;
    // Start is called before the first frame update
    void Start()
    {
        packetPool = FindObjectOfType<PacketPool>();
    }

    private void OnDisable()
    {
        if (packetPool != null)
        {
            packetPool.ReturnPacket(this.gameObject);
        }
    }

}
