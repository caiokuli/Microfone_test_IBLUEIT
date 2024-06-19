using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public static class Draw
{

    public static int width = 500;
    public static int height = 100;
    public static Color waveformColor = Color.green;
    public static Color col = Color.green;
    public static float sat = .5f;
    


    public static Texture2D DrawWave(AudioClip audio)
    {


        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples * audio.channels]; ;
        float[] waveform = new float[width];
        audio.GetData(samples, 0);
        float packSize = ((float)samples.Length / (float)width);
        int s = 0;
        for (float i = 0; Mathf.RoundToInt(i) < samples.Length && s < waveform.Length; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[Mathf.RoundToInt(i)]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.black);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, col);
                tex.SetPixel(x, (height / 2) - y, col);
            }
        }
        tex.Apply();

        return tex;
       


    }
}
