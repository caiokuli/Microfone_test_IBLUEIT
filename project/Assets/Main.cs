using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Mathematics;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Main : MonoBehaviour
{

    public AudioSource audiosource;
    public GameObject[] CalibrarUi = new GameObject[5];
    public int qtd_filterbank;
    public int qtd_amostras;
    public static float[] spectro;
    public float[] melspectrobank;
    public float[] cepstrum_mfcc;
    public float mfcc6;
    public float taxaAmostragem;
    public FFTWindow fftwindow;



    [SerializeField] public Image imgs;

    void Start()
    {
        

        iniciaVariavel();
        iniciaMic();
        iniciaUI();

    }

    void Update()
    {
        //  spectro amplitude =  spectro magnetude\
        //   DFT se quiser melhorar a escala precisa dividir por quantidade de amonstras X(k) = 1/N * |X(k)| ou comum tbm 1/sqrt(N)* |X(k)| (Lyons 2010)
        //https://medium.com/giant-scam/algorithmic-beat-mapping-in-unity-real-time-audio-analysis-using-the-unity-api-6e9595823ce4

        drawWaveform();

        // MFCC E SPECTRUM
        audiosource.GetSpectrumData(spectro, 0, fftwindow);
        //filtros

        if (GlobalConfig.Instance.tipoFiltro != GlobalConfig.TipoFiltro.SemFiltro)
        {
            // Inicializa o filtro com base no tipo selecionado
            switch (GlobalConfig.Instance.tipoFiltro)
            {
                case GlobalConfig.TipoFiltro.Filtrofft:
                    Algs.BandFilter(spectro,GlobalConfig.Instance.faixaFrequencia.Min, GlobalConfig.Instance.faixaFrequencia.Max, audiosource.clip.frequency);
                    break;

                case GlobalConfig.TipoFiltro.FiltroUnity:
                    // Configura e aplica um filtro disponível pela Unity com as faixas de frequência especificadas
                    GetComponent<AudioLowPassFilter>().cutoffFrequency = GlobalConfig.Instance.faixaFrequencia.Max;
                    GetComponent<AudioLowPassFilter>().enabled = true;
                    GetComponent<AudioHighPassFilter>().cutoffFrequency = GlobalConfig.Instance.faixaFrequencia.Min;
                    GetComponent<AudioHighPassFilter>().enabled = true;

                    break;

                case GlobalConfig.TipoFiltro.FiltroButterworth:
                    // Configura e aplica o filtro Butterworth com as faixas de frequência especificadas
                    spectro = Algs.Butterworth(  spectro, taxaAmostragem, GlobalConfig.Instance.faixaFrequencia.Max);
                    break;

                // Adicione mais casos conforme necessário para outros tipos de filtro
            }
        }        



        //Algs.BandFilter(spectro,2200, 3900, audiosource.clip.frequency);
        //spectro = Algs.Butterworth(  spectro, taxaAmostragem, 4000);
        
        
        //MFCC6.vad_flag = Algs.VADTEST(spectro); // 1 respiracao -> 0 ruido
        //Debug.Log(audiosource.clip.frequency);
        
        if (GlobalConfig.Instance.melFilterBankType == "ClassicMelFilterBank")
        {
        Algs.ClassicMelFilterBank(spectro, melspectrobank, (int)taxaAmostragem, qtd_filterbank);
        }
        else
        {
        Algs.AlternativeMelFilterBank(spectro, melspectrobank, (int)taxaAmostragem, qtd_filterbank);
        }

        Algs.MelSpectrum_LogScale(melspectrobank);
        Algs.DCT(melspectrobank, cepstrum_mfcc);
        mfcc6 = cepstrum_mfcc[6];
        MFCC6.Setvalue(mfcc6); // update constante do sexto mfcc
        IfCalibrar();

    }

    // Update is called once per frame
    // START
    void iniciaVariavel() {
        
        Debug.Log("QUANTIDADE AQUI AA : " + GlobalConfig.Instance.qtdAmostras);
        qtd_amostras = GlobalConfig.Instance.qtdAmostras;
        //qtd_amostras = 1024;
        taxaAmostragem = GlobalConfig.Instance.taxaAmostragem;
        AudioSettings.outputSampleRate = (int) taxaAmostragem;
        fftwindow = GlobalConfig.Instance.fftWindow;
        qtd_filterbank = GlobalConfig.Instance.quantidadeFilterbank;



        
        audiosource = GetComponent<AudioSource>();
        spectro = new float[qtd_amostras];
        melspectrobank = new float[qtd_filterbank];
        cepstrum_mfcc = new float[qtd_filterbank];

    }
    void iniciaMic() {
        int number_mic = 0;
        foreach (var item in Microphone.devices)
        {
            Debug.Log(item);    
        } 
        
        audiosource.clip = Microphone.Start(Microphone.devices[number_mic], true, 1, (int) taxaAmostragem); //AudioSettings.outputSampleRate
        audiosource.loop = true;
        audiosource.bypassEffects = false;
        while (!(Microphone.GetPosition(Microphone.devices[number_mic]) > 0)) { }
        audiosource.Play();
        
        //'taxaAmostragem = audiosource.clip.frequency;
    }

    void iniciaUI() {
        foreach (GameObject ui in CalibrarUi)
        {
            ui.SetActive(false);

        }
    }

    // UPDATE

    void drawWaveform() {
     
        Texture2D tex = Draw.DrawWave(audiosource.clip);
        imgs.overrideSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    void IfCalibrar() {
        //int vad = Algs.VADTEST(spectro); // nem sempre ok
        bool flag = MFCC6.GetFlag(); // ok
        //Debug.Log("Flag: " + flag);
        //Debug.Log("Vad: " + vad);

        if (flag /*&&  vad == 1*/)
        { // se true MFCC sera usado para calibrar

            lock (MFCC6.lockMFCC6)
            { // lock para courotine
                MFCC6.UpdateCont();
                MFCC6.MenorMfcc6();
                MFCC6.MaiorMfcc6();
                MFCC6.UpdateVetor();
            }
        }


    }

}




