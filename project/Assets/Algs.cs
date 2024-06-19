using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;

/*
static class VadClass{

    public static double Energy= 0;
    public static double ShortEnergy = 0;
    public static double LongeEnergy = 0;
    
    public static double alpha_short = 1 / 16f;
    public static double alpha_long = 1 / 128f;

    public static double NoiseFloor = 0;
    public static double Threshold = 0;
    public static double beta = 0.00009; // safety margin   // 0.0004 valor razoavel para falas // 0.00009
    public static int Hangover;
    public static int HangTime = 9;
}
*/
public static class MFCC6{
    public static object lockMFCC6 = new object();
    static float instante_value = 0;
    static uint cont = 0;

    static float sum_value = 0;
    static bool flag = false;
    public static float mfcc_in = 0;
    public static float mfcc_ex = 0;
    //public static int vad_flag;

    public static float menor_valor = 0;
    public static float maior_valor = 0;
    
    public static float menor_valor_ex = 0;
    public static float maior_valor_ex = 0;
    public static float menor_valor_ins = 0;
    public static float maior_valor_ins = 0;

    public static float mediana_ins = 0;
    public static float mediana_ex = 0;

    public static List<float> vetorDeMfcc6 = new List<float>();


 

    public static void SetFlag(bool tflag) {
        flag = tflag;
    }

    public static bool GetFlag() {
        return flag;
    }
    
    public static void Setvalue(float v) {
        instante_value = v;
    }
    public static void UpdateCont() {
            cont++;
            
        sum_value = sum_value+ instante_value; // nao vai ter concorencia pois unity eh single thread a nao ser q eu mande alguma thread (coroutina nao eh muito thread)
    }

    public static float MediaMfcc6() {
        float r = (float) sum_value / cont;
        Debug.Log(r);
        Debug.Log("SUM VALUE " +sum_value + "\nCONT " + cont  +"\nMEDIA MFCC6 "+r);
        return r;

    }

    public static void MaiorMfcc6() {
        if(instante_value >= maior_valor)
        {
        maior_valor = instante_value;    
        }

    }

    public static void ResetMaior(){
        maior_valor = 0;

    }


    public static void MenorMfcc6() {
        if(instante_value <= menor_valor)
        {
        menor_valor = instante_value;    
        }

    }

    public static void ResetMenor(){
        menor_valor = 0;

    }

    public static void ResetCont() {
        cont = 0;
        sum_value = 0;
    }

    public static float Getvalue() {
        return instante_value;
    }

    public static void UpdateVetor(){
        vetorDeMfcc6.Add(instante_value);
    }

    public static float MedianaMfcc6()
    {
            // Ordena a lista temporariamente para calcular a mediana
            var listaOrdenada = vetorDeMfcc6.OrderBy(n => n).ToList();
            int tamanho = listaOrdenada.Count;

            // Verifica se a lista está vazia
            if (tamanho == 0)
            {
                throw new InvalidOperationException("Não é possível calcular a mediana de uma lista vazia.");
            }

            // Calcula a mediana
            if (tamanho % 2 == 0) // Par
            {
                // Para lista par, a mediana é a média dos dois elementos do meio
                float meioInferior = listaOrdenada[(tamanho / 2) - 1];
                float meioSuperior = listaOrdenada[tamanho / 2];
                return (meioInferior + meioSuperior) / 2;
            }
            else // Ímpar
            {
                // Para lista ímpar, a mediana é o elemento do meio
                return listaOrdenada[tamanho / 2];
            }
    }




    public static void ResetVetor()
    {
        vetorDeMfcc6.Clear();
    }

}



public static class Algs 
{
    public static float ToMel(float hz)
    {
        return 1127.010480f * math.log(hz / 700f + 1f);
    }

    public static float ToHz(float mel)
    {
        return 700f * (math.exp(mel / 1127.010480f) - 1f);
    }


