using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI totalVirusDestroyedCount;

    [SerializeField]
    TextMeshProUGUI totalGoodPacketsCount;

    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Slider antiVirusSlider;

    private void OnEnable()
    {
        Server.VirusTerminated += DisplayVirusKilled;
        Server.GoodPacketRecieved += DisplayGoodPackets;
        Server.HealthUpdate += DisplayHealth;
        Server.AntiVirusUpdate += DisplayAntiVirusBar;
    }

    private void OnDisable()
    {
        Server.VirusTerminated -= DisplayVirusKilled;
        Server.GoodPacketRecieved -= DisplayGoodPackets;
        Server.HealthUpdate -= DisplayHealth;
        Server.AntiVirusUpdate -= DisplayAntiVirusBar;
    }

    public void DisplayVirusKilled(int value)
    {
        totalVirusDestroyedCount.text = "Virus Terminated: "+value.ToString();
    }

    public void DisplayGoodPackets(int value)
    {
        totalGoodPacketsCount.text = "Good Packets: " + value.ToString();
    }

    public void DisplayHealth(int value)
    {
        healthSlider.value = value;
    }

    public void DisplayAntiVirusBar(int value)
    {
        antiVirusSlider.value = value;
    }

}
