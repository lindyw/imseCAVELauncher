Shader "ECOnTop/UI" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		LOD 200
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
         
		Lighting Off 
		Cull Off 
		ZTest Off
		ZWrite On 
		Blend SrcAlpha OneMinusSrcAlpha
		 
		Pass 
		{
			SetTexture [_MainTex] {
				constantColor [_Color]
				combine constant lerp(texture) primary
			}
			// Multiply in texture
			SetTexture [_MainTex] {
				combine primary * texture
			}
		}
	}
}