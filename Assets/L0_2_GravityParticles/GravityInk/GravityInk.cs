// Add script to quad and assign material with shader. Play.
// Use left mouse to interact

using UnityEngine;

[ExecuteInEditMode]
public class GravityInk : MonoBehaviour 
{
	public int Resolution = 1024;
	public Material material;
	RenderTexture RTA1, RTA2, RTB1, RTB2;
	bool swap = true;
	public bool FPSmapping;

	void Blit(RenderTexture source, RenderTexture destination, Material mat, string name, int pass)
	{
		RenderTexture.active = destination;
		mat.SetTexture(name, source);
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.invertCulling = true;
		mat.SetPass(pass);
		GL.Begin(GL.QUADS);
		GL.MultiTexCoord2(0, 0.0f, 0.0f);
		GL.Vertex3(0.0f, 0.0f, 0.0f);
		GL.MultiTexCoord2(0, 1.0f, 0.0f);
		GL.Vertex3(1.0f, 0.0f, 0.0f); 
		GL.MultiTexCoord2(0, 1.0f, 1.0f);
		GL.Vertex3(1.0f, 1.0f, 1.0f); //(1.0f, 1.0f, 0.0f); 
		GL.MultiTexCoord2(0, 0.0f, 1.0f);
		GL.Vertex3(0.0f, 1.0f, 0.0f);
		GL.End();
		GL.invertCulling = false;
		GL.PopMatrix();
	}
			
	void Start () 
	{
		RTA1 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT 40
		RTA2 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT 40
		RTB1 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT
		RTB2 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT	
		GetComponent<Renderer>().material = material;

		sizeValue = 15.0f;
		opacityValue = 2.0f;
		numValue = 27;
		slopeAttraction = 0;
	}
	
	void Update () 
	{
		
		RaycastHit hit;
		if (Input.GetMouseButton(0))
		{

			material.SetVector("iMouse", new Vector4(
				Input.mousePosition.x * Resolution / Screen.width, Input.mousePosition.y * Resolution / Screen.height,//hit.textureCoord.x * Resolution, hit.textureCoord.y * Resolution,
				1.0f, 0.0f));

		}
		else if (Input.GetMouseButton(1)) {
			Debug.Log("mouse clicked");
			material.SetVector("iMouse", new Vector4(
				Input.mousePosition.x * Resolution / Screen.width, Input.mousePosition.y * Resolution / Screen.height,//hit.textureCoord.x * Resolution, hit.textureCoord.y * Resolution,
				0.0f, 1.0f));
		}
		else
		{
		}
		
		material.SetInt("iFrame",Time.frameCount);
		material.SetVector("iResolution", new Vector4(Resolution,Resolution,0.0f,0.0f));
		material.SetFloat("iTimeDelta", Time.deltaTime);
		material.SetFloat("_PARTICLE_NUM", numValue);
		material.SetFloat("_PARTICLE_SIZE", sizeValue);
		material.SetFloat("_OPACITY", opacityValue);
		material.SetFloat("_SLOPEATTRAC", slopeAttraction);

		if (swap)
		{			
			material.SetTexture("_BufferA", RTA1);
			Blit(RTA1, RTA2, material, "_BufferA", 0);
			material.SetTexture("_BufferA", RTA2);
			
			material.SetTexture("_BufferB", RTB1);
			Blit(RTB1, RTB2, material, "_BufferB", 1);
			material.SetTexture("_BufferB", RTB2);			
		
		}
		else
		{			
			material.SetTexture("_BufferA", RTA2);
			Blit(RTA2, RTA1, material, "_BufferA", 0);
			material.SetTexture("_BufferA", RTA1);
			
			material.SetTexture("_BufferB", RTB2);
			Blit(RTB2, RTB1, material, "_BufferB", 1);
			material.SetTexture("_BufferB", RTB1);			
		
		}
		swap = !swap;
	}
	
	void OnDestroy ()
	{
		RTA1.Release();
		RTA2.Release();
		RTB1.Release();
		RTB2.Release();		
	}

	bool _bWindowActive;
	public bool bWindowActive
	{
		get { return _bWindowActive; }
		set
		{
			_bWindowActive = value;
			if (bWindowActive)
			{
				// This is called everytime, when bWindowActive = true;
				OnWindowInited();
			}
			if (!bWindowActive)
			{
				// This is called everytime, when bWindowActive = false;
				OnWindowClosed();
			}
		}

	}
	public float sizeValue;
	public float opacityValue;
	public float numValue;
	public float slopeAttraction;

	void OnGUI()
	{
		GUI.color = Color.red;

		GUI.Label(new Rect(0, 0, 100, 20), "Particle Size");
		sizeValue = GUI.HorizontalSlider(new Rect(75, 5, 100, 30), sizeValue, 0.0F, 30.0F);

		GUI.Label(new Rect(0, 35, 100, 20), "Opacity");
		opacityValue = GUI.HorizontalSlider(new Rect(75, 40, 100, 30), opacityValue, 0.0F, 10.0F);

		GUI.Label(new Rect(0, 70, 100, 20), "Particle Num");
		numValue = GUI.HorizontalSlider(new Rect(75, 75, 100, 30), numValue, 0, 30);

		GUI.Label(new Rect(0, 105, 100, 20), "Slope Gravity");
		slopeAttraction = GUI.HorizontalSlider(new Rect(75, 110, 100, 30), slopeAttraction, -10, 10);
		Debug.Log("slopeAttraction" + slopeAttraction);
	}

	public void OnWindowInited()
	{
		Debug.Log("Windows Inited");
	}

	public void OnWindowClosed()
	{
		Debug.Log("Windows Closed");
	}

	public void DoMyWindow(int windowID)
	{
		if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
			print("Got a click");
	}
}