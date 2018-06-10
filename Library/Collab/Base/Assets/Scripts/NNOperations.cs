using System.Collections;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

public static class NNOperations{

	//Scalar Functions

	public static double relu(double x){
		return System.Math.Max(0.0, x);
	}

	public static double relu_b(double x){
		if (x > 0.0) {
			return 1.0;
		}else{
			return 0.0;
		}
	}

	public static double abs_b(double x){
		if (x > 0.0) {
			return 1.0;
		}else{
			return -1.0;
		}
	}

	public static double sigm(double x){
		return 1.0 / (1.0 + System.Math.Exp (-x));
	}

	public static double sigm_b(double x,double y,double gy){
		return y * (1 - y) * gy;
	}


	public static double tanh(double x){
		return System.Math.Tanh(x);
	}

	public static double tanh_b(double x,double y,double gy){
		return (1 - System.Math.Pow (y, 2)) * gy;
	}

	//Matrix Operations

	public static Matrix<double> relu(Matrix<double> x)
	{
		return x.Map (relu); 
	}

	public static Matrix<double> relu_b(Matrix<double> x, Matrix<double> y, Matrix<double> gy)
	{
		return x.Map (relu_b).PointwiseMultiply(gy); 
	}

	public static Matrix<double> sigm(Matrix<double> x)
	{
		return x.Map (sigm); 
	}

	public static Matrix<double> sigm_b(Matrix<double> x, Matrix<double> y, Matrix<double> gy)
	{
		return y.SubtractFrom(1.0).PointwiseMultiply (y).PointwiseMultiply (gy);
	}

	public static Matrix<double> tanh(Matrix<double> x)
	{
		return x.Map (tanh); 
	}

	public static Matrix<double> tanh(Matrix<double> x, Matrix<double> y, Matrix<double> gy)
	{
		return y.PointwisePower (2.0).SubtractFrom (1.0).PointwiseMultiply (gy); 
	}

	public static Matrix<double> exp(Matrix<double> x)
	{
		return x.PointwiseExp();
	}

	public static Matrix<double> exp_b(Matrix<double> x, Matrix<double> y, Matrix<double> gy)
	{
		return y.PointwiseMultiply(gy);
	}

	public static Matrix<double> log(Matrix<double> x)
	{
		return x.PointwiseLog ();
	}

	public static Matrix<double> log_b(Matrix<double> x,Matrix<double> y, Matrix<double> gy)
	{
		return x.DivideByThis (1.0).PointwiseMultiply (gy);
	}

	public static Matrix<double> add(Matrix<double> x,Matrix<double> y)
	{
		return x + y;
	}

	public static Matrix<double>[] add_b(Matrix<double> x,Matrix<double> y, Matrix<double> gz)
	{
		Matrix<double>[] g = { gz, gz };
		return g;
	}

	public static Matrix<double> subtract(Matrix<double> x,Matrix<double> y)
	{
		return x-y;
	}

	public static Matrix<double>[] subtract_b(Matrix<double> x,Matrix<double> y, Matrix<double> gz)
	{
		Matrix<double>[] g = { gz, -gz};
		return g;
	}

	public static Matrix<double> matrixmul(Matrix<double> x, Matrix<double> y)
	{
		return x * y;
	}

	public static Matrix<double>[] matrixmul_b(Matrix<double> x,Matrix<double> y, Matrix<double> gz)
	{
		Matrix<double>[] g = {gz*y.Transpose(),x.Transpose()*gz};
		return g;
	}

	public static Matrix<double> ewisemul(Matrix<double> x, Matrix<double> y)
	{
		return x.PointwiseMultiply(y);
	}

	public static Matrix<double>[] ewisemul_b(Matrix<double> x, Matrix<double> y, Matrix<double> gz)
	{

		Matrix<double>[] g = {gz.PointwiseMultiply(y),gz.PointwiseMultiply(x)};
		return g;
	}

	public static double sum(Matrix<double> x)
	{
		return x.RowSums ().Sum ();
	}

	public static Matrix<double> sum_b(Matrix<double> x, double y)
	{
		return Matrix<double>.Build.Dense(x.RowCount,x.ColumnCount,y);
	}

	public static Matrix<double> abs(Matrix<double> x)
	{
		return x.Map(System.Math.Abs);
	}

	public static Matrix<double> abs_b(Matrix<double> x,Matrix<double> y, Matrix<double> gy)
	{
		return x.Map(abs_b);
	}

	public static Matrix<double> softmax(Matrix<double> x)
	{
		return exp (x).NormalizeColumns(double.PositiveInfinity);
	}

	public static Matrix<double> logp(Matrix<double> x)
	{
		return log(softmax(x));
	}

	public static double mse(Matrix<double> y1,Matrix<double> y2)
	{
		return (y1 - y2).ColumnNorms (2.0).Sum ();
	}

	public static  Matrix<double>[]  mse_b(Matrix<double> y1,Matrix<double> y2, double gz)
	{
		Matrix<double> dj = (y1 - y2).Multiply (2.0 * gz);
		Matrix<double>[] djs = { dj, -dj };
		return djs;
	}

	//Vector Operations
	//TO-DO Vector Operations Backward
	public static Vector<double> relu(Vector<double> x)
	{
		return x.Map (relu); 
	}

	public static Vector<double> sigm(Vector<double> x)
	{
		return x.Map (sigm); 
	}

	public static Vector<double> tanh(Vector<double> x)
	{
		return x.Map (tanh); 
	}

	public static Vector<double> exp(Vector<double> x)
	{
		return x.PointwiseExp(); 
	}

	public static Vector<double> abs(Vector<double> x)
	{
		return x.Map (System.Math.Abs); 
	}

	public static Vector<double> log(Vector<double>  x)
	{
		return x.PointwiseLog ();
	}

	public static Vector<double> add(Vector<double> x,Vector<double> y)
	{
		return x+y;
	}

	public static Vector<double> subtract(Vector<double> x,Vector<double> y)
	{
		return x-y;
	}

	public static Vector<double> ewisemul(Vector<double> x,Vector<double> y)
	{
		return x.PointwiseMultiply (y);
	}

	public static double sum(Vector<double> x)
	{
		return x.Sum();
	}

	public static Vector<double> softmax(Vector<double> x)
	{
		return exp (x).Normalize (double.PositiveInfinity);
	}

	public static Vector<double> logp(Vector<double> x)
	{
		return log(softmax(x));
	}

	public static double mse(Vector<double> y1,Vector<double> y2)
	{
		return (double)(y1 - y2).Norm (2.0);
	}

	public static  Vector<double>[]  mse_b(Vector<double> y1,Vector<double> y2, double gz)
	{
		Vector<double> dj = (y1 - y2).Multiply(2.0*gz);
		Vector<double>[] g = { dj, -dj };
		return g;

	}
}
