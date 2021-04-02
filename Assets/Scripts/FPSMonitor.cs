using UnityEngine;

public class FPSMonitor : MonoBehaviour
{
    public bool isDisplay = true;
    public float updateInterval = 1f;
    private double lastInterval;
    private int frames = 0;
    private float fps = 0f;
    private int currentFrames = 0;

    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    void Update()
    {
        ++frames;
        ++currentFrames;

        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }
    private void OnGUI()
    {
        if(!isDisplay)
            return;
        var groupWidth = 60;
        var groupHeight = 30;
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        var nFPS = (int) (fps + 0.5f);
        var color = "#00FF00";
        if (nFPS < 25)
        {
            color = nFPS < 15 ? "red" : "yellow";
        }
        var text = string.Format("<color={1}><size=20> {0} </size></color>", nFPS,color);
        GUI.Label(new Rect(screenWidth-groupWidth,10,groupWidth,groupHeight), text);

    }
    public float GetCurrentFPS()
    {
        return fps;
    }

    public int GetCurrentFrames()
    {
        return currentFrames;
    }

    public void Reset()
    {
        currentFrames = 0;
    }
}