using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace BoidCharacter { 
    public class MeshPtBaker : MonoBehaviour
    {
        public GameObject m_Model;
        public GameObject Cube;
        MeshFilter mf;
        public BoidAffector[] affectorData;
        BoidAffector[] affectorRawData;
        GameObject m_Affector;

        [Header("Bounds")]
        [SerializeField]
        float3 _areaCenter = float3(0, 0, 0);
        [SerializeField]
        float3 _areaSize = float3(1, 1, 1);
        int count = 0;
        [System.Serializable]
        public struct BoidAffector {
            public Vector3 position;
            public float force;
        }

        // Start is called before the first frame update
        void Awake()
        {
            mf = m_Model.GetComponent<MeshFilter>();
            Matrix4x4 localToWorld = transform.localToWorldMatrix;
            affectorData = new BoidAffector[mf.mesh.vertices.Length];
            //var affectorData = new BoidAffector[mf.mesh.vertices.Length];

            for (int i = 0; i < mf.mesh.vertices.Length; ++i)
            {
                Vector3 world_v = localToWorld.MultiplyPoint3x4(mf.mesh.vertices[i]);
                if (world_v.z < _areaCenter.z + _areaSize.z / 2 && world_v.z > _areaCenter.z - _areaSize.z / 2) {
                    m_Affector = Instantiate(Cube, world_v, this.transform.rotation);
                    //m_Affector.transform.parent = this.transform;
                    var affector = new BoidAffector();
                    affector.position = world_v;
                    affector.force = 0;
                    affectorData[count] = affector;
                    Debug.Log("between");
                    count++;
                }
                Debug.Log("count");

            }
            GroupResize(count, ref affectorData);

            
        }
        public void GroupResize(int Size, ref BoidAffector[] Group)
        {

            BoidAffector[] temp = new BoidAffector[Size];
            for (int c = 1; c < Mathf.Min(Size, Group.Length); c++)
            {
                temp[c] = Group[c];
            }
            Group = temp;
        }
        void OnDrawGizmos()
        {
            // Display Area
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube
            (
                float3(_areaCenter.x, _areaCenter.y, _areaCenter.z),
                float3(_areaSize.x, _areaSize.y, _areaSize.z)
            );
        }
    }
}