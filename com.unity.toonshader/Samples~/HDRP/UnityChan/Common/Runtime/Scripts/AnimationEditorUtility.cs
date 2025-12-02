#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityChan {

public static class AnimationEditorUtility {
    
    public static List<string> FindStateNames(Animator animator, int layerIndex) {
        
        //fill in body state names from animator controller
        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        AnimatorController controller = rac as AnimatorController;

        // Handle AnimatorOverrideController
        if (controller == null && rac is AnimatorOverrideController overrideController) {
            controller = overrideController.runtimeAnimatorController as AnimatorController;
        }

        List<string> stateNames = new List<string>();
        if (controller == null || controller.layers.Length <= 0)
            return stateNames;

        Assert.IsTrue(layerIndex >= 0 && layerIndex < controller.layers.Length);
        ChildAnimatorState[] states = controller.layers[layerIndex].stateMachine.states;
        foreach (ChildAnimatorState state in states) {
            stateNames.Add(state.state.name);
        }

        return stateNames;
    }

}

}

#endif //UNITY_EDITOR