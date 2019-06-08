using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Unit unit;
    public bool active = false;
    // Start is called before the first frame update
    void Start() {
        unit = gameObject.GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update() {
        if (BoardManager.i.actionQueue.Peek() == unit) {
           
        }
    }

    private IEnumerator PerformMovement(int x, int y) {
        active = true;
        if (BoardManager.i.IsLegalTile(x + unit.x, y + unit.y)) {
            Vector3 start = new Vector3(unit.x, 0, unit.y);
            BoardManager.i.dungeonFloor.roomMap[unit.x, unit.y].unit = null;
            unit.x += x; unit.y += y;
            Vector3 dest = new Vector3(unit.x, 0, unit.y);
            BoardManager.i.dungeonFloor.roomMap[unit.x, unit.y].unit = unit;
            float t = 0;
            do {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, dest, t * 2);
                yield return null;
            } while (t <= .5f);
            transform.position = dest;
        }
        active = false;
    }
}
