using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using static AStarSearch;

public class ClassicDungeon : Dungeons
{
    public List<RectangleRoom> roomList;

    public override void GenerateRooms() {
        roomList = new List<RectangleRoom>();
        int roomCount = Random.Range(6, 9);
        for (int i = 0; i < roomCount; ++i) {
            roomList.Add(new RectangleRoom(roomList));
        }
    }

    public override void GeneratePaths() {
        int totalPathCount = 0;
        for (int i = 0; i < roomList.Count; ++i) {
            totalPathCount += roomList[i].pathCount;
            roomAdjacencyList[i] = new List<int>();
        }

        for (int i = 0; i < roomList.Count; ++i) {
            for (int j = roomAdjacencyList[i].Count; j < roomList[i].pathCount; ++j) {
                int itr = 0;
                while (totalPathCount > 0 && itr < 25) {
                    itr++;
                    int pValue = Random.Range(0, roomList.Count);
                    if (i == pValue || roomAdjacencyList[pValue].Count >= roomList[pValue].pathCount || roomAdjacencyList[i].Contains(pValue)) {
                        continue;
                    } else {
                        roomAdjacencyList[i].Add(pValue);
                        roomAdjacencyList[pValue].Add(i);
                        totalPathCount -= 2;
                        break;
                    }
                }
            }
        }
    }

    public override bool checkLayout() {
        int count = 1;
        List<int> dValues = new List<int>();

        for (int i = 0; i < roomList.Count; ++i) {
            dValues.Add(-1);
        }

        dValues[0] = 0;

        Queue<int> rQueue = new Queue<int>();
        rQueue.Enqueue(0);

        while (rQueue.Count != 0) {
            int current = rQueue.Dequeue();
            foreach (int i in roomAdjacencyList[current]) {
                if (dValues[i] == -1) {
                    ++count;
                    rQueue.Enqueue(i);
                    dValues[i] = dValues[current] + 1;
                }
            }
        }
        return count == roomList.Count;
    }

    public override void BuildRooms() {
        for (int i = 0; i < roomList.Count; ++i) {
            for (int xPos = roomList[i].xPos; xPos <= roomList[i].xPos + roomList[i].width; ++xPos) {
                for (int yPos = roomList[i].yPos; yPos <= roomList[i].yPos + roomList[i].height; ++yPos) {
                    if (xPos == roomList[i].xPos || xPos == roomList[i].xPos + roomList[i].width ||
                        yPos == roomList[i].yPos || yPos == roomList[i].yPos + roomList[i].height) {
                        roomMap[xPos, yPos].tile = 1;
                    } else {
                        roomMap[xPos, yPos].tile = 2;
                    }
                }
            }
        }
    }

    public override void BuildPaths() {
        for (int i = 0; i < roomList.Count; ++i) {
            foreach (int j in roomAdjacencyList[i]) {
                int xStart = Random.Range(roomList[i].xPos + 1, roomList[i].xPos + roomList[i].width);
                int yStart = Random.Range(roomList[i].yPos + 1, roomList[i].yPos + roomList[i].height);
                int xDest = Random.Range(roomList[j].xPos + 1, roomList[j].xPos + roomList[j].width);
                int yDest = Random.Range(roomList[j].yPos + 1, roomList[j].yPos + roomList[j].height);

                int xPos = xStart;
                int yPos = yStart;

                while (xPos != xDest) {
                    roomMap[xPos, yPos].tile = 2;

                    if (roomMap[xPos, yPos - 1].tile == 0) {
                        roomMap[xPos, yPos - 1].tile = 1;
                    }

                    if (roomMap[xPos, yPos + 1].tile == 0) {
                        roomMap[xPos, yPos + 1].tile = 1;
                    }

                    if (xPos < xDest) {
                        ++xPos;
                    } else {
                        --xPos;
                    }

                }
                for (int t1 = -1; t1 < 2; ++t1) {
                    for (int t2 = -1; t2 < 2; ++t2) {
                        if (roomMap[xPos + t1, yPos + t2].tile == 0) {
                            roomMap[xPos + t1, yPos + t2].tile = 1;
                        }
                    }
                }

                while (yPos != yDest) {
                    roomMap[xPos, yPos].tile = 2;
                    if (roomMap[xPos - 1, yPos].tile == 0) {
                        roomMap[xPos - 1, yPos].tile = 1;
                    }
                    if (roomMap[xPos + 1, yPos].tile == 0) {
                        roomMap[xPos + 1, yPos].tile = 1;
                    }
                    if (yPos < yDest) {
                        ++yPos;
                    } else {
                        --yPos;
                    }
                }
                roomAdjacencyList[j].Remove(i);
            }
        }
    }

