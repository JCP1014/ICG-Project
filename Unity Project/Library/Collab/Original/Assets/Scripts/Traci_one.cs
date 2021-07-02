using System;
using System.Collections;
using System.Collections.Generic;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Types;
using UnityEngine;

using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CodingConnected.TraCI.NET.Commands;
using Color = UnityEngine.Color;
using Object = System.Object;

public class Traci_one : MonoBehaviour
{

    public Light ttLight;
    public GameObject tLight;
    public GameObject egoVehicle;
    public Material roadmat;
    public List<string> vehicleidlist;
    public List<GameObject> carlist;
    public GameObject NPCVehicle_1;
    public GameObject NPCVehicle_2;
    public GameObject NPCVehicle_3;
    public GameObject NPCVehicle_4;
    public GameObject NPCVehicle_5;
    public TraCIClient client;
    private List<string> tlightids;
    private int phaser;
    private List<traLights> listy;
    public Dictionary<string, List<traLights>> dicti;
    private float next_spawn_time;
    private float max_spawn_interval = 10.0f;
    private int leftClicks = 0;
    private float leftTimer = 0f;
    private int rightClicks = 0;
    private float rightTimer = 0f;
    private float mouseTimerLimit = 0.25f;
    public List<GameObject> humanList;
    public GameObject NPCHuman;
    public List<GameObject> otherList;
    public GameObject NPCOther;
    public List<GameObject> blockList;
    public GameObject NPCBlock;
    private System.Random rnd = new System.Random();
    private float humanStepOffset = 0.03f;
    private int humanStepThreshold = 500;
    private List<string> edgeList;
    private float keyTimeout = 0.5f;


    // Start is called before the first frame update

    void Start()
    {



        client = new TraCIClient();
        client.Connect("127.0.0.1", 4001); //connects to SUMO simulation

        tlightids = client.TrafficLight.GetIdList().Content; //all traffic light IDs in the simulation
        client.Gui.TrackVehicle("View #0", "0");
        client.Gui.SetZoom("View #0", 1200); //tracking the player vehicle



        createTLS();
        client.Control.SimStep();
        client.Control.SimStep();//making sure vehicle is loaded in

        client.Vehicle.SetSpeed("0", 0); //stops SUMO controlling player vehicle
        var shape = client.Vehicle.GetPosition("0").Content;
        var angle = client.Vehicle.GetAngle("0").Content;
        //puts the player vehicle in the starting position
        egoVehicle.transform.position = new Vector3((float)shape.X, 1.33f, (float)shape.Y);
        egoVehicle.transform.rotation = Quaternion.Euler(0, (float)angle, 0);


        carlist.Add(egoVehicle);


        // Randomly set next_spawn_time
        next_spawn_time = Time.time + UnityEngine.Random.Range(0.0f, max_spawn_interval);

        edgeList = client.Edge.GetIdList().Content;
    }

    private void OnApplicationQuit()
    {
        client.Control.Close();//terminates the connection upon ending of the scene
    }

