// Original reference: https://www.shadertoy.com/view/3d2XDh

Shader "Ink/Gravity Particles"
{

	Properties
	{
		_PARTICLE_NUM("Particle Number",  Range(0, 30)) = 27
		_PARTICLE_SIZE("Particle Size", Range(0, 30)) = 15.00
		_OPACITY("Opacity", Range(0, 10)) = 2
		//_MainTex("Texture", 2D) = "white" {}
		//_MaskTex("MaskTexture", 2D) = "white" {}
		//_ColorHigh("Color High Speed", Color) = (1, 0, 0, 1)
	}

	SubShader
	{

//-------------------------------------------------------------------------------------------
	
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "AutoLight.cginc"
		#include "Lighting.cginc"
		#pragma vertex SetVertexShader
		#pragma fragment SetPixelShader
		#define NB_PARTICLES 27		//27
		#define MAXNB_PARTICLES 40
		#define PARTICLE_SIZE 7.5		//21.5
		#define BOUNCE 0.8
		#define MAX_SPEED 700000.0
		#define ATTRACTION 1000000.0
		#define OPACITY 2.0			//2.0

		Texture2D _BufferA;	
		Texture2D _BufferB;
		
		SamplerState sampler_linear_repeat;
			
		int iFrame;
		float4 iMouse;
		float4 iResolution;
		float iTimeDelta;
		//float NB_PARTICLES = 20;
		float add;
		float _PARTICLE_NUM;
		float _PARTICLE_SIZE;
		float _OPACITY;
		float _SLOPEATTRAC;

		
		void SetVertexShader (inout float4 vertex:POSITION, inout float2 uv:TEXCOORD0)
		{
			vertex = UnityObjectToClipPos(vertex);
		}
		
		ENDCG

//-------------------------------------------------------------------------------------------

		Pass
		{ 
			CGPROGRAM

			float nrand( float2 n )
			{
				return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
			}
		
			void SetPixelShader (float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{
				float2 fragCoord = uv * iResolution.xy;
				float2 pixelSize = 1.0 / iResolution.xy*add;
				//uv *= 2;
				//add = 0;
				float4 previousFrameValues = _BufferA.Sample(sampler_linear_repeat, uv); //tex2D(_BufferA, uv);
				float2 position = previousFrameValues.xy*iResolution.xy;
				float2 velocity = previousFrameValues.zw*iResolution.xy * 0.99;
 
				if (iMouse.z>0.01)
				{
					float2 attractionVector = (iMouse.xy) - position; //0.5 * iResolution.x, iMouse.xy
					if (length(attractionVector) > 20.)
					velocity += ATTRACTION * (normalize(attractionVector)/length(attractionVector))*iMouse.z;
				}

				float2 force = float2(0, 0);

				for (int y= 0; y < _PARTICLE_NUM;++y)
				{
					for (int x= 0; x < _PARTICLE_NUM;++x)  //20x20 400
					{
						float4 other =  _BufferA.Load(int3(x,y,0));  //Vector4(1 / width, 1 / height, width, height)
						float2 Line = other.xy * iResolution.xy - position; //xy->pos
						float len = length(Line);
						float2 normal = normalize(Line);

						if (length(Line) > 0.001)
							force += 150.*Line/len/len;
						if (len < 2.*_PARTICLE_SIZE && len > 0.001)
						{
							float2 vDiff = velocity - other.zw*iResolution.xy;
							float approach = dot(vDiff,normal);
							if (approach > 0.)
							{
								force -= 0.025*approach*normal;
							}
						}
						if (iMouse.w > 0.01)    //add more particles
						{
						}
					}         
				}
				float r = nrand(float2(uv.x, _Time.y));

				//if (iMouse.w > 0.01 && uv.y * 50 < 0.3 + r / 5 && uv.y * 50 > r / 5)    //re-emit particles
				if (iMouse.w > 0.01 && r<0.01)  //0.05  //re-emit particles
				{
					position = float2(step(0.5, uv.x * 50), iMouse.y/ iResolution.y + nrand(uv.yx)*0.1) * iResolution.xy;

					//fragColor = float4(float2(step(0.5, uv.x * 50), 0.5 + nrand(uv.yx) * 0.15), 0.0, 0.0);
					//NB_PARTICLES += 5;
					//fragColor = float4(float2((clamp(nrand(uv) * 2.8, 0.9, 1.9) - 0.9), nrand(uv.yx)), 0.0, 0.0); //nrand(uv)
					//return;
					//float4 tPos = float4(float2((clamp(nrand(uv) * 2.8, 0.9, 1.9) - 0.9), nrand(uv.yx)), 0.0, 0.0);
					//force = 100.0*(tPos - position);
					//fragColor = float4(float2(nrand(iMouse.xy*uv), nrand(iMouse.xy*uv.yx)), 0.0, 0.0);
					//return;
				}
				if (_SLOPEATTRAC != 0&& position.y > 0.0 && position.y < iResolution.y) {
					position.y += _SLOPEATTRAC;
				}
				velocity += force;

				if (position.x < 0.0)
				{	
					velocity = float2(abs(velocity.x) * BOUNCE, velocity.y);
				}

				if (position.x > iResolution.x)
				{	
					velocity = float2(-abs(velocity.x) * BOUNCE, velocity.y);
				}

				if (position.y < 0.0)
				{
					velocity = float2(velocity.x, abs(velocity.y) * BOUNCE);
				}

				if (position.y > iResolution.y)
				{
					velocity = float2(velocity.x, -abs(velocity.y) * BOUNCE);
				}

				position.xy += velocity * iTimeDelta * 0.001;


				if (iFrame < 2)
				{
					
					fragColor = float4(float2(step(0.5, uv.x * 50), 0.5 + nrand(uv.yx) * 0.15), 0.0, 0.0); //nrand(uv), nrand(uv.yx) (clamp(nrand(uv) * 2.8, 0.9, 1.9) - 0.9)  nrand(float2(iResolution.y*0.3, iResolution.y*0.5))
					return;
				}


				fragColor = float4(position.xy/iResolution.xy, velocity.xy/iResolution.xy);
  
			}
			
			ENDCG
		}
		
//-------------------------------------------------------------------------------------------
		
		Pass
		{ 
			CGPROGRAM

			void SetPixelShader (float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{	
				float2 fragCoord = uv * iResolution.xy;
				float3 finalColor = float3(1.0,1.0,1.0);
				
				for (int x = 0; x < _PARTICLE_NUM; x++)
				{
					for (int y = 0; y < _PARTICLE_NUM; y++)
					{
						float4 currentParticle = _BufferA.Load( int3(x, y, 0));
						float2 dist = (currentParticle.xy - uv)*iResolution.xy;
						if (length(dist)  <  _PARTICLE_SIZE)
						{
							float val = (currentParticle.z * currentParticle.z + currentParticle.w * currentParticle.w) *0.000001;
							float3 velocityColor = float3(val, val, val) * _OPACITY; //(-0.25 + val, 0.1, 0.25-val)
							finalColor = velocityColor.xyz;//+= velocityColor.xyz;
						} 
					}
				}
				
				float3 previous = _BufferB.Sample(sampler_linear_repeat, uv).xyz;
				fragColor = float4(finalColor+0.7*previous, 1.0);   
			}
			
			ENDCG
		}

//-------------------------------------------------------------------------------------------
	
		Pass
		{ 
			CGPROGRAM

			void SetPixelShader (float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{
				fragColor = _BufferB.Sample(sampler_linear_repeat,uv);
			}
			
			ENDCG
		}

//-------------------------------------------------------------------------------------------		
	}
}