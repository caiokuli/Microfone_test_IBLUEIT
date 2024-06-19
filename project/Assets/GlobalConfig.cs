using UnityEngine;

public class GlobalConfig : MonoBehaviour
{
    public static GlobalConfig Instance;

    // Propriedade para a quantidade de amostras
    public int qtdAmostras;
    // Propriedade para a taxa de amostragem
    public float taxaAmostragem;
    public float delta;
    public FFTWindow fftWindow;
    public TipoFiltro tipoFiltro;
    public FaixaFrequencia faixaFrequencia;
    public int quantidadeFilterbank;
    public int frequenciaMaxMel;
    public string melFilterBankType;
    public string mfccMetric;
    public int ciclo;

    public enum TipoFiltro
    {
        Filtrofft,
        FiltroUnity,
        FiltroButterworth,
        SemFiltro
    }

    public struct FaixaFrequencia
    {
        public int Min;
        public int Max;

        public FaixaFrequencia(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Isso faz com que a instância persista entre cenas
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetConfig(string key, string value)
    {
        switch (key)
        {
            case "qtd_amostras":
                qtdAmostras = int.Parse(value);
                break;
            case "taxa_amostragem":
                taxaAmostragem = int.Parse(value);
                break;
            case "delta":
                delta = float.Parse(value);
                break;
            case "fftWindow":
                fftWindow = (FFTWindow)System.Enum.Parse(typeof(FFTWindow), value, true);
                break;
            // Adicione mais casos conforme necessário



            case "tipo_filtro":
                tipoFiltro = (TipoFiltro)System.Enum.Parse(typeof(TipoFiltro), value, true);
                break;
            case "faixa_frequencia":
                var freqs = value.Split('-');
                if (freqs.Length == 2)
                {
                    faixaFrequencia = new FaixaFrequencia(int.Parse(freqs[0]), int.Parse(freqs[1]));
                }
                break;

            case "quantidade_filterbank":
                quantidadeFilterbank = int.Parse(value);
                break;
            
            case "frequencia_max_mel":
                frequenciaMaxMel = int.Parse(value);
                break;

            case "melFilterBankType":
                melFilterBankType = value;
                break;
            case "mfccMetric":
                mfccMetric = value;
                Debug.Log(value);
                break;
            case "ciclo":
                ciclo = int.Parse(value);
                break;
        }
    }
}