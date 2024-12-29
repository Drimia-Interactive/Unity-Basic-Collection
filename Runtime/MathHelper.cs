using System;
using Random = UnityEngine.Random;

namespace DrimiaInteractive
{
	public static class MathHelper
	{
		public static int GetRandomWeightedIndex(float[] weights)
		{
			if (weights == null || weights.Length == 0) return -1;

			float w;
			float t = 0;
			int i;
			for (i = 0; i < weights.Length; i++)
			{
				w = weights[i];
				if (float.IsPositiveInfinity(w)) return i;
				else if (w >= 0f && !float.IsNaN(w)) t += weights[i];
			}

			float r = Random.value;
			float s = 0f;

			for (i = 0; i < weights.Length; i++)
			{
				w = weights[i];
				if (float.IsNaN(w) || w <= 0f) continue;

				s += w / t;
				if (s >= r) return i;
			}

			return -1;
		}

		public class WeightedObjectsGroup
		{
			public float[] Weights;
			private float[] startingWeights;

			private float maxWeight = -1;
			private float[] maxWeights;
			private bool isMaxWeightUsed = false;

			private float weightChange;
			private float[] weightsChanges;

			public WeightedObjectsGroup(float[] weights, float weightChange = 1f, float maxWeight = -1f)
			{
				startingWeights = new float[weights.Length];
				Array.Copy(weights, startingWeights, weights.Length);
				Weights = weights;
				this.weightChange = weightChange;
				this.maxWeight = maxWeight;
				isMaxWeightUsed = maxWeight > 0;
			}

			public WeightedObjectsGroup(float[] weights, float[] weightsChanges, float[] maxWeights = null) : this(weights, -1f)
			{
				this.weightsChanges = weightsChanges;
				this.maxWeights = maxWeights;
				isMaxWeightUsed = maxWeights != null;
			}

			public int GetRandomIndex()
			{
				int index = -1;

				if (isMaxWeightUsed)
				{
					// get all weights in their maximum values
					bool isWeightInMax = false;
					float[] weightsInMax = new float[Weights.Length];
					for (int i = 0; i < Weights.Length; i++)
					{
						var maxW = maxWeight == -1 ? maxWeights[i] : maxWeight;
						if (Weights[i] >= maxW)
						{
							isWeightInMax = true;
							weightsInMax[i] = Weights[i];
						}
					}

					if (isWeightInMax)
					{
						index = GetRandomWeightedIndex(weightsInMax);
					}
				}

				// get the index
				if (index == -1)
				{
					index = GetRandomWeightedIndex(Weights);
				}

				if (index == -1)
					index = Random.Range(0, Weights.Length);

				// update weights
				for (int i = 0; i < Weights.Length; i++)
				{
					if (i == index)
						Weights[i] = startingWeights[i];
					else
					{
						var change = weightChange == -1 ? weightsChanges[i] : weightChange;
						Weights[i] += change;
					}
				}

				return index;
			}
		}

		public class WeightedObjectsGroup<T> : WeightedObjectsGroup
		{
			public T[] WeightedObjects;

			public WeightedObjectsGroup(float[] weights, T[] weightedObjects, float weightChange = 1f, float maxWeight = -1f) : base(weights, weightChange, maxWeight)
			{
				WeightedObjects = weightedObjects;
			}

			public WeightedObjectsGroup(float[] weights, float[] weightsChanges, T[] weightedObjects, float[] maxWeights = null) : base(weights, weightsChanges, maxWeights)
			{
				WeightedObjects = weightedObjects;
			}

			public T GetRandomObject()
			{
				return WeightedObjects[GetRandomIndex()];
			}
		}
	}
}