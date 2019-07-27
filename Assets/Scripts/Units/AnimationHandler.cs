using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AStarSearch;

public class AnimationHandler : MonoBehaviour
{

    Animator anim;
    public List<GameObject> currentAnims = new List<GameObject>();
    public int finished_anims;

    // Update is called once per frame
    void Start() {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (finished_anims == currentAnims.Count && currentAnims.Count > 0) {
            BoardManager.i.PopQueue();
            finished_anims = 0;
            foreach (GameObject go in currentAnims) {
                Destroy(go);
            }
            currentAnims.Clear();
            GetComponent<PlayerController>().active = false;
        }
    }

    public void PerformAttack(int attackAnim, string animName, Location[] locs) {
        anim.SetTrigger(animName);
        foreach (Location l in locs) {
            GameObject go = Instantiate(AnimationContainer.i.animations[attackAnim], new Vector3(l.x, 0, l.y), Quaternion.identity);
            currentAnims.Add(go);
        }
    }
}
