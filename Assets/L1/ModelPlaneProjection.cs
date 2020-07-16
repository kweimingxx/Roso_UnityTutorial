using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using BoidCharacter;
public class ModelPlaneProjection : MonoBehaviour
{
    private GridSpawner.BoidAffector[] _affectors;
    private Vector3[] _randomBalls;
    private Vector3[] _projectedPoints;
    private int AffectorCounts;
    public int RandomBallsCount;
    public GameObject Target;
    public float SpawnRadius;
    private Vector3 _planeNormal;

    // Start is called before the first frame update
    void Start()
    {
        string path = "Assets/CharacterTest/ren.json";
        ReadDataToAffectors(path);
        DrawFromAffector();
        CreateDataIntoRandomBalls();
        Vector3 normal = CalculatePlaneNormals(_affectors);
        Debug.Log(normal);
        ProjectOnToPlane(normal);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateDataIntoRandomBalls()
    {
        _randomBalls = new Vector3[RandomBallsCount];
        _projectedPoints = new Vector3[RandomBallsCount];
        for (int i = 0; i < RandomBallsCount; i++)
        {
            Vector3 pos = Target.transform.position + UnityEngine.Random.insideUnitSphere * SpawnRadius;
            _randomBalls[i] = pos;
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            go.transform.position = pos;
        }
    }

    private void ReadDataToAffectors(string path)
    {
        string jsonString = File.ReadAllText(path);
        //InkData character = JsonUtility.FromJson<InkData>(jsonString);
        AffectorCounts = _affectors.Length;
        _affectors = new GridSpawner.BoidAffector[AffectorCounts];
        for (int i = 0; i < AffectorCounts; i++)
        {
            float xPos = _affectors[i].position.x;
            float yPos = _affectors[i].position.y;
            //float zPos = character.inks[zIndex]; 
            Vector3 position = new Vector3(xPos, yPos, 0);
            var affector = new GridSpawner.BoidAffector();
            affector.position = position;
            affector.force = 0;
            _affectors[i] = affector;
        }
    }

    private void DrawFromAffector()
    {
        //foreach(var affector in _affectors) {
        //    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    go.transform.localScale = new Vector3(1,1,1);
        //    Vector3 rotatedPosition = ParticleFlock.RotatePointAroundPivot(affector.position, new Vector3(50,50,0), new Vector3(60, 0, 0));
        //    affector.position = rotatedPosition;
        //    go.transform.position = rotatedPosition;
        //}

        for (int i = 0; i < AffectorCounts; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(1, 1, 1);
            //Vector3 rotatedPosition = ParticleFlock.RotatePointAroundPivot(_affectors[i].position, new Vector3(50, 50, 0), new Vector3(60, 20, 50));
            //_affectors[i].position = rotatedPosition;
            go.transform.position = _affectors[i].position;
        }

    }

    public static Vector3 CalculatePlaneNormals(GridSpawner.BoidAffector[] affectors)
    {
        Vector3 p0 = affectors[100].position;
        Vector3 p1 = affectors[31].position;
        Vector3 p2 = affectors[32].position;
        Vector3 v0 = p1 - p0;
        Vector3 v1 = p2 - p0;
        Debug.Log(v0);
        Debug.Log(v1);
        Debug.Log("-------------");
        return Vector3.Cross(v0, v1).normalized;
    }

    private void ProjectOnToPlane(Vector3 faceNormal)
    {
        Vector3 planePoint = _affectors[20].position;
        foreach (var pos in _randomBalls)
        {
            Vector3 projectedPoint = PlaneMath.ProjectPointOnPlane(faceNormal, planePoint, pos);
            Debug.DrawLine(pos, projectedPoint, Color.red, 500);
        }
    }



}
