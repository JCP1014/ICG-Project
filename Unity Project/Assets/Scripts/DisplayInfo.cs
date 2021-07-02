using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DisplayInfo : MonoBehaviour
{
    private Traci_one t;
    public TMP_Text textPos;
    public TMP_Text textAngle;
    public TMP_Text textEdge;
    public TMP_Text textLane;
    public TMP_Text textFuel;
    public TMP_Text textLimit;
    public TMP_Text textTime;
    public TMP_Text textNum;
    public TMP_Text textArrived;
    public TMP_Text textCO;
    public TMP_Text textCO2;
    public TMP_Text textHC;
    public TMP_Text textPMx;
    public TMP_Text textNOx;

    // Start is called before the first frame update
    void Start()
    {
        t = GameObject.Find("GameObject").GetComponent<Traci_one>();
    }

    // Update is called once per frame
    void Update()
    {
        var carNum = t.client.Vehicle.GetIdCount().Content;
        var pos_x = t.client.Vehicle.GetPosition("0").Content.X;
        var pos_y = t.client.Vehicle.GetPosition("0").Content.Y;
        var angle = t.client.Vehicle.GetAngle("0").Content;
        var edge = t.client.Vehicle.GetRoadID("0").Content;
        var lane = t.client.Vehicle.GetLaneID("0").Content;
        var laneIndex = t.client.Vehicle.GetLaneIndex("0").Content;
        var co2 = t.client.Vehicle.GetCO2Emission("0").Content;
        var co = t.client.Vehicle.GetCOEmission("0").Content;
        var hc = t.client.Vehicle.GetHCEmission("0").Content;
        var pmx = t.client.Vehicle.GetPMxEmission("0").Content;
        var nox = t.client.Vehicle.GetNOxEmission("0").Content;
        var fuel = t.client.Vehicle.GetFuelConsumption("0").Content;
        var limit = t.client.Lane.GetMaxSpeed(lane).Content;
        var currentTime = t.client.Simulation.GetTime("0").Content;
        var arrivedNum = t.client.Simulation.GetArrivedNumber("0").Content;

        carNum = carNum - t.humanList.Count - t.blockList.Count;

        textPos.text = string.Format("({0:0.00, 1:0.00})", pos_x, pos_y);
        textAngle.text = string.Format("{0:0.00}", angle);
        textEdge.text = edge;
        textLane.text = string.Format("{0:0}", laneIndex);
        textCO2.text = string.Format("{0:0.00}", co2);
        textCO.text = string.Format("{0:0.00}", co);
        textHC.text = string.Format("{0:0.00}", hc);
        textPMx.text = string.Format("{0:0.00}", pmx);
        textNOx.text = string.Format("{0:0.00}", nox);
        textFuel.text = string.Format("{0:0.00}", fuel);
        textLimit.text = string.Format("{0:0.00}", limit);
        textTime.text = string.Format("{0:0}", currentTime);
        textNum.text = string.Format("{0:0}", carNum);
        textArrived.text = string.Format("{0:0}", arrivedNum);
    }
}
