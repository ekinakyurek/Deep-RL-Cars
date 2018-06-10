using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

public class Memory2
{

	public int Limit;

	public List<Entry2> Entries;

	public int count = 0;

	public int stateSize;

	public System.Random rnd;

	public double[] r_alphas = {1.0, 5.0, 5.0};

	public double euclideanDistance;

	public Memory2 (int limit, System.Random r, int size)
	{
		Entries = new List<Entry2> ();
		rnd = r;
		stateSize = size;
		Limit = limit;
//		this.euclideanDistance = euclidean;
	}

	public void Enqueue(Matrix<double> s, int action, double speed, double dist_surroundings, double dist_to_cube) 
	{
		Entry2 e = new Entry2 ();
		e.state = s;
		e.action = action;
		e.speed = speed;
		e.dist_to_cube = dist_to_cube;
		e.dist_surroundings = dist_surroundings;

		Entries.Add (e);
		count++;

		while (count > Limit) {
			Entries.RemoveAt (0);
			count--;
		}
	}

	public void RecordCrash()
	{
		//		Debug.Log ("Count " + count);
		//		Debug.Log ("Len " + Entries.Count);
		Entries[count - 1].crash = true;
		//		Debug.Log ("CRASH" + (count - 1));
	}

	public List<Entry2> Sample(int size)
	{
		double cumReward = 0.0;
		List<Entry2> l = new List<Entry2> (size);
		int[] arr = GetRandomIndices (size);
		foreach (int i in arr) {
			Entry2 e = Entries [i];
			e.nextState = Entries [i + 1].state;
			e.reward = ComputeReward (e, Entries[i+1]);
			l.Add (e);
			cumReward += e.reward;
		}
		Debug.Log ("Reward: " + cumReward/size);
		return l;
	}

	public double ComputeReward(Entry2 current, Entry2 next)
	{ 
		double speedDiff = next.speed - current.speed;
		double distDiff = next.dist_surroundings - current.dist_surroundings; //System.Math.Max(next.dist - euclideanDistance, 0.0);
//		double cubeDistDiff = next.dist_to_cube;
//		double b =  (cubeDistDiff >= 3 && cubeDistDiff <= 4) ? 2.0 : 0.0; //to map 0.5 to 1
//		if (cubeDistDiff > 3.5) {
//			cubeDistDiff = 0.5 - (cubeDistDiff - 3.5);
//		} else {
//			cubeDistDiff = 0.5 - (3.5 - cubeDistDiff);
//		}
//		Debug.Log ("Cube Dist Diff: " + cubeDistDiff);
//		double s = next.speed < 0.2 ? (next.speed - 1) : 0.0;
		//		s = next.speed > 0.5 ? next.speed : 0.0;
		return speedDiff * r_alphas [0] + distDiff * r_alphas [1];// + b*cubeDistDiff * r_alphas[2];

	}

	public int[] GetRandomIndices(int size)
	{
		return Enumerable.Range(0,count-1).OrderBy(i => rnd.NextDouble()).Take(size).ToArray();
	}
}