// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013

// these won't change in a given iteration of the shader
float4x4 World;
float4x4 View;
float4x4 Projection;
float4 cameraPos;
float4 sunLightAmbCol;
float4 sunLightPntPos;
float4 sunLightPntCol;
float4 moonLightAmbCol;
float4 moonLightPntPos;
float4 moonLightPntCol;
float4x4 worldInvTrp;
float4 color = float4(0.5f, 0.5f, 0.5f, 1.0f);
//

struct VS_IN
{
	float4 pos : SV_POSITION;
	float4 nrm : NORMAL;
	//float4 col : COLOR;
// Other vertex properties, e.g. texture co-ords, surface Kd, Ks, etc
};

struct PS_IN
{
	float4 pos : SV_POSITION; //Position in camera co-ords
	float4 col : COLOR;
	float4 wpos : TEXCOORD0; //Position in world co-ords
	float3 wnrm : TEXCOORD1; //Normal in world co-ords 
};


PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	// Convert Vertex position and corresponding normal into world coords
	// Note that we have to multiply the normal by the transposed inverse of the world 
	// transformation matrix (for cases where we have non-uniform scaling; we also don't
	// care about the "fourth" dimension, because translations don't affect the normal)
	output.wpos = mul(input.pos, World);
	output.wnrm = mul(input.nrm.xyz, (float3x3)worldInvTrp);

	// Transform vertex in world coordinates to camera coordinates
	float4 viewPos = mul(output.wpos, View);
    output.pos = mul(viewPos, Projection);

	// Just pass along the colour at the vertex
	//output.col = input.col;
	output.col = color;
	return output;
}

float4 PS( PS_IN input ) : SV_Target
	{
	// Our interpolated normal might not be of length 1
	float3 interpNormal = normalize(input.wnrm);
	
	// Calculate ambient RGB intensities
	float Ka = 1;
	float3 amb = input.col.rgb*sunLightAmbCol.rgb*Ka;
	amb = amb + input.col.rgb*moonLightAmbCol.rgb*Ka;
	// Calculate diffuse RBG reflections
	float fAtt = 1;
	float Kd = 1;
	float3 Lsun = normalize(sunLightPntPos.xyz - input.wpos.xyz);
	float3 Lmoon = normalize(moonLightPntPos.xyz - input.wpos.xyz);
	float LsunDotN = saturate(dot(Lsun,interpNormal.xyz));
	float LmoonDotN = saturate(dot(Lmoon, interpNormal.xyz));
	float3 dif = fAtt*sunLightPntCol.rgb*Kd*input.col.rgb*LsunDotN;
	dif = dif + fAtt*moonLightPntCol.rgb*Kd*input.col.rgb*LmoonDotN;
	// Calculate specular reflections
	float Ks = 1;
	float specN = 5; // Numbers>>1 give more mirror-like highlights
	float3 V = normalize(cameraPos.xyz - input.wpos.xyz);
	float3 Rsun = normalize(2*LsunDotN*interpNormal.xyz - Lsun.xyz);
	float3 Rmoon = normalize(2 * LmoonDotN*interpNormal.xyz - Lmoon.xyz);
	//float3 R = normalize(0.5*(L.xyz+V.xyz)); //Blinn-Phong equivalent
	float3 spe = fAtt*sunLightPntCol.rgb*Ks*pow(saturate(dot(V,Rsun)),specN);
	spe = spe + fAtt*moonLightPntCol.rgb*Ks*pow(saturate(dot(V, Rmoon)), specN);

	// Combine reflection components
	float4 returnCol = float4(0.0f,0.0f,0.0f,0.0f);
	returnCol.rgb = amb.rgb+dif.rgb+spe.rgb;
	returnCol.a = input.col.a;

	return returnCol;
}



technique Lighting
{
    pass Pass1
    {
		Profile = 9.1;
        VertexShader = VS;
        PixelShader = PS;
    }
}