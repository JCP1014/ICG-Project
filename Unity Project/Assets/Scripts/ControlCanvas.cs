using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCanvas : MonoBehaviour
{
    public GameObject DashboardCanvas;
    public GameObject RearviewCanvas;
    public GameObject InfoCanvas;
    private bool isDashboard = false;
    private bool isRearview = false;
    private bool isInfo = false;

    // Start is called before the first frame update
    void Start()
    {
        DashboardCanvas.SetActive(isDashboard);
        RearviewCanvas.SetActive(isRearview);
        InfoCanvas.SetActive(isInfo);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Debug.Log("Alpha2");
            isDashboard = !isDashboard;
            DashboardCanvas.SetActive(isDashboard);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Debug.Log("Alpha3");
            isRearview = !isRearview;
            RearviewCanvas.SetActive(isRearview);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            Debug.Log("Alpha4");
            isInfo= !isInfo;
            InfoCanvas.SetActive(isInfo);
        }
    }
}
