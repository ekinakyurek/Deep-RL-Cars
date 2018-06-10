using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public static class ImageManipulation {


	public static int[,] mapping = { {2,8,5}, {3,0,6}, {1,7,4} }; 

	// Use this for initialization

	public static void test(){

		for (int y = 0; y < 3; y++)
		{
			for (int x = 0; x < 3; x++)
			{

				Debug.Log(mapping [x,y]);
			}
		}
	}

	public static void MakeImage(Texture2D texture, Vector<double> imarray) {
		
		double mx = imarray.Maximum ();
		int mxindex = imarray.MaximumIndex ();
		double mn = imarray.Minimum ();
		double rng = mx - mn;

		var img = imarray.Subtract (mn).Divide (rng);


		for (int y = 0; y < texture.height; y++)
		{
			for (int x = 0; x < texture.width; x++)
			{
				int index = mapping [x, y];
				float rgb = (float)img [index];
				Color color;
				if (index!= mxindex) {
					color = new Color (rgb, rgb, rgb, 1.0f);
				} else {
					color = Color.green;
				}
				texture.SetPixel (x, y, color);
			}
		}
	}

}
