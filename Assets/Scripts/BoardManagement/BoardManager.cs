using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static AStarSearch;

public class BoardManager : MonoBehaviour {
    public static BoardManager i;
    public static JObject dungeonData;
    public static JObject monsterData;
    public string dungeonName = "TutorialDungeon";
    public GameObject unit;

    private Transform boardHolder;
    public  Dungeons dungeonFloor;

    int currentFloor = 0;

    public Queue<Unit> actionQueue;

    public void Start() {
        i = this;
        using (StreamReader file = File.OpenText(Application.dataPath + "/StreamingAssets/Data/floorData.json"))
        using (JsonTextReader reader = new JsonTextReader(file)) {
            dungeonData = (JObject)JToken.ReadFrom(reader);
        }
        using (StreamReader file = File.OpenText(Application.dataPath + "/StreamingAssets/Data/monsterData.json"))
        using (JsonTextReader reader = new JsonTextReader(file)) {
            monsterData = (JObject)JToken.ReadFrom(reader);
        }
        SetupScene();

    }

    public void BoardSetup() {

        if (boardHolder != null) {
            Destroy(boardHolder.gameObject);
        }

        while (true) {
            dungeonFloor = (Dungeons)Activator.CreateInstance(Type.GetType(dungeonData[dungeonName]["dungeonFloors"][currentFloor]["algo"].ToString()));
            dungeonFloor.data = new Layout(dungeonData[dungeonName]);

            dungeonFloor.roomMap = new Location[100, 100];
            for (int i = 0; i < 100; ++i) {
                for (int j = 0; j < 100; ++j) {
                    dungeonFloor.roomMap[i, j] = new Location(i, j, null, 0);
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

        actionQueue = new Queue<Unit>();

        dungeonFloor.BuildRooms();

        dungeonFloor.BuildPaths();

        dungeonFloor.BuildFloor();

        dungeonFloor.PlacePlayer();

        for(int i =0; i < 10; ++ i)
            dungeonFloor.PlaceEnemy();
    }

    public JToken GetMonster() {
        int val = Random.Range(0, 999);
        int loc = 0;
        while (val > int.Parse(dungeonData[dungeonName]["dungeonFloors"][currentFloor]["enemies"][loc]["weight"].ToString())) {
            val -= int.Parse(dungeonData[dungeonName]["dungeonFloors"][currentFloor]["enemies"][loc]["weight"].ToString());
            loc++;
        }
        print(dungeonData[dungeonName]["dungeonFloors"][currentFloor]["enemies"][loc]["name"].ToString());
        return monsterData[dungeonData[dungeonName]["dungeonFloors"][currentFloor]["enemies"][loc]["name"].ToString()];
    }

    public void CloseMenu() {
        actionQueue.Peek().GetComponent<PlayerController>().active = false;
    }

    public void PopQueue() {
        actionQueue.Enqueue(actionQueue.Dequeue());
    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene() {
        //Creates the outer walls and floor.
        currentFloor++;
        BoardSetup();
    }

    public bool IsLegalTile(int x, int y) {
        return 2 == dungeonFloor.roomMap[x, y].tile && null == dungeonFloor.roomMap[x, y].unit;
    }
}