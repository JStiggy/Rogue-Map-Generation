using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AStarSearch;

public abstract class Dungeons {
    public Transform board;
    public Layout data;
    public Dictionary<int, List<int>> roomAdjacencyList;

    public Location[,] roomMap;

    public abstract void GenerateRooms();

    public abstract void GeneratePaths();

    public abstract bool checkLayout();

    public abstract void BuildRooms();

    public abstract void BuildPaths();

    public abstract void BuildFloor();

    public abstract void PlacePlayer();

    public abstract void PlaceItem();

    public abstract void PlaceEnemy();

    public abstract Location AddLocation();

    public class Room {
        public int xPos;                                                   //X Position for bottom left hand corner
        public int yPos;                                                   //Y Position for bottom left hand corner
        public int pathCount;                                              //Number of connections allowed to other rooms
    }
}