    public static void BandFilter(float[] spectrum, float baixa, float alta, float audio_samplerate)
    {

        float range_frequencia = audio_samplerate / (2 * spectrum.Length); //divisao por no teorema de nychist
        float f = range_frequencia;
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (f < baixa + range_frequencia / 2 || alta - range_frequencia / 2 < f)
            {
                spectrum[i] = 0;
            }


            f = f + range_frequencia;
        }

    }


    public static float[] Butterworth(float[] indata, float taxa_amostragem, float CutOff)//lowww pass butterworth
                                                                                          //https://apps.dtic.mil/sti/pdfs/AD1060538.pdf
    {
        if (indata == null) return null;
        if (CutOff == 0) return indata;
        float Samplingrate =   taxa_amostragem;
        long dF2 = indata.Length - 1; // The data range is set with dF2
        float[] Dat2 = new float[dF2 + 4]; // Array with 4 extra points front and back
        float[] data = indata; // Ptr., changes passed data
        for (long r = 0; r < dF2; r++)
        {
            Dat2[2 + r] = indata[r];
        }
        Dat2[1] = Dat2[0] = indata[0];
        Dat2[dF2 + 3] = Dat2[dF2 + 2] = indata[dF2];
        const float pi = (float)3.14159265358979;
        float wc = (float)Math.Tan(CutOff * pi / Samplingrate);
        float k1 = (float)1.414213562 * wc; // Sqrt(2) * wc
        float k2 = wc * wc;
        float a = k2 / (1 + k1 + k2);
        float b = 2 * a;
        float c = a;
        float k3 = b / k2;
        float d = -2 * a + k3;
        float e = 1 - (2 * a) - k3;
        // RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
        float[] DatYt = new float[dF2 + 4];
        DatYt[1] = DatYt[0] = indata[0];
        for (long s = 2; s < dF2 + 2; s++)
        {
            DatYt[s] = a * Dat2[s] + b * Dat2[s - 1] + c * Dat2[s - 2]
            + d * DatYt[s - 1] + e * DatYt[s - 2];
        }
        DatYt[dF2 + 3] = DatYt[dF2 + 2] = DatYt[dF2 + 1];
        // FORWARD filter
        float[] DatZt = new float[dF2 + 2];
        DatZt[dF2] = DatYt[dF2 + 2];
        DatZt[dF2 + 1] = DatYt[dF2 + 3];
        for (long t = -dF2 + 1; t <= 0; t++)
        {
            DatZt[-t] = a * DatYt[-t + 2] + b * DatYt[-t + 3] + c * DatYt[-t + 4]
            + d * DatZt[-t + 1] + e * DatZt[-t + 2];
        }
        // Calculated points are written
        for (long p = 0; p < dF2; p++)
        {
            data[p] = DatZt[p];
        }
        return data;
    }


    public static void AlternativeMelFilterBank(float[] spectrum, float[] melSpectrum, float sampleRate, int melBands) {
    // Determina a frequência máxima baseada na configuração global
    float fMax = GlobalConfig.Instance.frequenciaMaxMel;
    // Converte a frequência máxima para a escala de Mel
    float melMax = ToMel(fMax);

    // Define o número máximo de índices com base no espectro
    int nMax = spectrum.Length / 2;
    // Calcula o intervalo de frequência entre os pontos do espectro
    float df = sampleRate / (2 * nMax);
    // Determina o intervalo Mel entre os filtros
    float dMel = melMax / (melBands + 1);

    for (int m = 1; m <= melBands; m++) {
        // Define os pontos centrais dos filtros na escala de Mel
        float melCenter = m * dMel;
        float melLower = (m - 1) * dMel;
        float melUpper = (m + 1) * dMel;

        // Converte os pontos Mel de volta para a frequência em Hz
        float fCenter = ToHz(melCenter);
        float fLower = ToHz(melLower);
        float fUpper = ToHz(melUpper);

        // Calcula os índices correspondentes no espectro
        int indexCenter = (int)Math.Round(fCenter / df);
        int indexLower = (int)Math.Round(fLower / df);
        int indexUpper = (int)Math.Round(fUpper / df);

        float energy = 0f;
        for (int i = indexLower; i < indexUpper; i++) {
            float filter = 0f;
            // Calcula o valor do filtro triangular
            if (i < indexCenter) {
                filter = (i - indexLower) / (float)(indexCenter - indexLower);
            } else {
                filter = (indexUpper - i) / (float)(indexUpper - indexCenter);
            }
            // Aplica o filtro ao espectro e acumula a energia
            energy += filter * spectrum[i];
        }
        // Armazena a energia acumulada no espectro Mel
        melSpectrum[m - 1] = energy;
    }
}



    public static void ClassicMelFilterBank(float[] spectrum, float[] melSpectrum, float sampleRate, int melDiv) {
   
  
        float fMax = GlobalConfig.Instance.frequenciaMaxMel;
        float melMax = ToMel(fMax);
        int nMax = spectrum.Length / 2;
        float df = fMax / nMax;
        float dMel = melMax / (melDiv + 1);

        for (int n = 0; n < melDiv; ++n)
        {
            float melBegin = dMel * n;
            float melCenter = dMel * (n + 1);
            float melEnd = dMel * (n + 2);

            float fBegin = ToHz(melBegin);
            float fCenter = ToHz(melCenter);
            float fEnd = ToHz(melEnd);

            int iBegin = (int)math.round(fBegin / df);
            int iCenter = (int)math.round(fCenter / df);
            int iEnd = (int)math.round(fEnd / df);

            float sum = 0f;
            for (int i = iBegin + 1; i < iEnd; ++i)
            {
                float a = (i < iCenter) ? ((float)i / iCenter) : ((float)(i - iCenter) / iCenter);
                sum += a * spectrum[i];
                //Debug.Log("N: " + n + "  I: " + i + "\n" + "a: " + a + "spectroi : " + spectrum[i] + "\n" + "sum " + sum);

            }
            melSpectrum[n] = sum;
        }


    }


    public static void MelFilterBank(float[] spectrum, float[] melSpectrum, float sampleRate, int melDiv)
    {


        float fMax = sampleRate / 2;
        float melMax = ToMel(fMax);
        int nMax = spectrum.Length / 2;
        float df = fMax / nMax;
        float dMel = melMax / (melDiv + 1);

        for (int n = 0; n < melDiv; ++n)
        {
            float melBegin = dMel * n;
            float melCenter = dMel * (n + 1);
            float melEnd = dMel * (n + 2);

            float fBegin = ToHz(melBegin);
            float fCenter = ToHz(melCenter);
            float fEnd = ToHz(melEnd);

            int iBegin = (int)math.round(fBegin / df);
            int iCenter = (int)math.round(fCenter / df);
            int iEnd = (int)math.round(fEnd / df);

            float sum = 0f;
            for (int i = iBegin + 1; i < iEnd; ++i)
            {
                float a = (i < iCenter) ? ((float)i / iCenter) : ((float)(i - iCenter) / iCenter);
                sum += a * spectrum[i];
                //Debug.Log("N: " + n + "  I: " + i + "\n" + "a: " + a + "spectroi : " + spectrum[i] + "\n" + "sum " + sum);

            }
            melSpectrum[n] = sum;
        }
    }


    public static void DCT(float[] spectrum, float[] cepstrum)
    {
        int N = spectrum.Length;

        float a = math.PI / N;
        for (int i = 0; i < N; ++i)
        {
            float sum = 0f;
            for (int j = 0; j < N; ++j)
            {
                float ang = (j + 0.5f) * i * a;
                
                sum += spectrum[j] * math.cos(ang);
            }
            cepstrum[i] = sum;//*math.sqrt(2f/13f);
        }
    }

    public static void MelSpectrum_LogScale(float[] melspectrobank) {
        for (int i = 0; i < melspectrobank.Length; i++)
        {
            if (melspectrobank[i] != 0)
            {
                melspectrobank[i] = math.log10(melspectrobank[i]);
            }
        }

    }
    
    /*
    public static int VADTEST(float[] spectrum) { // abusshakra e fazipour 
        int flag;
        VadClass.Energy = 0;
        int i = 0;
        while (spectrum[i] != 0) {
            VadClass.Energy = VadClass.Energy + (spectrum[i] * spectrum[i]) ;
            i++;
        }
        //Debug.Log("Energy" + VadClass.Energy);
        VadClass.ShortEnergy = ((1 - VadClass.alpha_short) * VadClass.ShortEnergy) + VadClass.alpha_short * VadClass.Energy;
        //Debug.Log("alpha:" + VadClass.alpha_short + " x energy : " + VadClass.Energy + " ->: "+ VadClass.alpha_short * VadClass.Energy);
        //Debug.Log("ShortEnergy" + VadClass.ShortEnergy);

        if (VadClass.NoiseFloor < VadClass.ShortEnergy)
        {
            VadClass.NoiseFloor = (1 - VadClass.alpha_long) * VadClass.NoiseFloor + VadClass.alpha_long * VadClass.Energy;
        }
        else 
        {
            VadClass.NoiseFloor = (1 - VadClass.alpha_short) * VadClass.NoiseFloor + VadClass.alpha_short * VadClass.Energy;
        }

        VadClass.Threshold = VadClass.NoiseFloor/(1 - VadClass.alpha_long) + VadClass.beta;
        flag = 0;

        //Debug.Log("Threshold =" + VadClass.Threshold);
        //Debug.Log("BEta = " + VadClass.beta);
       
        
        if (VadClass.ShortEnergy >= VadClass.Threshold){
            flag = 1;
        }
        if (flag == 1)
        {
            VadClass.Hangover = VadClass.HangTime;
        }
        else 
        {
            if (VadClass.Hangover-- <= 0)
            {
                VadClass.Hangover = 0;
            }
            else {
                flag = 1;
            }
        }
        return flag;
    }
    */
}

