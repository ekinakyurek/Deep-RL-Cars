using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

public class MLP {

	public Matrix<double>[] W;
	public int inputSize;
	public int outputSize;
	public int hiddenSize;

	// Use this for initialization
	public MLP (int ninput,int hidden,int noutput) {
		//Init Neural Network Weights
		W = new Matrix<double>[4];
		W[0] = Matrix<double>.Build.Random(hidden, ninput); 
		W[1] = Matrix<double>.Build.Dense(hidden, 1); 
		W[2] = Matrix<double>.Build.Random(noutput, hidden); 
		W[3] = Matrix<double>.Build.Dense(noutput, 1); 
		inputSize  = ninput;
		outputSize = noutput;
		hiddenSize = hidden;
	}

	public Matrix<double> Forward(Matrix<double> x){
		//Layer1
		Matrix<double> x1 = NNOperations.matrixmul (W [0], x);
		Matrix<double> x2 = NNOperations.add (W [1], x1);
		Matrix<double> x3 = NNOperations.sigm (x2);
		//Layer2
		Matrix<double> x4 = NNOperations.matrixmul (W [2], x3);
		Matrix<double> x5 = NNOperations.add (W [3], x4);
		return x5;
	}

	public double Loss(Matrix<double> x, Vector<double> ygold){
		Matrix<double> y = Forward (x);
		return NNOperations.mse (y,ygold.ToColumnMatrix());
	}

	public Matrix<double>[] Gradients(Matrix<double> x,Vector<double> ygold){

		//Forward
		Matrix<double> x1 = NNOperations.matrixmul (W [0], x);
		Matrix<double> x2 = NNOperations.add (W [1], x1);
		Matrix<double> x3 = NNOperations.sigm (x2);
		Matrix<double> x4 = NNOperations.matrixmul (W [2], x3);
		Matrix<double> x5 = NNOperations.add (W [3], x4);
		//Loss
		//double loss = NNOperations.mse (x5.Column(0),ygold);
		//Backward
		Matrix<double> ym = ygold.ToColumnMatrix();
		Matrix<double>[] gx5_gym    = NNOperations.mse_b (x5, ym, 1.0);
		Matrix<double>[] gw3_gx4    = NNOperations.add_b (W[3], x4, gx5_gym[0]);
		Matrix<double>[] gw2_gx3    = NNOperations.matrixmul_b (W[2], x3, gw3_gx4[1]);
		Matrix<double> gx2          = NNOperations.sigm_b (x2, x3,  gw2_gx3[1]);
		Matrix<double>[] gw1_gx1    = NNOperations.add_b (W[1], x1, gx2);
		Matrix<double>[] gw0_gx     = NNOperations.matrixmul_b (W[0], x, gw1_gx1[1]);

		Matrix<double> [] gradients = {gw0_gx[0],gw1_gx1[0],gw2_gx3[0],gw3_gx4[0]};

		return gradients;
	}

	public void update(Matrix<double>[] g,double lr){
		for (var i = 0; i < g.Length; i++) {
			W [i] = W [i] - g [i].Multiply (lr);
		} 
	}
}
