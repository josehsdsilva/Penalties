using System;
using System.Threading.Tasks;

public class GlobalTools
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

    public static async Task WaitForSeconds(int seconds)
    {
        await Task.Delay(seconds * 1000);
    }

    public static async Task WaitForFrames(int frames)
    {
        await Task.Delay(frames * 1000 / 60);
    }

    #endregion Waiting Tasks
}
