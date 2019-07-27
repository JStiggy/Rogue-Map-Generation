using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (AnimationHandler ah in GameObject.FindObjectsOfType<AnimationHandler>()) {
            if (ah.currentAnims.Count != 0) {
                ah.finished_anims++;
            }
        }
    }
}
