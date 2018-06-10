//#define TRACK1
//#define TRACK2
#define TRACK3
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

public class Memory
{

	public int Limit;

	public List<Entry> Entries;

	public int count = 0;

	public int stateSize;

	public System.Random rnd;

	#if TRACK1 || TRACK3

		public double[] r_alphas = {1.0, 2.0, 1.0};

	#endif

	#if TRACK2

		public double[] r_alphas = {1.0, 5.0, 5.0};

	#endif

	public double euclideanDistance;


	public Memory (int limit, System.Random r, int size)
	{
		Entries = new List<Entry> ();
		rnd = r;
		stateSize = size;
		Limit = limit;
	}

	public void Enqueue(Matrix<double> s, int action, double speed, double dist) 
	{
		Entry e = new Entry ();
		e.state = s;
		e.action = action;
		e.speed = speed;
		e.dist = dist;

		Entries.Add (e);
		count++;

		while (count > Limit) {
			Entries.RemoveAt (0);
			count--;
		}
	}

	public void Enqueue(Matrix<double> s, int action, double speed, double dist_surroundings, double dist_to_cube) 
	{
		Entry e = new Entry ();
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

	public List<Entry> Sample(int size)
	{
		double cumReward = 0.0;
		List<Entry> l = new List<Entry> (size);
		int[] arr = GetRandomIndices (size);
		foreach (int i in arr) {
			Entry e = Entries [i];
			e.nextState = Entries [i + 1].state;
			e.reward = ComputeReward (e, Entries[i+1]);
			l.Add (e);
			cumReward += e.reward;
		}
		Debug.Log ("Reward: " + cumReward/size);
		return l;
	}

	#if TRACK1 || TRACK3

	public double ComputeReward(Entry current, Entry next)
	{ 
		double speedDiff = next.speed - current.speed;
		double distDiff = next.dist - current.dist;
		double s = next.speed < 0.2 ? (next.speed - 1) : next.speed;
//		s = next.speed > 0.5 ? next.speed : 0.0;
		return speedDiff * r_alphas [0] + distDiff * r_alphas [1] + s * r_alphas[2];

	}

	#endif

	#if TRACK2

	public double ComputeReward(Entry current, Entry next)
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

	#endif


	public double LastReward()
	{
		if (count > 1) {
			Entry next = Entries [count - 1];
			Entry current = Entries [count - 2];
			return ComputeReward (current, next);
		}
		return 0.0;
	}
			
	public int[] GetRandomIndices(int size)
	{
		return Enumerable.Range(0,count-1).OrderBy(i => rnd.NextDouble()).Take(size).ToArray();
	}

	public static void saveGraphs(List<double> returnValues, List<int> lifeTime, string filename){

		using (StreamWriter outfile = new StreamWriter(@filename))
		{
			string content = "";
			for (int y = 0; y < returnValues.Count; y++)
			{
				content += returnValues[y].ToString ();
				if (y != returnValues.Count - 1) {
						content += ",";
				}
			}
			outfile.WriteLine(content);

			content = "";
			for (int y = 0; y < lifeTime.Count; y++)
			{
				content += lifeTime[y].ToString ();
				if (y != lifeTime.Count - 1) {
					content += ",";
				}
			}
			outfile.WriteLine(content);
		}
	}
}