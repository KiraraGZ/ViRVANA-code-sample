using System.Collections.Generic;
using UnityEngine;

public static class PhysicsHelper
{
    public static Vector3 RandomizeDirectionInCone(Vector3 direction, float angle = 30f)
    {
        direction = direction.normalized;

        float randomAngle = Random.Range(-angle / 2f, angle / 2f);
        float randomTilt = Random.Range(0f, angle / 2f);

        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.up) *
                              Quaternion.AngleAxis(randomTilt, Vector3.Cross(direction, Vector3.up));

        return rotation * direction;
    }

    public static Vector3[] GetCircularPositions(int amount, int cycle, float radius, float angleOffset, Transform reference = null)
    {
        Vector3[] positions = new Vector3[amount];
        int[] spawnOrder = GetCycledIndices(amount, cycle);

        for (int i = 0; i < amount; i++)
        {
            int index = spawnOrder[i];
            positions[i] = CalculatePositionInCircle(index, amount, radius, angleOffset, reference);
        }

        return positions;
    }

    private static int[] GetCycledIndices(int amount, int cycleCount)
    {
        if (cycleCount < 1) cycleCount = 1;

        List<int>[] cycles = new List<int>[cycleCount];

        for (int i = 0; i < cycleCount; i++)
        {
            cycles[i] = new List<int>();
        }

        for (int i = 0; i < amount; i++)
        {
            cycles[i % cycleCount].Add(i);
        }

        List<int> orderedIndices = new();
        foreach (var cycle in cycles)
        {
            orderedIndices.AddRange(cycle);
        }

        return orderedIndices.ToArray();
    }

    private static Vector3 CalculatePositionInCircle(int index, int total, float radius, float angleOffset, Transform reference = null)
    {
        float angle = (index * Mathf.PI * 2 / total) + Mathf.Deg2Rad * angleOffset;

        return radius * Mathf.Cos(angle) * (reference != null ? reference.up : Vector3.up) +
               radius * Mathf.Sin(angle) * (reference != null ? reference.right : Vector3.right);
    }
}
