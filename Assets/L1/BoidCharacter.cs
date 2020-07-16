using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BoidCharacter
{ 
    public class BoidCharacter : MonoBehaviour
    {
        // Boidデータの構造体
        [System.Serializable]
        struct BoidData
        {
            public Vector3 Velocity; // 速度
            public Vector3 Position; // 位置
        }
        public struct BoidAffector
        {
            public Vector3 position;
            public float force;
        }
        // スレッドグループのスレッドのサイズ
        const int SIMULATION_BLOCK_SIZE = 256;

        #region Boids Parameters
        // 最大數量
        [Range(256, 32768)]
        public int MaxObjectNum = 16384;

        // 與其他的結合半徑
        public float CohesionNeighborhoodRadius = 2.0f;
        // 
        public float AlignmentNeighborhoodRadius = 2.0f;
        // 
        public float SeparateNeighborhoodRadius = 1.0f;

        // 速度最大値
        public float MaxSpeed = 5.0f;
        // 加速度最大値
        public float MaxSteerForce = 0.5f;

        // 結合權重
        public float CohesionWeight = 1.0f;
        // 整列權重
        public float AlignmentWeight = 1.0f;
        // 分離權重
        public float SeparateWeight = 3.0f;

        // 
        public float AvoidWallWeight = 10.0f;

        //   
        public Vector3 WallCenter = Vector3.zero;
        // 
        public Vector3 WallSize = new Vector3(32.0f, 32.0f, 32.0f);

        public float ForceScale = 1;
        public bool DrawingAffectors = true;  //Transform AttractPt;
        #endregion
        public MeshPtBaker meshPtBaker;
        public GridSpawner gridSpawner;
        private Vector3 _planePoint;
        public float AffectorDistance;
        public float AffectorForce;
        private bool _startChangeAffectorDistance;
        Vector3 _planeNormal;

        #region Built-in Resources
        // 
        public ComputeShader BoidsCS;
        #endregion

        #region Private Resources
        //
        ComputeBuffer _boidForceBuffer;
        //
        ComputeBuffer _boidDataBuffer;

        ComputeBuffer BoidAffectorBuffer;

        #endregion

        #region Accessors
        // 取得Boid的基本buffer data取得
        public ComputeBuffer GetBoidDataBuffer()
        {
            return this._boidDataBuffer != null ? this._boidDataBuffer : null;
        }

        // 取得object數
        public int GetMaxObjectNum()
        {
            return this.MaxObjectNum;
        }

        // simulation中心座標
        public Vector3 GetSimulationAreaCenter()
        {
            return this.WallCenter;
        }

        // 
        public Vector3 GetSimulationAreaSize()
        {
            return this.WallSize;
        }
        #endregion

        #region MonoBehaviour Functions
        void Start()
        {
            // 初始化
            InitBuffer();
        }

        void Update()
        {
            // 
            Simulation();
        }

        void OnDestroy()
        {
            // 
            ReleaseBuffer();
        }

        void OnDrawGizmos()
        {
            // 
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(WallCenter, WallSize);
        }
        #endregion

        #region Private Functions
        // buffer初期化
        void InitBuffer()
        {
            // buffer初期化
            _boidDataBuffer = new ComputeBuffer(MaxObjectNum,
                Marshal.SizeOf(typeof(BoidData)));
            _boidForceBuffer = new ComputeBuffer(MaxObjectNum,
                Marshal.SizeOf(typeof(Vector3)));

            // Boid data, Force bufferを初期化
            var forceArr = new Vector3[MaxObjectNum];
            var boidDataArr = new BoidData[MaxObjectNum];
            for (var i = 0; i < MaxObjectNum; i++)
            {
                forceArr[i] = Vector3.zero;
                boidDataArr[i].Position = Random.insideUnitSphere * 1.0f;
                boidDataArr[i].Velocity = Random.insideUnitSphere * 0.1f;
            }
            _boidForceBuffer.SetData(forceArr);
            _boidDataBuffer.SetData(boidDataArr);
            forceArr = null;
            boidDataArr = null;
        }

        // 
        void Simulation()
        {
            ComputeShader cs = BoidsCS;
            int id = -1;

            // 
            int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);

            // Force計算
            id = cs.FindKernel("ForceCS"); // kernal ID 取得
            cs.SetInt("_MaxBoidObjectNum", MaxObjectNum);
            cs.SetFloat("_CohesionNeighborhoodRadius", CohesionNeighborhoodRadius);
            cs.SetFloat("_AlignmentNeighborhoodRadius", AlignmentNeighborhoodRadius);
            cs.SetFloat("_SeparateNeighborhoodRadius", SeparateNeighborhoodRadius);
            cs.SetFloat("_MaxSpeed", MaxSpeed);
            cs.SetFloat("_MaxSteerForce", MaxSteerForce);
            cs.SetFloat("_SeparateWeight", SeparateWeight);
            cs.SetFloat("_CohesionWeight", CohesionWeight);
            cs.SetFloat("_AlignmentWeight", AlignmentWeight);
            cs.SetVector("_WallCenter", WallCenter);
            cs.SetVector("_WallSize", WallSize);
            cs.SetFloat("_AvoidWallWeight", AvoidWallWeight);

            BoidAffectorBuffer = new ComputeBuffer(gridSpawner.affectorData.Length, Marshal.SizeOf(typeof(BoidAffector)));
            BoidAffectorBuffer.SetData(gridSpawner.affectorData);
            cs.SetInt("_AffectorCount", gridSpawner.affectorData.Length);
            cs.SetFloat("AffectorDistance", AffectorDistance);
            cs.SetFloat("AffectorForce", AffectorForce);
            _planePoint = gridSpawner.affectorData[100].position;
            cs.SetVector("PlanePoint", _planePoint);
            _planeNormal = ModelPlaneProjection.CalculatePlaneNormals(gridSpawner.affectorData);

            cs.SetVector("PlaneNormal", _planeNormal);//new Vector3(0, 0, 1));//
            cs.SetFloat("_DeltaTime", Time.deltaTime);

            //Debug.Log("meshPtBaker.affectorData. " + meshPtBaker.affectorData.Length);
            Debug.Log("_AffectorCount " + gridSpawner.affectorData.Length);
            Debug.Log("_planeNormal " + _planeNormal);
            cs.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
            cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
            cs.SetBuffer(id, "_BoidAffectorDataBuffer", BoidAffectorBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行

            // 由加速度、計算速度&位置
            id = cs.FindKernel("IntegrateCS"); // カーネルIDを取得
            cs.SetFloat("_DeltaTime", Time.deltaTime);
            cs.SetFloat("force_Scale", ForceScale);

            //cs.SetVector("llpo1", Head.pos[0]);     //new Vector3(AttractPts[0].position.x, AttractPts[0].position.y, AttractPts[0].position.z));
            //cs.SetVector("llpo2", Hands.pos[0]);    //new Vector3(AttractPts[1].position.x, AttractPts[1].position.y, AttractPts[1].position.z));
            //cs.SetVector("llpo3", Hands.pos[1]);
            //Debug.Log("attpts0" + new Vector3(AttractPts[0].position.x, AttractPts[0].position.y, AttractPts[0].position.z));
            //Debug.Log("attpts1"+ new Vector3(AttractPts[1].position.x, AttractPts[1].position.y, AttractPts[1].position.z));
            cs.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
            cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行
        }

        //private void GenerateCharacterAffectorFromData(InkData character)
        //{
        //    if (BoidAffectorBuffer != null)
        //    {
        //        BoidAffectorBuffer.Release();
        //    }
        //    AffectorCounts = character.inks.Length / 3;
        //    var affectorData = new BoidAffector[AffectorCounts];
        //    //TODO: Put it in a function
        //    float randomAngleX = UnityEngine.Random.Range(-50, 50);
        //    float randomAngleY = UnityEngine.Random.Range(-50, 50);
        //    Debug.Log(randomAngleX);
        //    Debug.Log(randomAngleY);
        //    Debug.Log("-------------------------------");
        //    for (int i = 0; i < AffectorCounts; i++)
        //    {
        //        int xIndex = i * 3;
        //        int yIndex = i * 3 + 1;
        //        int zIndex = i * 3 + 2;
        //        float xPos = character.inks[xIndex];
        //        float yPos = character.inks[yIndex];
        //        //float zPos = character.inks[zIndex]; 
        //        Vector3 position = new Vector3(xPos, yPos, 50);
        //        //position = RotatePointAroundPivot(position, Vector3.zero, Quaternion.Euler(new Vector3(0, 135, 0)));
        //        Vector3 rotatedPosition = RotatePointAroundPivot(position, new Vector3(50, 50, 50), new Vector3(randomAngleX, randomAngleY, -90));
        //        var affector = new BoidAffector();
        //        affector.position = rotatedPosition;
        //        affector.force = 0;
        //        affectorData[i] = affector;
        //    }
        //    if (DrawingAffectors)
        //    {
        //        foreach (var affector in affectorData)
        //        {
        //            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //            go.transform.localScale = new Vector3(1, 1, 1);
        //            go.transform.position = affector.position;
        //        }
        //    }
        //    _planeNormal = TestPlaneProjection.CalculatePlaneNormals(affectorData);
        //    Debug.Log(_planeNormal);
        //    Debug.Log("IIIIIIIIIIIIIIIIIIII");
        //    _planePoint = affectorData[89].position;
        //    //StartMoveCamera(_planeNormal);
        //    BoidAffectorBuffer = new ComputeBuffer(AffectorCounts, Marshal.SizeOf(typeof(BoidAffector)));
        //    BoidAffectorBuffer.SetData(affectorData);
        //    affectorData = null;
        //    _startChangeAffectorDistance = true;
        //    AffectorDistance = -2f;
        //}

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }
        // 解放
        void ReleaseBuffer()
        {
            if (_boidDataBuffer != null)
            {
                _boidDataBuffer.Release();
                _boidDataBuffer = null;
            }

            if (_boidForceBuffer != null)
            {
                _boidForceBuffer.Release();
                _boidForceBuffer = null;
            }
        }
        #endregion
    } // class
} // namespace