public class VAD
{
    private double alpha_long;
    private double alpha_short;
    private double beta;
    private int hang_time;
    private double noise_floor;
    private double short_energy;
    private int hangover;

    public VAD(double alpha_long = 0.99, double alpha_short = 0.1, double beta = 1.5, int hang_time = 5)
    {
        this.alpha_long = alpha_long;
        this.alpha_short = alpha_short;
        this.beta = beta;
        this.hang_time = hang_time;
        this.noise_floor = 0;
        this.short_energy = 0;
        this.hangover = 0;
    }

    private double ComputeStd(float[] frame)
    {
        // Calcula a média do frame
        double mean_frame = frame.Average();
        // Calcula o desvio padrão do frame
        double std_frame = Math.Sqrt(frame.Select(x => (x - mean_frame) * (x - mean_frame)).Average());
        // Aplica a fórmula com logaritmo natural e multiplicação por 20
        std_frame = 20 * Math.Log(std_frame + 1e-10);  // Adiciona uma pequena constante para evitar log(0)
        return std_frame;
    }

    public int ProcessFrame(float[] frame)
    {
        // Calcula a energia do frame
        double energy = frame.Select(x => x * x).Sum();
        // Atualiza a energia de curto prazo
        this.short_energy = (1 - this.alpha_short) * this.short_energy + this.alpha_short * energy;

        // Atualiza o nível de ruído
        if (this.noise_floor < this.short_energy)
        {
            this.noise_floor = (1 - this.alpha_long) * this.noise_floor + this.alpha_long * energy;
        }
        else
        {
            this.noise_floor = (1 - this.alpha_short) * this.noise_floor + this.alpha_short * energy;
        }

        // Calcula o limiar de detecção
        double threshold = this.noise_floor / (1 - this.alpha_long) + this.beta;

        // Calcula o desvio padrão do frame
        double std_frame = ComputeStd(frame);

        // Inicializa a flag de voz como 0 (não voz)
        int flag = 0;

        // Detecta a presença de voz
        if (this.short_energy >= threshold && std_frame > 0)
        {
            flag = 1;
        }

        // Gerencia o tempo de hangover
        if (flag == 1)
        {
            this.hangover = this.hang_time;
        }
        else
        {
            if (this.hangover > 0)
            {
                this.hangover--;
                flag = 1;
            }
        }

        return flag;
    }
}