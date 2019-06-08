using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch {
    public Dictionary<Location, Location> cameFrom
        = new Dictionary<Location, Location>();
    public Dictionary<Location, double> costSoFar
        = new Dictionary<Location, double>();

    // Note: a generic version of A* would abstract over Location and
    // also Heuristic
    static public double Heuristic(Location a, Location b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public AStarSearch(int[,] map, Location start, Location goal) {
        var frontier = new PriorityQueue<Location>();

        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0) {
            Location current = frontier.Dequeue();
            if (current.Equals(goal)) {
                break;
            }

            foreach (Location next in Neighbors(current, map)) {
                double newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next)
                    || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
    }

    public Location[] Neighbors(Location loc, int[,] map) {
        List<Location> locs = new List<Location>();
        try {
            if (map[loc.x + 1, loc.y] == 2) {
                locs.Add(BoardManager.i.dungeonFloor.roomMap[loc.x + 1, loc.y]);
            }
            if (map[loc.x - 1, loc.y] == 2) {
                locs.Add(BoardManager.i.dungeonFloor.roomMap[loc.x - 1, loc.y]);
            }
            if (map[loc.x, loc.y + 1] == 2) {
                locs.Add(BoardManager.i.dungeonFloor.roomMap[loc.x, loc.y + 1]);
            }
            if (map[loc.x, loc.y - 1] == 2) {
                locs.Add(BoardManager.i.dungeonFloor.roomMap[loc.x, loc.y - 1]);
            }
        } catch (IndexOutOfRangeException e) { }

        return locs.ToArray();
    }

    public class PriorityQueue<T> {
        private List<Tuple<T, double>> elements = new List<Tuple<T, double>>();

        public int Count {
            get { return elements.Count; }
        }

        public void Enqueue(T item, double priority) {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue() {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++) {
                if (elements[i].Item2 < elements[bestIndex].Item2) {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

    public struct Location {
        // Implementation notes: I am using the default Equals but it can
        // be slow. You'll probably want to override both Equals and
        // GetHashCode in a real project.

        public int tile;
        public readonly int x, y;
        public Unit unit;
        //public Item item;
        //public Interactable interactable;
        public Location(int x, int y, Unit u, int t) {
            this.x = x;
            this.y = y;
            this.unit = u;
            this.tile = t;
        }
    }
}