    // Update is called once per frame
    private void FixedUpdate()
    {


        var newvehicles = client.Simulation.GetDepartedIDList("0").Content; //new vehicles this step
        var vehiclesleft = client.Simulation.GetArrivedIDList("0").Content; //vehicles that have left the simulation

        for (int i = 0; i < newvehicles.Count; i++)
        {
            if (newvehicles[i].Contains("other") || newvehicles[i].Contains("human") || newvehicles[i].Contains("block"))
            {
                newvehicles.RemoveAt(i);
            }
        }

        //if any vehicles have left the scene they are removed and destroyed
        for (int j = 0; j < vehiclesleft.Count; j++)
        {
            GameObject toremove = GameObject.Find(vehiclesleft[j]);

            if (toremove)
            {
                Debug.Log(toremove.name);
                if (toremove.name.Contains("human"))
                    humanList.Remove(toremove);
                else if (toremove.name.Contains("other"))
                    otherList.Remove(toremove);
                else if (toremove.name.Contains("block"))
                    blockList.Remove(toremove);
                else
                    carlist.Remove(toremove);
                Destroy(toremove);
            }

        }


        //road and lane client are on, necessary for manipulation in SUMO
        var road = client.Vehicle.GetRoadID(egoVehicle.name).Content;
        var lane = client.Vehicle.GetLaneIndex(egoVehicle.name).Content;
        /*
         * Updates the ego-vehicle's position in the SUMO scene
         * @params id: ego-vehicle's name in the SUMO simulation
         * @params road: current edge the vehicle is on in the SUMO simulation
         * @params lane: current lane number the ego vehicle is on
         * @params xPosition: X-axis position of the ego-vehicle in the Unity scene
         * @params yPosition: Z-axis position of the ego-vehicle in the Unity scene
         * @params angle: The angle that the ego vehicle is facing at
         * @params keepRoute: maps the ego-vehicle to the exact X-Y position in the SUMO simulation
         */
        client.Vehicle.MoveToXY("0", road, lane, (double)egoVehicle.transform.position.x,
            (double)egoVehicle.transform.position.z, (double)egoVehicle.transform.eulerAngles.y, 2);

        for (int carid = 1; carid < carlist.Count; carid++)
        {
            var carpos = client.Vehicle.GetPosition(carlist[carid].name).Content; //gets position of NPC vehicle
            carlist[carid].transform.position = new Vector3((float)carpos.X, carlist[carid].transform.position.y, (float)carpos.Y);
            var newangle = client.Vehicle.GetAngle(carlist[carid].name).Content; //gets angle of NPC vehicle
            carlist[carid].transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);
        }





