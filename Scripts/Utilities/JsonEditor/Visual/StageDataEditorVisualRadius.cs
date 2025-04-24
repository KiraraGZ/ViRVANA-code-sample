using Magia.Utilities.Pooling;
using UnityEngine;

namespace Magia.Utilities.Json.Editor
{
    public class StageDataEditorVisualRadius : PoolObject<StageDataEditorVisualRadius>
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Material material;

        public void Initialize()
        {
            sr.material = new(material);
        }

        public void Dispose()
        {
            Destroy(sr.material);
            sr.material = null;

            Return();
        }

        public void UpdateInfo(Vector2 range)
        {
            float inner;
            float outer;
            if (range.y >= range.x)
            {
                inner = range.x * 2 - 10;
                outer = range.y * 2 + 10;
            }
            else
            {
                inner = range.y * 2 - 10;
                outer = range.x * 2 + 10;
            }

            inner = inner < 0 ? 0 : inner;

            sr.material.SetFloat("_InnerRadius", inner);
            transform.localScale = Vector3.one * outer;
        }
    }
}
