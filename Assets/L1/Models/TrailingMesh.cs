using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailingMesh : MonoBehaviour
{


	private Material mat;
	private Mesh Tmesh;
	private GameObject TgameObject;
	private MeshFilter mf;

	private SkinnedMeshRenderer render;
	public int[] index;
	public Vector3[] pos;
	class Bone
	{
		internal Transform bone;
		internal float weight;
		internal Vector3 delta;
	}
	List<List<Bone>> allBones = new List<List<Bone>>();

	// Use this for initialization
	void Start()
	{
		render = GetComponent<SkinnedMeshRenderer>();

		TgameObject = new GameObject("TarlingObject");//特效对象

		var mr = TgameObject.AddComponent<MeshRenderer>();//赋予材质
		mat = new Material(Shader.Find("Standard"));
		mr.material = mat;


		mf = TgameObject.AddComponent<MeshFilter>();


	}

	// Update is called once per frame
	void Update()
	{

		#region-------从SkinnedMeshRenderer上拷贝可以正确显示的网格
		TgameObject.transform.position = transform.position;
		TgameObject.transform.rotation = transform.rotation;
		Tmesh = new Mesh();
		render.BakeMesh(Tmesh);
		#endregion


		mf.mesh = Tmesh;
		for (int i = 0; i < index.Length; i++)
		{
			pos[i] = mf.mesh.vertices[index[i]] - new Vector3(0,12,0);  //how to get roatation
		}
		//Debug.Log(mf.mesh.vertices.Length);

	}

	void OnDrawGizmos() {
		Vector3 position = Vector3.zero;

		//for (int i=0;i< mf.mesh.vertices.Length;i++) {
		if (mf!=null) {
			for (int i = 0; i < index.Length; i++)
			{
				position = mf.mesh.vertices[index[i]] - new Vector3(0, 12, 0);  //how to get roatation
				Gizmos.DrawWireCube(position, 1f * Vector3.one);
			}
		}
		//}


	}

}