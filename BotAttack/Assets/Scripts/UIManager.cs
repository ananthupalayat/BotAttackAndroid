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
    GameObject popUpGoodPacket;

    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Slider antiVirusSlider;

    PacketPool pacaketPool;

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

    private void Awake()
    {
        pacaketPool = FindObjectOfType<PacketPool>();
    }

    /// <summary>
    /// Displays Virus Terminated on UI
    /// </summary>
    /// <param name="value"></param>
    public void DisplayVirusKilled(int value)
    {
        totalVirusDestroyedCount.text = "Virus Terminated: "+value.ToString();
    }

    /// <summary>
    /// Displays Total good packets collected on UI
    /// </summary>
    /// <param name="value"></param>
    public void DisplayGoodPackets(int value)
    {
        totalGoodPacketsCount.text = "Good Packets: " + value.ToString();
        StartCoroutine(PoPUPGoodPackets());
    }

    /// <summary>
    /// Displays health on health slider
    /// </summary>
    /// <param name="valueIn"></param>
    /// <param name="maxValue"></param>
    public void DisplayHealth(float valueIn,float maxValue)
    {
        float value = healthSlider.maxValue / maxValue;
        value = value * valueIn;
        healthSlider.value = value;
    }

    /// <summary>
    /// Displays antivirus status on slider
    /// </summary>
    /// <param name="valueIn"></param>
    /// <param name="maxValue"></param>
    public void DisplayAntiVirusBar(float valueIn,int maxValue,float duration)
    {
        float value = antiVirusSlider.maxValue / maxValue;
        value = value * valueIn;
        if (value == antiVirusSlider.maxValue)
        {
            StartCoroutine(DepleteAntivirusBar(value,duration));
        }
        else
        {
            antiVirusSlider.value = value;
        }
    }

    /// <summary>
    /// Slowly deplets the antivirus bar
    /// </summary>
    /// <param name="valueIn"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator DepleteAntivirusBar(float valueIn,float duration)
    {
        Debug.Log(valueIn);
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            antiVirusSlider.value = Mathf.Lerp(valueIn, antiVirusSlider.minValue, counter / duration);
            yield return null;
        }
    }

    IEnumerator PoPUPGoodPackets()
    {
        var popUp = pacaketPool.GetPacket(popUpGoodPacket);
        popUp.transform.position = popUpGoodPacket.transform.position;
        yield return new WaitForSeconds(2);
        popUp.SetActive(false);
        
    }

}
