using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketPool : MonoBehaviour
{
    Dictionary<string, Queue<GameObject>> packetPool = new Dictionary<string, Queue<GameObject>>();

    public GameObject GetPacket(GameObject packet)
    {
        if(packetPool.TryGetValue(packet.name,out Queue<GameObject> packetList))
        {
            if (packetList.Count == 0)
            {
                return CreateNewPacket(packet);
            }
            else
            {
                GameObject _packet = packetList.Dequeue();
                _packet.SetActive(true);
                return _packet;
            }
        }
        else
        {
            return CreateNewPacket(packet);
        }
    }

    private GameObject CreateNewPacket(GameObject packet)
    {
        GameObject NewPacket = Instantiate(packet,transform);
        NewPacket.name = packet.name;
        return NewPacket;

    }

    public void ReturnPacket(GameObject packet)
    {
        if(packetPool.TryGetValue(packet.name,out Queue<GameObject> packetList))
        {
            packetList.Enqueue(packet);
        }
        else
        {
            Queue<GameObject> newPacketQueue = new Queue<GameObject>();
            newPacketQueue.Enqueue(packet);
            packetPool.Add(packet.name, newPacketQueue);
        }

        packet.SetActive(false);

    }

}
