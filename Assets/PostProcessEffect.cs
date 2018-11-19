using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;

public class PostProcessEffect : MonoBehaviour {
	public Material simplePerlin, circleGradient, inverseCircleGradient;
	public Material add, subtract, multiply, lerp, mask, quantize;

	public bool saveToTexture;

	RenderTexture rt, fileSave;

	RenderTexture source, dest;

	Color c = Color.white;

	void Start() {
		if ( saveToTexture ) {
			fileSave = new RenderTexture( Screen.width, Screen.height, 24, GraphicsFormat.R8G8B8A8_SRGB );
		}
	}

	void OnRenderImage( RenderTexture source, RenderTexture dest ) {
		this.source = RenderTexture.GetTemporary(source.width, source.height);
		this.dest = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, this.source);
		
		//syntax: Operation( x-offset, y-offset, scale, strength, noiseType )
		//Operations: Add, Multiply, Lerp, Subtract, Quantize, Mask
		//noiseTypes: simplePerlin, circleGradient, inverseCircleGradient

		//add code here
		/*
		Quantize(0.01f);

		//CIRCLE GRADIENT FOLLOWING MOUSE IN A SLOW RED CLOUD
		float offsetX = ( -Input.mousePosition.x / Screen.width ) + 0.5f;
		float offsetY = ( -Input.mousePosition.y / Screen.height ) + 0.5f;

		//SetColor(0, 1, 0);
		//Add( offsetX, offsetY, .1f, 1f, inverseCircleGradient );
		//Quantize(.1f);
		
		SetColor(1,0,0);
		Add(Time.time * .03f, 1, 2f, .35f, simplePerlin);

		SetColor(1,.5f,0);
		Subtract(Time.time * .1f, 0, 1f, .15f, simplePerlin);

		//Quantize(.05f);
		SetColor(0,0,1);
		Add(Time.time * .2f, -1, .5f, .35f, simplePerlin);
		*/

		//EXAMPLE: STATIC RGB NOISE
		/*
		float t = Input.mousePosition.x * .01f;//Time.time * .25f;

		SetColor(1,0,0);
		Add(t,0,1,1,simplePerlin);

		SetColor(0,1,0);
		Add(1-t,1,1,1,simplePerlin);
		
		SetColor(0,0,1);
		Add(0.5f*t,0.5f,1,1,simplePerlin);
		*/

		//EXAMPLE: REPEATING EXPLOSION/IMPLOSION WAVE
		/*
		float t = Mathf.Sin( Time.time * .25f + Mathf.PI * 1.5f ) + 1;
		for( float s = t; s > 0; s -= .1f ) {
			SetColor(s, 1, 1-s );
			Add( 0, 0, s, .5f, inverseCircleGradient );
			Subtract( 0, 0, s-0.05f, .5f, inverseCircleGradient );
		}
		*/
		//Quantize(0.025f);
		

		//stop adding code here

		if ( saveToTexture ) {
			Graphics.Blit(this.dest, fileSave);
		}
		
		Graphics.Blit(this.dest, dest);
		RenderTexture.ReleaseTemporary(this.source);
		RenderTexture.ReleaseTemporary(this.dest);
	}

	void OnApplicationQuit() {
		if ( saveToTexture ) {
			RenderTexture.active = fileSave;

			Texture2D savedTex = new Texture2D( fileSave.width, fileSave.height, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None );
			savedTex.ReadPixels( new Rect(0, 0, fileSave.width,fileSave.height), 0, 0);
			byte[] pngBytes = ImageConversion.EncodeToPNG(savedTex);

			RenderTexture.active = null;

			string fileName = System.DateTime.Now.ToLongTimeString();
			fileName = fileName.Replace(":","-");
			fileName = fileName.Replace(" ", "");

			System.IO.File.WriteAllBytes( Application.dataPath + "/" + fileName + ".png", pngBytes );

			AssetDatabase.Refresh();
		}
	}

	//HERE BE DRAGONS
	//
	void Add( float offsetX, float offsetY, float scale, float strength, Material material ) {
		DoCombination( new Vector2( offsetX, offsetY), scale, strength, material, add );
	}

	void Subtract( float offsetX, float offsetY, float scale, float strength, Material material ) {
		DoCombination( new Vector2(offsetX, offsetY), scale, strength, material, subtract );
	}

	void Multiply( float offsetX, float offsetY, float scale, float strength, Material material ) {
		DoCombination( new Vector2(offsetX, offsetY), scale, strength, material, multiply );
	}

	void Lerp( float offsetX, float offsetY, float scale, float strength, Material material ) {
		DoCombination( new Vector2(offsetX, offsetY), scale, strength, material, lerp );
	}

	void Mask( float offsetX, float offsetY, float scale, float strength, Material material ) {
		DoCombination( new Vector2(offsetX, offsetY), scale, strength, material, mask );
	}

	void Quantize( float scale ) {
		quantize.SetFloat( "_Scale", scale );
		Graphics.Blit(source, dest, quantize );
		Graphics.Blit(dest, source);
	}

	void SetColor( float r, float g, float b ) {
		c.r = r;
		c.g = g;
		c.b = b;
	}

	void DoCombination( Vector2 offset, float scale, float strength, Material material, Material op) {
		rt = RenderTexture.GetTemporary(Screen.width, Screen.height);

		material.SetVector("_Offset", offset);
		material.SetFloat("_Scale", scale);
		material.SetFloat("_Strength", scale);
		material.SetColor("_Color", c );
		Graphics.Blit(null, rt, material);

		op.SetTexture("_SecondTex", rt);
		op.SetFloat("_Strength", strength);
		op.SetColor("_Color", c );
		Graphics.Blit(source, dest, op);
		
		RenderTexture.ReleaseTemporary(rt);

		Graphics.Blit(dest, source);
	}
}
