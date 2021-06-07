using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketReturn : MonoBehaviour
{
    PacketPool packetPool;
   
    void Start()
    {
        packetPool = FindObjectOfType<PacketPool>();
    }
    
    /// <summary>
    /// Returns to their respective pool when disabled
    /// </summary>
    private void OnDisable()
    {
        if (packetPool != null)
        {
            packetPool.ReturnPacket(this.gameObject);
        }
    }

}
