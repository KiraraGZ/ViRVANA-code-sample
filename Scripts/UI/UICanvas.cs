using Magia.GameLogic;
using UnityEngine;

namespace Magia.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] protected Canvas canvas;

        protected virtual void Start()
        {
            canvas.worldCamera = GameplayController.Instance.CameraHandler.UICamera;
        }
    }
}
