using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class GetIP : MonoBehaviour
{
    public TextMeshProUGUI localIPText;

    private void Start()
    {
        GetLocalIPAddress();
    }
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIPText.text = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
