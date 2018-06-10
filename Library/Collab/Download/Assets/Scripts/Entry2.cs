using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

public class Entry2
{
	public Matrix<double> state { get; set;} //not sure set works
	public int action { get; set;}
	public bool crash{ get; set;}
	public Matrix<double> nextState { get; set;}
	public double speed{ get; set;}
	public double dist_surroundings{ get; set;}
	public double dist_to_cube{ get; set;}
	public double reward{ get; set;}
}