        for (int i = 0; i < newvehicles.Count; i++)
        {
            var newcarposition = client.Vehicle.GetPosition(newvehicles[i]).Content; //gets position of new vehicle
            var num = rnd.Next(1, 6);
            switch (num)
            {
                case 1:
                    {
                        GameObject newcar = GameObject.Instantiate(NPCVehicle_1);
                        newcar.transform.position = new Vector3((float)newcarposition.X, 1.33f, (float)newcarposition.Y);//maps its initial position
                        var newangle = client.Vehicle.GetAngle(newvehicles[i]).Content;
                        newcar.transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);//maps initial angle
                        newcar.name = newvehicles[i];//object name the same as SUMO simulation version
                        carlist.Add(newcar);
                        break;
                    }
                case 2:
                    {
                        GameObject newcar = GameObject.Instantiate(NPCVehicle_2);
                        newcar.transform.position = new Vector3((float)newcarposition.X, 1.33f, (float)newcarposition.Y);//maps its initial position
                        var newangle = client.Vehicle.GetAngle(newvehicles[i]).Content;
                        newcar.transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);//maps initial angle
                        newcar.name = newvehicles[i];//object name the same as SUMO simulation version
                        carlist.Add(newcar);
                        break;
                    }
                case 3:
                    {
                        GameObject newcar = GameObject.Instantiate(NPCVehicle_3);
                        newcar.transform.position = new Vector3((float)newcarposition.X, 1.33f, (float)newcarposition.Y);//maps its initial position
                        var newangle = client.Vehicle.GetAngle(newvehicles[i]).Content;
                        newcar.transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);//maps initial angle
                        newcar.name = newvehicles[i];//object name the same as SUMO simulation version
                        carlist.Add(newcar);
                        break;
                    }
                case 4:
                    {
                        GameObject newcar = GameObject.Instantiate(NPCVehicle_4);
                        newcar.transform.position = new Vector3((float)newcarposition.X, 1.33f, (float)newcarposition.Y);//maps its initial position
                        var newangle = client.Vehicle.GetAngle(newvehicles[i]).Content;
                        newcar.transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);//maps initial angle
                        newcar.name = newvehicles[i];//object name the same as SUMO simulation version
                        carlist.Add(newcar);
                        break;
                    }
                case 5:
                    {
                        GameObject newcar = GameObject.Instantiate(NPCVehicle_5);
                        newcar.transform.position = new Vector3((float)newcarposition.X, 1.33f, (float)newcarposition.Y);//maps its initial position
                        var newangle = client.Vehicle.GetAngle(newvehicles[i]).Content;
                        newcar.transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);//maps initial angle
                        newcar.name = newvehicles[i];//object name the same as SUMO simulation version
                        carlist.Add(newcar);
                        break;
                    }
            }
        }

        var currentphase = client.TrafficLight.GetCurrentPhase("42443658");
        client.Control.SimStep();
        //checks traffic light's phase to see if it has changed
        if (client.TrafficLight.GetCurrentPhase("42443658").Content != currentphase.Content)
        {
            changeTrafficLights();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            generateSpecialCar();
        }
        if (Input.GetKey(KeyCode.W))
        {
            generateHuman();
        }
        if (Input.GetKey(KeyCode.E))
        {
            generateBlock();
        }


        //// Detect left click
        //if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
        //{
        //    leftClicks++;
        //    Debug.Log("Num of left clicks ->" + leftClicks);
        //}
        //if (leftClicks >= 1 && leftClicks < 3)
        //{
        //    leftTimer += Time.fixedDeltaTime;

        //    if (leftClicks == 2)
        //    {
        //        if (leftTimer - mouseTimerLimit < 0)
        //        {
        //            Debug.Log("Left Double Click");
        //            leftTimer = 0;
        //            leftClicks = 0;
        //            generateBlock();
        //        }
        //        else
        //        {
        //            Debug.Log("Left Single Click");
        //            leftClicks = 0;
        //            leftTimer = 0;
        //            generateSpecialCar();
        //        }
        //    }

        //    if (leftTimer > mouseTimerLimit)
        //    {
        //        Debug.Log("Left Single Click");
        //        leftClicks = 0;
        //        leftTimer = 0;
        //        generateSpecialCar();
        //    }
        //}

        //// Detect right click
        //if (Input.GetMouseButtonDown(1) && GUIUtility.hotControl == 0)
        //{
        //    rightClicks++;
        //    Debug.Log("Num of right clicks ->" + rightClicks);
        //}
        //if (rightClicks >= 1 && rightClicks < 3)
        //{
        //    rightTimer += Time.fixedDeltaTime;

        //    if (rightClicks == 2)
        //    {
        //        if (rightTimer - mouseTimerLimit < 0)
        //        {
        //            Debug.Log("Right Double Click");
        //            rightTimer = 0;
        //            rightClicks = 0;
        //        }
        //        else
        //        {
        //            Debug.Log("Right Single Click");
        //            rightClicks = 0;
        //            rightTimer = 0;
        //            generateHuman();
        //        }
        //    }

        //    if (rightTimer > mouseTimerLimit)
        //    {
        //        Debug.Log("Right Single Click");
        //        rightClicks = 0;
        //        rightTimer = 0;
        //        generateHuman();
        //    }
        //}


        // Update special cars
        for (int i = 0; i < otherList.Count; i++)
        {
            var newPos = client.Vehicle.GetPosition(otherList[i].name).Content; //gets position of new vehicle
            otherList[i].transform.position = new Vector3((float)newPos.X, otherList[i].transform.position.y,
                    (float)newPos.Y);//maps its initial position
            var newangle = client.Vehicle.GetAngle(otherList[i].name).Content;
            otherList[i].transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);//maps initial angle
        }


        // Update humans
        for (int i = 0; i < humanList.Count; i++)
        {
            var h = humanList[i].GetComponent<HumanAttribute>();
            humanList[i].transform.position += humanList[i].transform.forward * humanStepOffset;
            h.stepCount += 1;
            try
            {
                client.Vehicle.MoveToXY(humanList[i].name, road, lane, (double)humanList[i].transform.position.x,
                    (double)humanList[i].transform.position.z, (double)humanList[i].transform.eulerAngles.y, 2);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (h.stepCount == humanStepThreshold)
            {
                h.stepCount = 0;
                var num = rnd.Next(0, 8);
                switch (num)
                {
                    case 0:
                        h.transform.forward = new Vector3(1, 0, 0);
                        break;
                    case 1:
                        h.transform.forward = new Vector3(1, 0, 1);
                        break;
                    case 2:
                        h.transform.forward = new Vector3(0, 0, 1);
                        break;
                    case 3:
                        h.transform.forward = new Vector3(-1, 0, 1);
                        break;
                    case 4:
                        h.transform.forward = new Vector3(-1, 0, 0);
                        break;
                    case 5:
                        h.transform.forward = new Vector3(-1, 0, -1);
                        break;
                    case 6:
                        h.transform.forward = new Vector3(0, 0, -1);
                        break;
                    case 7:
                        h.transform.forward = new Vector3(1, 0, -1);
                        break;
                }
            }
        }


        // Update blocks
        for (int i = 0; i < blockList.Count; i++)
        {
            try
            {
                client.Vehicle.MoveToXY(blockList[i].name, road, lane, (double)blockList[i].transform.position.x,
                    (double)blockList[i].transform.position.z, (double)blockList[i].transform.eulerAngles.y, 2);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }


    }


    // Generate other types of vehicels at the position clicked
    void generateSpecialCar()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5.0f), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 cameraPoint = Camera.main.transform.position;
        Vector3 direction = mousePoint - cameraPoint;
        double alpha = cameraPoint.y / direction.y;

        Vector3 newPoint = new Vector3((float)(cameraPoint.x - alpha * direction.x), (float)0.5, (float)(cameraPoint.z - alpha * direction.z));


        var newOtherID = "other_" + Convert.ToString(otherList.Count);
        GameObject newOther = GameObject.Instantiate(NPCOther); //creates the vehicle GameObject

        newOther.transform.position = newPoint;
        // newOther.transform.position = worldPoint;//maps its initial position
        newOther.name = newOtherID;//object name the same as SUMO simulation version
        otherList.Add(newOther);

        client.Vehicle.Add(newOtherID, "typeB", client.Route.GetIdList().Content[0], 0, 20.0f, 0, (byte)0);
        client.Vehicle.ChangeTarget(newOtherID, edgeList[rnd.Next(0, edgeList.Count)]);
        var road = client.Vehicle.GetRoadID(newOther.name).Content;
        var lane = client.Vehicle.GetLaneIndex(newOther.name).Content;
        try
        {
            client.Vehicle.MoveToXY(newOther.name, road, lane, (double)newOther.transform.position.x,
                (double)newOther.transform.position.z, (double)newOther.transform.eulerAngles.y, 2);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }


    // Generate human at the position clicked
    void generateHuman()
    {
        // Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5.0f), Camera.MonoOrStereoscopicEye.Mono);

        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5.0f), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 cameraPoint = Camera.main.transform.position;
        Vector3 direction = mousePoint - cameraPoint;
        double alpha = cameraPoint.y / direction.y;

        Vector3 newPoint = new Vector3((float)(cameraPoint.x - alpha * direction.x), (float)0.5, (float)(cameraPoint.z - alpha * direction.z));
        var newHumanID = "human_" + Convert.ToString(humanList.Count);
        GameObject newHuman = GameObject.Instantiate(NPCHuman); //creates the vehicle GameObject
        newHuman.transform.position = newPoint;//maps its initial position
        newHuman.name = newHumanID;//object name the same as SUMO simulation version
        humanList.Add(newHuman);

        client.Vehicle.Add(newHumanID, "human", client.Route.GetIdList().Content[0], 0, 20.0f, 0, (byte)0);
        var road = client.Vehicle.GetRoadID(newHuman.name).Content;
        var lane = client.Vehicle.GetLaneIndex(newHuman.name).Content;
        try
        {
            client.Vehicle.MoveToXY(newHuman.name, road, lane, (double)newHuman.transform.position.x,
                (double)newHuman.transform.position.z, (double)newHuman.transform.eulerAngles.y, 2);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    // Generate block at the position clicked
    void generateBlock()
    {
        // Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5.0f), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5.0f), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 cameraPoint = Camera.main.transform.position;
        Vector3 direction = mousePoint - cameraPoint;
        double alpha = cameraPoint.y / direction.y;

        Vector3 newPoint = new Vector3((float)(cameraPoint.x - alpha * direction.x), (float)0.5, (float)(cameraPoint.z - alpha * direction.z));

        var newBlockID = "block_" + Convert.ToString(blockList.Count);
        GameObject newBlock = GameObject.Instantiate(NPCBlock); //creates the vehicle GameObject
        newBlock.transform.position = newPoint;//maps its initial position
        newBlock.name = newBlockID;//object name the same as SUMO simulation version
        blockList.Add(newBlock);

        client.Vehicle.Add(newBlockID, "block", client.Route.GetIdList().Content[0], 0, 20.0f, 0, (byte)0);
        client.Vehicle.SetSpeed(newBlockID, 0);
        var road = client.Vehicle.GetRoadID(newBlock.name).Content;
        var lane = client.Vehicle.GetLaneIndex(newBlock.name).Content;
        try
        {
            client.Vehicle.MoveToXY(newBlock.name, road, lane, (double)newBlock.transform.position.x,
                (double)newBlock.transform.position.z, (double)newBlock.transform.eulerAngles.y, 2);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    //Changes traffic lights to their next phase
    void changeTrafficLights()
    {
        for (int i = 0; i < tlightids.Count; i++)
        {
            //for each traffic light value of a junctions name
            for (int k = 0; k < dicti[tlightids[i]].Count; k++)
            {

                var newstate = client.TrafficLight.GetState(tlightids[i]).Content;
                var lightchange = dicti[tlightids[i]][k]; //retrieves traffic light object from list

                var chartochange = newstate[lightchange.index].ToString();//traffic lights new state based on its index
                if (lightchange.isdual == false)
                {
                    lightchange.changeState(chartochange.ToLower());//single traffic light change
                }
                else
                {
                    lightchange.changeStateDual(chartochange.ToLower());//dual traffic light change
                }

            }
        }

    }


    // Creates the TLS for of all junctions in the SUMO simulation

    void createTLS()
    {
        dicti = new Dictionary<string, List<traLights>>(); //the dictionary to hold each junctions traffic lights
        for (int ids = 0; ids < tlightids.Count; ids++)
        {
            List<traLights> traLightslist = new List<traLights>();
            int numconnections = 0;  //The index that represents the traffic light's state value
            var newjunction = GameObject.Find(tlightids[ids]); //the traffic light junction
            for (int i = 0; i < newjunction.transform.childCount; i++)
            {
                bool isdouble = false;
                var trafficlight = newjunction.transform.GetChild(i);//the next traffic light in the junction
                //Checks if the traffic light has more than 3 lights
                if (trafficlight.childCount > 3)
                {
                    isdouble = true;
                }
                Light[] newlights = trafficlight.GetComponentsInChildren<Light>();//list of light objects belonging to
                                                                                  //the traffic light
                                                                                  //Creation of the traffic light object, with its junction name, list of lights, index in the junction
                                                                                  //and if it is a single or dual traffic light
                traLights newtraLights = new traLights(newjunction.name, newlights, numconnections, isdouble);
                traLightslist.Add(newtraLights);
                var linkcount = client.TrafficLight.GetControlledLinks(newjunction.name).Content.NumberOfSignals;
                var laneconnections = client.TrafficLight.GetControlledLinks(newjunction.name).Content.Links;
                if (numconnections + 1 < linkcount - 1)
                {
                    numconnections++;//index increases
                    //increases index value until the next lane is reached
                    while ((laneconnections[numconnections][0] == laneconnections[numconnections - 1][0] || isdouble) &&
                           numconnections < linkcount - 1)
                    {
                        //if the next lane is reached but the traffic light is a dual lane, continue until the
                        //lane after is reached
                        if (laneconnections[numconnections][0] != laneconnections[numconnections - 1][0] && isdouble)
                        {
                            isdouble = false;
                        }
                        numconnections++;
                    }
                }
            }
            dicti.Add(newjunction.name, traLightslist);
        }
        changeTrafficLights(); //displays the initial state of all traffic lights
        print(550.4 + +0.54 + 776.4);
    }









}
