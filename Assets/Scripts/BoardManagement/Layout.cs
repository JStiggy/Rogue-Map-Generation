using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Layout {
    public string name;

    public GameObject stairs;
    public List<GameObject> floorTiles;
    public List<GameObject> wallTiles;
    public List<GameObject> cornerTRTiles;
    public List<GameObject> cornerTLTiles;
    public List<GameObject> cornerBLTiles;
    public List<GameObject> cornerBRTiles;
    public List<FloorData> floorData;
    public int floorCount;

    public Layout(JToken token) {
        PopulateData((JArray)token["floorTiles"], out floorTiles);
        PopulateData((JArray)token["wallTiles"], out wallTiles);
        stairs = Resources.Load<GameObject>(token["stairs"].ToString());
        floorData = new List<FloorData>();
        foreach (JToken t in token["dungeonFloors"]) {
            string algo = t["algo"].ToString();
            List<FloorObj> item = new List<FloorObj>();
            List<FloorObj> enemy = new List<FloorObj>();
            foreach (JToken i in t["items"].Children()) {
                item.Add(new FloorObj(i["name"].ToString(), int.Parse(i["weight"].ToString())));
            }

            foreach (JToken i in t["enemies"].Children()) {
                enemy.Add(new FloorObj(i["name"].ToString(), int.Parse(i["weight"].ToString())));
            }
            floorData.Add(new FloorData(item, enemy, algo));
        }
    }

    public void PopulateData(JArray token, out List<GameObject> data) {
        data = new List<GameObject>();
        foreach (JToken t in token.Children()) {
            
            data.Add(Resources.Load<GameObject>(t.ToString()));
        }
    }

    public struct FloorData {
        public List<FloorObj> item;
        public List<FloorObj> enemy;
        string algo;

        public FloorData (List<FloorObj> i, List<FloorObj> e, string a) {
            item = i;
            enemy = e;
            algo = a;
        }
    }

    public struct FloorObj {
        public string name;
        public int weight;

        public FloorObj(string n, int w) {
            name = n;
            weight = w;
        }
    }
}