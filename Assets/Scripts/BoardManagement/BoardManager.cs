using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static AStarSearch;

public class BoardManager : MonoBehaviour {

    public static JObject dungeonData;
    public string dungeonName = "TutorialDungeon";

    private Transform boardHolder;
    private Dungeons dungeonFloor;

    int currentFloor = 0;

    public void Start() {
        using (StreamReader file = File.OpenText(Application.dataPath + "/Data/floorData.json"))
        using (JsonTextReader reader = new JsonTextReader(file)) {
            dungeonData = (JObject)JToken.ReadFrom(reader);
        }
        SetupScene();

    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            Location end = dungeonFloor.AddLocation();
            Location current = dungeonFloor.AddLocation();
            AStarSearch ass = new AStarSearch(dungeonFloor.roomMap, current, end);
            int axe = 0;
            while (current.x != end.x || current.y != end.y) {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.transform.position = new Vector3(end.x, 0, end.y);
                end = ass.cameFrom[end];
                axe += 1;
                if (axe >= 55) break;
            }
        }
    }

    public void BoardSetup() {

        if (boardHolder != null) {
            Destroy(boardHolder.gameObject);
        }

        if (dungeonData == null) print("yikes");

        while (true) {
            dungeonFloor = (Dungeons)Activator.CreateInstance(Type.GetType(dungeonData[dungeonName]["dungeonFloors"][currentFloor]["algo"].ToString()));
            dungeonFloor.data = new Layout(dungeonData[dungeonName]);

            dungeonFloor.roomMap = new int[100, 100];
            for (int i = 0; i < 100; ++i) {
                for (int j = 0; j < 100; ++j) {
                    dungeonFloor.roomMap[i, j] = 0;
                }
            }

            dungeonFloor.GenerateRooms();

            if (dungeonFloor.roomAdjacencyList == null) {
                dungeonFloor.roomAdjacencyList = new Dictionary<int, List<int>>();
            } else {
                dungeonFloor.roomAdjacencyList.Clear();
            }

            dungeonFloor.GeneratePaths();

            if (dungeonFloor.checkLayout()) {
                break;
            }
        }

        //Board is legal for play
        dungeonFloor.board = new GameObject("Board").transform;

        dungeonFloor.BuildRooms();

        dungeonFloor.BuildPaths();

        dungeonFloor.BuildFloor();
    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene() {
        //Creates the outer walls and floor.
        currentFloor++;
        BoardSetup();
    }
}