using System;
using System.Threading.Tasks;
using UnityEngine;

public class GlobalTools : MonoBehaviour
{
    #region Waiting Tasks
    
    public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
    {
        var waitTask = Task.Run(async () =>
        {
            while (!condition()) await Task.Delay(frequency);
        });

        if (waitTask != await Task.WhenAny(waitTask, 
                Task.Delay(timeout))) 
            throw new TimeoutException();
    }

    public static async Task WaitForSeconds(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
    }

    public static async Task WaitForFrames(int frames)
    {
        await Task.Delay(frames * 1000 / 60);
    }

    #endregion Waiting Tasks

    public static Vector3 GetPointInLine(Vector3 startPos, Vector3 endPos, float t, int shootingDirection = -1)
    {
        Vector3 midPos = new Vector3(startPos.x - shootingDirection * (endPos.x - startPos.x), (startPos.y + endPos.y) / 2, (startPos.z + endPos.z) / 2);

        float x = ((1 - t) * (1 - t) * startPos.x) + (2 * t * (1 - t) * midPos.x) + (t * t * endPos.x);
        float y = ((1 - t) * (1 - t) * startPos.y) + (2 * t * (1 - t) * midPos.y) + (t * t * endPos.y);
        float z = startPos.z + (t * (endPos.z - startPos.z));

        return new Vector3(x, y, z);
    }
}
