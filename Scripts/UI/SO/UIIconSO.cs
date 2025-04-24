using UnityEngine;

namespace Magia.UI
{
    [CreateAssetMenu(fileName = "UIIcons", menuName = "ScriptableObject/UI/Icons")]
    public class UIIconSO : ScriptableObject
    {
        public Sprite[] Sprites;
    }
}
