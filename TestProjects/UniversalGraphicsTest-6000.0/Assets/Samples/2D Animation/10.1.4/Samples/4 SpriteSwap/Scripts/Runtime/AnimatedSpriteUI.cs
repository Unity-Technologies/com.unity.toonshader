using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    internal class AnimatedSpriteUI : MonoBehaviour
    {
        [SerializeField]
        Sprite m_OpenPalmSprite;

        [SerializeField]
        Sprite m_ThumbsUpSprite;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            var label = uiDocument.rootVisualElement.Q<Label>();
            label.text = "These are different characters that share a single Animation clip.\nThis is done by animating Sprite Resolver values.\nThe hand animation is a swap between two Sprites.";

            var openPalmImage = uiDocument.rootVisualElement.Q<Image>("OpenPalmImage");
            openPalmImage.sprite = m_OpenPalmSprite;

            var thumbsUpImage = uiDocument.rootVisualElement.Q<Image>("ThumbsUpImage");
            thumbsUpImage.sprite = m_ThumbsUpSprite;
        }
    }
}
