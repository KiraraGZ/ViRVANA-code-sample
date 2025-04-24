using UnityEngine;

namespace Magia.UI.Gameplay.Crosshair
{
    public abstract class UICrosshair : MonoBehaviour
    {
        public virtual void Initialize(int _index)
        {
            Hide();
        }

        public virtual void Dispose()
        {
            Hide();
        }

        public virtual void UpdateLogic(float value)
        {

        }

        public virtual void PerformSkill()
        {
            Show();
        }

        public virtual void RepeatSkill()
        {

        }

        public virtual void ReleaseSkill()
        {

        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
