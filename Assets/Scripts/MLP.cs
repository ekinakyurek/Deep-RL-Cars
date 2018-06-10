using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

public class MLP {

	public Matrix<double>[] W;

	public static double winit = 0.01;
	public double lr = 0.0005;
	public double lr_decay = 0.95;
	public double lr_end = 0.0001;
	public double lr_endt = 100000000;
	public Matrix<double>[] m; //first moment of gradients
	public Matrix<double>[] v; // second moment of gradients
	public double beta1 = 0.9;
	public double beta2 = 0.999;
	public double eps=1e-8;
	public static double clipnum = 5.0;
	public int t = 0;

	public int inputSize;
	public int outputSize;
	public int[] hiddenSizes;
	public System.Func<Matrix<double>,Matrix<double>>[] activations;
	public System.Func<Matrix<double>,Matrix<double>,Matrix<double>,Matrix<double>>[] activations_b;

	public MatrixBuilder<double> M = Matrix<double>.Build;

	// Use this for initialization
	public MLP (int ninput,int[] hidden) {

		inputSize   = ninput;
		hiddenSizes = hidden;
		outputSize  = hidden[hidden.Length-1];


		int l = 2 * hiddenSizes.Length;
		W = new Matrix<double>[l];
		m = new Matrix<double>[l];
		v = new Matrix<double>[l];


		int x = inputSize;

		activations = new System.Func<Matrix<double>, Matrix<double>>[hidden.Length - 1];
		activations_b = new System.Func<Matrix<double>, Matrix<double>, Matrix<double>, Matrix<double>>[hidden.Length - 1];

		for (var i = 0; i < hidden.Length; i++) {
			if (i != hidden.Length - 1) {
				W [2 * i]         = M.Random (hiddenSizes [i], x).Multiply (winit); 
				W [2 * i + 1]     = M.Dense (hiddenSizes [i], 1); 
				activations [i]   = NNOperations.tanh;
				activations_b [i] = NNOperations.tanh_b;
			} else {
				W [2 * i]     = M.Random (hiddenSizes [i], x); 
				W [2 * i + 1] = M.Dense (hiddenSizes [i], 1); 
			}

			m [2 * i] = M.Dense (hiddenSizes [i], x);
			m [2*i+1] = M.Dense (hiddenSizes[i], 1);
			v [2 * i] = M.Dense (hiddenSizes [i], x);
			v [2*i+1] = M.Dense (hiddenSizes[i], 1);
			x = hiddenSizes [i];
		}
	}

	public void copyTo(MLP network){
		for (var i = 0; i < W.Length; i++) {
			network.W [i] = M.DenseOfMatrix (W [i]);
			network.m [i] = M.DenseOfMatrix (m [i]);
			network.v [i] = M.DenseOfMatrix (v [i]);
		} 	
	}

	public bool write(string filename){

		using (StreamWriter outfile = new StreamWriter(@filename))
		{
			for (int x = 0; x < W.Length; x++)
			{
				string content = "";
				double[] data = W [x].ToColumnWiseArray();
				for (int y = 0; y < data.Length; y++)
				{

					content += data [y].ToString ();
					if (y != data.Length - 1) {
						content += ",";
					}
				}
				//trying to write data to csv
				outfile.WriteLine(content);
			}
		}
		return true;
	}

	public bool read(string filename){

		using (StreamReader reader = new StreamReader(@filename))
		{
			for (int x = 0; x < W.Length; x++)
			{
				string line = reader.ReadLine();
				double[] values = System.Array.ConvertAll (line.Split (','), System.Double.Parse);
				W [x] = M.DenseOfColumnMajor (W [x].RowCount, W [x].ColumnCount, values); 
			}
		}
		return true;
	}

	public void setlr (double LR) {
		//Init Neural Network Weights
		this.lr = LR;
	}


	public Matrix<double>[] similar(){
		Matrix<double>[] sim = new 	Matrix<double>[W.Length];
		for (var i = 0; i < W.Length; i++) {
			sim[i] = M.Dense(W [i].RowCount,W [i].ColumnCount);
		} 
		return sim;
	}

	public static Matrix<double>[] divide(Matrix<double>[] weights, double e){
		Matrix<double>[] div = new 	Matrix<double>[weights.Length];
		for (var i = 0; i < weights.Length; i++) {
			div [i] = weights [i].Divide (e);
		} 
		return div;
	}

	public static void cumulate(Matrix<double>[] cumulation, Matrix<double>[] x){
		for (var i = 0; i < cumulation.Length; i++) {
			cumulation [i] = cumulation [i] + x [i];
		} 
	}

	public static void softupdate(MLP target, MLP qnet, double epsilon){
		for (var i = 0; i < target.W.Length; i++) {
			target.W [i] = target.W [i].Multiply(1.0-epsilon) + qnet.W [i].Multiply(epsilon);
		} 
	}



	public static double clip(double weight){
		return (System.Math.Abs(weight) < MLP.clipnum ? weight : MLP.clipnum * System.Math.Sign(weight));
	}

	public static void gclip(Matrix<double>[] weights){
		for (var i = 0; i < weights.Length; i++) {
			weights [i].MapInplace (clip);
		} 
	}

	public Matrix<double> Forward(Matrix<double> x){

		Matrix<double> input = x; 
		for (var i = 0; i < hiddenSizes.Length; i++) {
			if (i < hiddenSizes.Length - 1) {
				var results = fclayer (activations[i],input, 2 * i);
				input = results [3];
			} else {
				var results = lastlayer (input, 2 * i);
				input = results [2];
			}
		}
		return input;
	}


