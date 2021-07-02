using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDashboard : MonoBehaviour
{
    private Traci_one t;
    public TMP_Text textSpeed;
    public TMP_Text textDistance;
    

    private void Start()
    {
        t = GameObject.Find("GameObject").GetComponent<Traci_one>();
    }

    // Update is called once per frame
    private void Update()
    {
        double speed = GameObject.FindWithTag("Player").GetComponent<WheelDrive>().speed;
        double distance = t.client.Vehicle.GetDistance("0").Content;
        textSpeed.text = string.Format("{0:0.0}", speed);
        textDistance.text = string.Format("{0:0.0}", distance);
    }
}
