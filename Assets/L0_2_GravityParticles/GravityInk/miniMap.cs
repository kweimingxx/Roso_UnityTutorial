using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class miniMap : MonoBehaviour
{
	Vector3 down;
	Vector3 horiz;
	public GameObject miniplane;
	public GameObject[] walls;
	public GameObject GravityShader;
	float resolution;
	float planeSize;
	float LengthX;
	float LengthY;
	Vector3 startCoord;
	Vector3 endCoord;

	public Vector2 FPSCoord;

	Vector3[] w_startCoord;
	Vector3[] w_endCoord;
	public Vector2 WallCoord;
	int dir;
	float w_LengthX;
	float w_LengthY;
	// Start is called before the first frame update
	void Start()
    {
		dir = 1;
		down = transform.TransformDirection(Vector3.down) * 50;
		horiz = transform.TransformDirection(new Vector3(1,0,0)) * 50;
		resolution = GravityShader.GetComponent<GravityParticles>().Resolution;
		planeSize *= 10;
		startCoord = miniplane.transform.position + new Vector3((-1) * miniplane.transform.localScale.x * 10 / 2, 0, miniplane.transform.localScale.z * 10 / 2 );
		endCoord = miniplane.transform.position + new Vector3(miniplane.transform.localScale.x * 10 / 2, 0, (-1) * miniplane.transform.localScale.z * 10 / 2);
		startCoord.y = endCoord.y = 0;
		LengthX = Mathf.Abs(endCoord.x - startCoord.x);
		LengthY = LengthX;//Mathf.Abs(endCoord.z - startCoord.z);

		if (walls.Length > 1)
		{
			w_startCoord = new Vector3[2];
			w_endCoord = new Vector3[2];

			w_startCoord[0] = walls[0].transform.position + new Vector3(walls[0].transform.localScale.x * 10 / 2, (-1) * walls[0].transform.localScale.z * 10 / 2, 0);
			w_endCoord[0] = walls[0].transform.position + new Vector3(-1 * walls[0].transform.localScale.x * 10 / 2, (1) * walls[0].transform.localScale.z * 10 / 2, 0);
			w_startCoord[1] = walls[1].transform.position + new Vector3(walls[1].transform.localScale.x * 10 / 2, (-1) * walls[1].transform.localScale.z * 10 / 2, 0);
			w_endCoord[1] = walls[1].transform.position + new Vector3(-1 * walls[1].transform.localScale.x * 10 / 2, (1) * walls[1].transform.localScale.z * 10 / 2, 0);
			Debug.Log("w_startCoord[1] " + w_startCoord[1]);
			Debug.Log("w_endCoord[0]" + w_endCoord[0]); 
			Debug.Log("w_startCoord[0] " + w_startCoord[0]);
			Debug.Log("w_endCoord[1]" + w_endCoord[1]);
			w_LengthX = Mathf.Abs(w_endCoord[0].x - w_startCoord[0].x);
			w_LengthY = w_LengthX;
		}

	}

	// Update is called once per frame
	void Update()
	{
		if (walls.Length>1) {
			
			dir = (FPSCoord.y < 0.5) ? 1 : -1;
			Debug.DrawRay(transform.position, horiz* dir, Color.grey);
			RaycastHit hitWall;
			if (Physics.Raycast(transform.position, horiz * dir, out hitWall))
			{
				Debug.DrawRay(transform.position, horiz * dir, Color.magenta);
				WallCoord = new Vector2(Mathf.Abs(hitWall.point.x - w_startCoord[1].x) / w_LengthX, Mathf.Abs(hitWall.point.y - w_endCoord[1].y) / w_LengthY);
				//WallCoord = new Vector2(Mathf.Abs(hitWall.point.x - w_startCoord[0].x) / w_LengthX, Mathf.Abs(hitWall.point.y - w_endCoord[0].y) / w_LengthY);
				if (dir == -1)
					WallCoord.y = 1 - WallCoord.y;
				//Debug.Log("WallCoord: " + WallCoord);

			}
			else {
				WallCoord.x = WallCoord.y = 0;
			}

		}

		Debug.DrawRay(transform.position, down, Color.green);

		RaycastHit hit;
		if (Physics.Raycast(transform.position, down, out hit))
		{
			//Debug.Log("hit: "+ hit.point);
			Debug.DrawRay(transform.position, down, Color.red);
			FPSCoord = new Vector2(Mathf.Abs(hit.point.x - startCoord.x-3) / LengthX, Mathf.Abs(hit.point.z - startCoord.z - 10)/LengthY);
			//offset value vary from different plane
		}
	//	Debug.Log("FPSCoord: " + FPSCoord);

	}

}
