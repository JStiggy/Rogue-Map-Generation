using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    Unit unit;
    AnimationHandler animHandler;
    Vector3 facingDirection = Vector3.forward;

    public bool active = false;
    private GameObject statusPanel;
    // Start is called before the first frame update
    void Start()
    {
        unit = gameObject.GetComponent<Unit>();
        statusPanel = GameObject.Find("PlayerData");
        animHandler = gameObject.GetComponent<AnimationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatus();

        if (BoardManager.i.actionQueue.Peek() == unit) {
            Vector2 movement = new Vector2(ControllerManager.inputs["Horizontal"].down, ControllerManager.inputs["Vertical"].down);
            if ((movement.x == 0 || movement.y == 0) && ControllerManager.inputs["Diagnol"].down == 1 && !active) {
                return;
            } else if ((movement.x != 0 || movement.y != 0) && !active) {
                StartCoroutine(PerformMovement((int)movement.x, (int)movement.y));
                return;
            }
            if(ControllerManager.inputs["Menu"].downLastFrame == 1 && !active) {
                OpenMenu();
                return;
            }
            if (ControllerManager.inputs["Submit"].downLastFrame == 1 && !active) {
                active = true;
                AStarSearch.Location[] locs = new AStarSearch.Location[1];
                locs[0] = BoardManager.i.dungeonFloor.roomMap[(int)(unit.x + facingDirection.x), (int)(unit.y + facingDirection.y)];
                animHandler.PerformAttack(0,"Attack",locs);
                return;
            }
        }
    }

    private void UpdateStatus () {
        statusPanel.GetComponentsInChildren<Text>()[0].text = unit.name;
        statusPanel.GetComponentsInChildren<Text>()[1].text = "HP\n" + unit.HP + "/" + unit.MAX_HP;
        statusPanel.GetComponentsInChildren<Text>()[2].text = "MP\n" + unit.MP + "/" + unit.MAX_MP;
    }

    private void OpenMenu() {
        GameObject.FindObjectOfType<MenuManager>().menu.SetActive(true);
        GameObject.FindObjectOfType<EventSystem>().SetSelectedGameObject(GameObject.Find("ItemMenuButton"));
        active = true;
    }

    private IEnumerator PerformMovement(int x, int y) {
        active = true;
        if(BoardManager.i.IsLegalTile(x + unit.x, y + unit.y)){
            Vector3 start = new Vector3(unit.x, 0, unit.y);
            BoardManager.i.dungeonFloor.roomMap[unit.x, unit.y].unit = null;
            unit.x += x; unit.y += y;
            Vector3 dest = new Vector3(unit.x, 0, unit.y);
            facingDirection = dest - start;
            BoardManager.i.dungeonFloor.roomMap[unit.x, unit.y].unit = unit;
            float t = 0;
            do {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, dest, t*2);
                yield return null;
            } while (t <= .5f);
            transform.position = dest;
            active = false;
            BoardManager.i.PopQueue();
        }
        active = false;

    }
}