    public override void BuildFloor() {
        for (int i = 0; i < 80; ++i) {
            for (int j = 0; j < 80; ++j) {
                int tVal = roomMap[i, j].tile;
                if (tVal == 0) {
                    continue;
                } else if (tVal == 2) {
                    GameObject tmp = (GameObject)GameObject.Instantiate(data.floorTiles[Random.Range(0, data.floorTiles.Count)], new Vector3(i, 0, j), Quaternion.LookRotation(Vector3.down));
                    tmp.transform.SetParent(board);
                } else {
                    GameObject tmp = (GameObject)GameObject.Instantiate(data.wallTiles[Random.Range(0, data.wallTiles.Count)], new Vector3(i, 0, j), Quaternion.LookRotation(Vector3.forward));
                    tmp.transform.SetParent(board);
                }
            }
        }
    }

    public override Location AddLocation() {
        RectangleRoom rr = roomList[Random.Range(0, roomList.Count - 1)];
        return roomMap[rr.xPos + Random.Range(1, rr.width-1), rr.yPos + Random.Range(1, rr.height-1)];
    }

    public override void PlacePlayer() {
        Location loc = AddLocation();
        GameObject go = GameObject.Instantiate(BoardManager.i.unit, new Vector3(loc.x, 0, loc.y), Quaternion.identity);
        go.AddComponent<PlayerController>();
        BoardManager.i.actionQueue.Enqueue(go.GetComponent<Unit>());
        go.GetComponent<Unit>().x = loc.x;
        go.GetComponent<Unit>().y = loc.y;
        roomMap[loc.x, loc.y].unit = go.GetComponent<Unit>();
        roomMap[loc.x, loc.y].unit.name = "Jack Frost";
    }

    public override void PlaceEnemy() {
        JToken monsterData = BoardManager.i.GetMonster();
        Location loc = AddLocation();
        GameObject go = GameObject.Instantiate(BoardManager.i.unit, new Vector3(loc.x, 0, loc.y), Quaternion.identity);
        go.AddComponent<EnemyController>();
        BoardManager.i.actionQueue.Enqueue(go.GetComponent<Unit>());
        go.GetComponent<Unit>().x = loc.x;
        go.GetComponent<Unit>().y = loc.y;
        roomMap[loc.x, loc.y].unit = go.GetComponent<Unit>();
        roomMap[loc.x, loc.y].unit.name = monsterData["name"].ToString();
        roomMap[loc.x, loc.y].unit.MAX_HP = int.Parse(monsterData["max_hp"].ToString());
        roomMap[loc.x, loc.y].unit.MAX_MP = int.Parse(monsterData["max_mp"].ToString());
        roomMap[loc.x, loc.y].unit.HP = roomMap[loc.x, loc.y].unit.MAX_HP;
        roomMap[loc.x, loc.y].unit.MP = roomMap[loc.x, loc.y].unit.MAX_HP;
        roomMap[loc.x, loc.y].unit.STR = int.Parse(monsterData["str"].ToString());
        roomMap[loc.x, loc.y].unit.VIT = int.Parse(monsterData["vit"].ToString());
    }

    public override void PlaceItem() {

    }

    [System.Serializable]
    public class RectangleRoom : Room{
        public int height;                                                 //Vertical length of the room
        public int width;                                                  //Horizontal length of the room

        /*
        // Create a new room object, with random coordinates and dimensions
        //
        // @param: roomList, the current list of rooms in the dungeon
        // @effects: Create a new room object
        */
        public RectangleRoom(List<RectangleRoom> roomList) {
            while (true) {
                xPos = Random.Range(0, 61);
                yPos = Random.Range(0, 61);
                width = Random.Range(8, 18);
                height = Random.Range(5, 16);
                bool collision = false;

                for (int i = 0; i < roomList.Count; ++i) {
                    if (((Mathf.Abs(this.xPos - roomList[i].xPos) - 7) * 2 <= (this.width + ((RectangleRoom)roomList[i]).width)) &&
                        ((Mathf.Abs(this.yPos - roomList[i].yPos) - 7) * 2 <= (this.height + ((RectangleRoom)roomList[i]).height))) {
                        collision = true;
                        break;
                    }
                }
                if (!collision) {
                    int rVal = Random.Range(0, 100);
                    if (rVal > 90) {
                        this.pathCount = 3;
                    } else if (rVal > 20) {
                        this.pathCount = 2;
                    } else {
                        this.pathCount = 1;
                    }
                    break;
                }
            }
        }
    }
}