	public double Loss(Matrix<double> x, int action, double qtarget){
		Matrix<double> x1 = Forward(x);
		Matrix<double>[] loss = losslayer (x1, action, qtarget);
		return loss[loss.Length-1][0,0];
	}

	public Matrix<double>[] Gradients(Matrix<double> x, int action, double qtarget){

		Matrix<double>[][] resArray = new Matrix<double>[hiddenSizes.Length][];
		//Forward
		Matrix<double> input = x; 
		for (var i = 0; i < hiddenSizes.Length; i++) {
			if (i < hiddenSizes.Length - 1) {
				resArray [i] = fclayer (activations[i],input, 2 * i);
				input = resArray [i] [3];
			} else {
				resArray [i] = lastlayer (input, 2 * i);
				input = resArray [i] [2];
			}
		}

		Matrix<double>[] resloss = losslayer (input, action, qtarget);
		Matrix<double> gl        =  lossbackward(resloss);

		Matrix<double>[][] gradArray = new Matrix<double>[hiddenSizes.Length][];
		Matrix<double> gradinput = gl; 
		for (var i = hiddenSizes.Length-1; i >= 0; i--) {
			if (i < hiddenSizes.Length - 1) {
				gradArray[i] = fcbackward (activations_b[i],resArray[i], 2*i, gradinput);
			} else {
				gradArray[i] = lastbackward (resArray[i], 2*i, gradinput);
			}
			gradinput = gradArray [i] [2];
		}

		Matrix<double>[] gradients = new Matrix<double>[W.Length];

		for (var i = 0; i < hiddenSizes.Length; i++) {
			gradients [2 * i] = gradArray [i] [0];
			gradients [2 * i+1] = gradArray [i] [1];
		}

		return gradients;
	}

	public Matrix<double>[] losslayer(Matrix<double> x, int action, double qtarget){
		//Layer1
		Matrix<double> ind = Matrix<double>.Build.Dense (x.RowCount,1);
		ind [action, 0] = 1.0;
		Matrix<double> x1 = x.PointwiseMultiply (ind);
		Matrix<double> ygold = ind.Multiply(qtarget);
		Matrix<double> loss = M.Dense (1, 1, NNOperations.mse (x1,ygold));
		Matrix<double>[] results = { x,ind, x1, ygold, loss };
		return results;
	}

	public Matrix<double> lossbackward(Matrix<double>[] x){
		//Layer1
		Matrix<double>[] gx1_gyg   = NNOperations.mse_b (x[2], x[3], 1.0);
		Matrix<double>[] gx_gind  = NNOperations.ewisemul_b (x[0], x[1], gx1_gyg [0]);
		return gx_gind [0];
	}

	public Matrix<double>[] lastlayer(Matrix<double> x,int i){
		//Layer1
		Matrix<double> x1 = NNOperations.matrixmul (W [i], x);
		Matrix<double> x2 = NNOperations.add (W [i+1], x1);
		Matrix<double>[] results = {x, x1, x2};
		return results;
	}

	public Matrix<double>[] lastbackward(Matrix<double>[] x, int i, Matrix<double> gy){
		//Layer1
		Matrix<double>[] gw1_gx1    = NNOperations.add_b (W[i+1], x[1], gy);
		Matrix<double>[] gw0_gx     = NNOperations.matrixmul_b (W[i], x[0], gw1_gx1[1]);
		Matrix<double>[] gradients = { gw0_gx [0], gw1_gx1 [0], gw0_gx [1]};
		return gradients;
	}

	public Matrix<double>[] fclayer(System.Func<Matrix<double>,Matrix<double>> activation, Matrix<double> x,int i){
		//Layer1
		Matrix<double> x1 = NNOperations.matrixmul (W [i], x);
		Matrix<double> x2 = NNOperations.add (W [i+1], x1);
		Matrix<double> x3 = activation (x2);
		Matrix<double>[] results = {x, x1, x2, x3 };
		return results;
	}

	public Matrix<double>[] fcbackward(System.Func<Matrix<double>,Matrix<double>,Matrix<double>,Matrix<double>> activation_b, Matrix<double>[] x, int i, Matrix<double> gy){
		//Layer1
		Matrix<double>   gx2        = activation_b (x[2], x[3], gy);
		Matrix<double>[] gw1_gx1    = NNOperations.add_b (W[i+1], x[1], gx2);
		Matrix<double>[] gw0_gx     = NNOperations.matrixmul_b (W[i], x[0], gw1_gx1[1]);
		Matrix<double>[] gradients = { gw0_gx [0], gw1_gx1 [0], gw0_gx [1]};
		return gradients;
	}

	public void update(Matrix<double>[] g){

		t += 1;

		for (var i = 0; i < m.Length; i++) {
			m [i] = m [i].Multiply(beta1) + g [i].Multiply (1.0-beta1);
			v [i] = v [i].Multiply(beta2) + g [i].Multiply (1.0-beta2).PointwiseMultiply(g[i]);
		}  

		Matrix<double>[] mhat = new Matrix<double>[m.Length];
		Matrix<double>[] vhat = new Matrix<double>[v.Length];

		for (var i = 0; i < m.Length; i++) {
			mhat [i] = m [i].Divide (1.0 - System.Math.Pow(beta1,t));
			vhat [i] = v [i].Divide (1.0 - System.Math.Pow(beta2,t));
		} 

		for (var i = 0; i < g.Length; i++) {
			W [i] = W [i] - vhat[i].PointwisePower(0.5).Add(eps).DivideByThis(lr).PointwiseMultiply(mhat[i]);
		} 
	}
}
