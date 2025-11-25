using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChan {

public class AnimationButtonUISelector {

    public void AddButton(Button button, int stateHash, int animationIndex) {

        m_bodyAnimationButtons.Add(button);
        m_animatorStateHash2Index[stateHash] = animationIndex;
    }

    public void Select(int stateHash) {
        if (!m_animatorStateHash2Index.TryGetValue(stateHash, out int animationIndex)) {
            return;
        }

        if (animationIndex == m_selectedButtonIndex) {
            return;
        }

        int newIndex = m_animatorStateHash2Index[stateHash];
        if (m_selectedButtonIndex != INVALID_BUTTON_INDEX)
            m_bodyAnimationButtons[m_selectedButtonIndex].RemoveFromClassList("selected");
        m_selectedButtonIndex = newIndex;
        m_bodyAnimationButtons[m_selectedButtonIndex].AddToClassList("selected");
    }
    
//----------------------------------------------------------------------------------------------------------------------

    private readonly List<Button> m_bodyAnimationButtons = new List<Button>();
    private readonly Dictionary<int, int> m_animatorStateHash2Index = new Dictionary<int, int>();

    private int m_selectedButtonIndex = INVALID_BUTTON_INDEX;

    const int INVALID_BUTTON_INDEX = -1;

}

} //end namespace