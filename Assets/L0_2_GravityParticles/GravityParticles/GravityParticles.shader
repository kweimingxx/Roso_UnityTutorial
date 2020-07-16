// Original reference: https://www.shadertoy.com/view/3d2XDh

Shader "Gravity Particles"
{
	SubShader
	{

//-------------------------------------------------------------------------------------------
	
		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		
		Texture2D _BufferA;	
		Texture2D _BufferB;
		
		SamplerState sampler_linear_repeat;
			
		int iFrame;
		float4 iMouse;
		float4 iResolution;
		float iTimeDelta;
		
		void vert(inout float4 vertex:POSITION, inout float2 uv:TEXCOORD0)
		{
			vertex = UnityObjectToClipPos(vertex);
		}
		
		ENDCG

//-------------------------------------------------------------------------------------------

		Pass
		{ 
			CGPROGRAM

			#define NB_PARTICLES 20
			#define PARTICLE_SIZE 2.5
			#define BOUNCE 0.9
			#define MAX_SPEED 800000.0

			#define ATTRACTION 600000.0

			float nrand( float2 n )
			{
				return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
			}
		
			void frag(float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{
				float2 fragCoord = uv * iResolution.xy;
				float2 pixelSize = 1.0 / iResolution.xy;
			  
				if (iFrame < 2)
				{
					fragColor = float4(float2(nrand(uv), nrand(uv.yx)), 0.0 , 0.0);
					return;
				}
				
				float4 previousFrameValues = _BufferA.Sample(sampler_linear_repeat, uv);
				float2 position = previousFrameValues.xy * iResolution.xy;
				float2 velocity = previousFrameValues.zw * iResolution.xy;
 
				if (iMouse.w>0.01)
				{
					float2 attractionVector = (iMouse.xy) - position;
					if (length(attractionVector) > 20.)
					velocity += ATTRACTION * (normalize(attractionVector)/length(attractionVector));
				}
				float2 force = float2(0, 0);
				for (int y= 0; y < NB_PARTICLES;++y)
				{
					for (int x= 0; x < NB_PARTICLES;++x)
					{
						float4 other =  _BufferA.Load(int3(x,y,0));
						float2 Line = other.xy * iResolution.xy - position; 
						float len = length(Line);
						float2 normal = normalize(Line);

						if (length(Line) > 0.001)
						force += 150.*Line/len/len;
						if (len < 2.*PARTICLE_SIZE && len > 0.001)
						{
							float2 vDiff = velocity - other.zw*iResolution.xy;
							float approach = dot(vDiff,normal);
							if (approach > 0.)
							{
								force -= 0.025*approach*normal;
							}
						}
					}         
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

				if ( length(velocity) > MAX_SPEED)
					velocity = normalize(velocity) * MAX_SPEED;

				fragColor = float4(position.xy/iResolution.xy, velocity.xy/iResolution.xy);
  
			}
			
			ENDCG
		}
		
//-------------------------------------------------------------------------------------------
		
		Pass
		{ 
			CGPROGRAM

			#define NB_PARTICLES 20
			#define PARTICLE_SIZE 2.5
			#define OPACITY 2.0
			
			void frag(float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{	
				float2 fragCoord = uv * iResolution.xy;
				float3 finalColor = float3(0.0,0.0,0.0);
				
				for (int x = 0; x < NB_PARTICLES; x++)
				{
					for (int y = 0; y < NB_PARTICLES; y++)
					{
						float4 currentParticle = _BufferA.Load( int3(x, y, 0));
						float2 dist = (currentParticle.xy - uv)*iResolution.xy;
						if (length(dist)  <  PARTICLE_SIZE)
						{
							float val = (currentParticle.z * currentParticle.z + currentParticle.w * currentParticle.w) *0.000001;
							float3 velocityColor = float3(-0.25 + val, 0.1, 0.25-val) * OPACITY;
							finalColor += velocityColor.xyz;
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
					
			#define NB_PARTICLES 20
			#define PARTICLE_SIZE 2.5
			#define OPACITY 2.0

			void frag(float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{
				fragColor = _BufferB.Sample(sampler_linear_repeat,uv);
			}
			
			ENDCG
		}

//-------------------------------------------------------------------------------------------		
	}
}