using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Vfx
{
    //TODO: implement pool managing with every hit effect vfx in the game.
    public class VfxManager : MonoBehaviour
    {
        [Header("Impact Decal")]
        [SerializedDictionary("Type", "Material")]
        public SerializedDictionary<DecalType, Material> materialDict;
        [SerializeField] private DecalVfx prefab;
        private Queue<DecalVfx> decalPool;
        private List<DecalVfx> decals;

        [Header("Energy Orb")]
        public OrbVfx orbPrefab;

        private static VfxManager instance;
        public static VfxManager Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        public void Initialize()
        {
            decalPool = new();
            decals = new();
        }

        public void Dispose()
        {
            for (int i = 0; i < decals.Count; i++)
            {
                Destroy(decals[i].gameObject);
            }

            decalPool.Clear();
            decalPool = null;
            decals = null;
        }

        #region impact decal
        public void RentVfx(ElementType element, Vector3 position, Quaternion rotation)
        {
            if (decalPool == null) return;

            Material material = element switch
            {
                ElementType.None => null,
                ElementType.Lightning => materialDict[DecalType.LIGHTNING],
                ElementType.Magia => materialDict[DecalType.MAGIC],
                ElementType.Void => materialDict[DecalType.VOID],
                _ => null,
            };

            if (material == null) return;

            DecalVfx vfx;

            if (decalPool.Count > 0)
            {
                vfx = decalPool.Dequeue();
                vfx.gameObject.SetActive(true);
                vfx.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                vfx = Instantiate(prefab, position, rotation);
                decals.Add(vfx);
            }

            vfx.Initialize(material, 3, this);
        }

        public void ReturnDecal(DecalVfx decal)
        {
            if (decalPool == null) return;

            decalPool.Enqueue(decal);
        }
        #endregion

        #region orb
        public void RentOrb(Vector3 position, Transform target)
        {
            var orb = orbPrefab.Rent(position, Quaternion.identity);
            orb.Initialize(target, Random.onUnitSphere);
        }
        #endregion
    }

    public enum DecalType
    {
        MAGIC,
        LIGHTNING,
        VOID,
        EXPLODE,
    }
}
