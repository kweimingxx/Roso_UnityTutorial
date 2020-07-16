using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace BoidCharacter
{
    public class GridSpawner : MonoBehaviour
    {
        public Texture2D imageToRead; // notice to set image being readable
        public bool isSpawn =true;
        private GameObject[] imageWall;
        [Header("Bounds")]
        [SerializeField]
        float3 _areaCenter = float3(0, 0, 0);
        [SerializeField]
        float3 _areaSize = float3(1, 1, 1);
        int count = 0;
        Vector3 pos;
        [System.Serializable]
        public struct BoidAffector
        {
            public Vector3 position;
            public float force;
        }
        public BoidAffector[] affectorData;

        void Awake()
        {
            ColorCubeSpawner();
        }


        void ColorCubeSpawner()
        {
            Color[] pixels = imageToRead.GetPixels();
            imageWall = new GameObject[pixels.Length];
            affectorData = new BoidAffector[pixels.Length];

            
            for (int index = 0; index < pixels.Length; index++)
            {
                //Debug.Log("Color  " + pixels[index]);
                if (pixels[index].a >= 0.1)
                {
                    pos = new Vector3(index % imageToRead.width, index / imageToRead.width, 0);

                    //Debug.Log("jifdsaj");
                    var affector = new BoidAffector();
                    affector.position = pos;
                    affector.force = 0;
                    affectorData[count] = affector;
                    count++;

                }

            }
            GroupResize(count, ref affectorData);

            if (isSpawn)
            {
                for (int index = 0; index < pixels.Length; index++)
                {
                    //Debug.Log("Color  " + pixels[index]);
                    if (pixels[index].a >= 0.1)
                    {
                        imageWall[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        imageWall[index].transform.position = new Vector3(index % imageToRead.width, index / imageToRead.width, 0);
                        imageWall[index].GetComponent<MeshRenderer>().material.color = pixels[index];
                        imageWall[index].transform.parent = this.transform;

                    }

                }
            }
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