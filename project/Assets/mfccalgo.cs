using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class mfccalgo : MonoBehaviour
{

    public enum Estado
    {
        Ins,
        Apn,
        Ex
    }

    // Start is called before the first frame update
    public AudioSource audio;
    
    public bool mic;
    public int qtd_amostras = 512;
    public int qtd_filterbank = 24;
    public float[] spectro;
    public float[] melspectrobank;
    public float[] cepstrum_mfcc;


    public float mfcc6;
    public Estado estado;
    private void Start()
    {

        audio = GetComponent<AudioSource>();
        spectro = new float[qtd_amostras];
        melspectrobank = new float[qtd_filterbank];
        cepstrum_mfcc = new float[qtd_filterbank];


        if (Microphone.devices.Length > 0 && mic == true) {
            Debug.Log(Microphone.devices[0] + " " + Microphone.devices[1] + " " +Microphone.devices[2]);
            audio.clip = Microphone.Start(Microphone.devices[0], true, 1, AudioSettings.outputSampleRate);
            audio.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { }
            audio.Play();
        }

     
    }
    private void Update()
    {
        
        
        audio.GetSpectrumData(spectro, 0, FFTWindow.Hanning);
        Algs.BandFilter(spectro, 0, 4000, audio.clip.frequency);
        Algs.MelFilterBank(spectro, melspectrobank, audio.clip.frequency, qtd_filterbank);
        for (int i = 0; i < melspectrobank.Length; i++) {
            if (melspectrobank[i] != 0) {
                melspectrobank[i] = math.log10(melspectrobank[i]);
            }
        }
        Algs.DCT(melspectrobank, cepstrum_mfcc);
        mfcc6 = cepstrum_mfcc[5];
    }

  
}
