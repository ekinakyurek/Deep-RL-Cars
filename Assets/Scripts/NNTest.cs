using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

	public class NNTest
	{

		public Matrix<double>[] W;


		public NNTest ()
		{

			var M = Matrix<double>.Build;

			double[,] x  = {{5.0},{3.0},{-1.0},{2.5},{-0.5},{0.3},{0.7},{1.9},{3.5},{-0.3}};
		
	     	double qtarget = 0.5;

			int inputsize = 10;

			int[] hiddenSizes = {25,9};
		    
			MLP network = new MLP(inputsize,hiddenSizes);
			
		    network.write("test.txt");

	     	Matrix<double> xmat = M.DenseOfArray(x);
			
			Debug.Log(network.Forward(xmat));

		    Debug.Log(network.Loss(xmat,0,qtarget));	

			Matrix<double>[] g = network.Gradients(xmat,0,qtarget);
			
			Debug.Log(g[0]);	
			Debug.Log(g[1]);	
			Debug.Log(g[2]);	
			Debug.Log(g[3]);  

			network.update(g);

			Debug.Log(network.W[0]);	
			Debug.Log(network.W[1]);	
			Debug.Log(network.W[2]);	
			Debug.Log(network.W[3]); 

			MLP targetnetwork = new MLP(inputsize,hiddenSizes);
			
		    network.copyTo(targetnetwork);

			Debug.Log(targetnetwork.W[0]);	
			Debug.Log(targetnetwork.W[1]);	
			Debug.Log(targetnetwork.W[2]);	
			Debug.Log(targetnetwork.W[3]); 

			network.W[0][1,2] = 2.0;
			Debug.Log(network.W[0]);	
			Debug.Log(targetnetwork.W[0]);	
			
	    	network.write("testdata.txt");
			network.read("testdata.txt");
			Debug.Log(network.W[0]);	


		}


	}


