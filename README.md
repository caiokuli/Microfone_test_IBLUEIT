# Microfone_test_IBLUEIT
Teste para classificação em (ins e ex) usando microfone

- qtd_amostras: Define o número de amostras a serem processadas em cada frame de áudio. Valores válidos incluem 512, 1024, 2048, 4096 e 8192. Ajustar este valor pode afetar a resolução do espectro de frequência e o desempenho do processamento.
  Exemplo: qtd_amostras=1024

- taxa_amostragem: Especifica a taxa de amostragem do áudio em Hz. Valores comuns são 22000, 44100, 48000 e 96000. Este parâmetro deve corresponder à taxa de amostragem do áudio que está sendo analisado.
  Exemplo: taxa_amostragem=44100

- delta: Um valor flutuante que define melhora a sensibilidade do thrashold (usado para controle da apneia) . Recomenda-se que varie entre 0 e 1.
  Exemplo: delta=0.5

- fftWindow: Define a janela a ser aplicada na Transformada de Fourier. Opções incluem: Rectangular, Triangle, Hamming, Hanning, Blackman, BlackmanHarris.
  Exemplo: fftWindow=Hanning

- tipo_filtro: Escolhe o tipo de filtro a ser usado no pré-processamento do áudio. Opções incluem: Filtrofft, FiltroUnity, FiltroButterworth, SemFiltro.
  Exemplo: tipo_filtro=MeuFiltroProgramado

- faixa_frequencia: Define a faixa de frequência do filtro selecionado. Deve ser especificado no formato Min-Max (ex: 1800-4000).
  Exemplo: faixa_frequencia=1800-4000

- quantidade_filterbank: Determina o número de coeficientes MFCC a serem gerados, com sugestões de valores sendo 13, 24 e 40.
  Exemplo: quantidade_filterbank=13

- frequencia_max_mel: Especifica a frequência máxima para o banco de filtros Mel. Recomenda-se usar a frequência máxima do filtro de pré-processamento ou a metade da taxa de amostragem.
  Exemplo: frequencia_max_mel=4000

- melFilterBankType: Permite escolher entre diferentes algoritmos de banco de filtros Mel. Opções são: ClassicMelFilterBank e AlternativeMelFilterBank.
  Exemplo: melFilterBankType=ClassicMelFilterBank

- mfccMetric: Define a métrica estatística a ser aplicada aos vetores MFCC. Opções incluem: mean (média), median (mediana), max (valor máximo) e min (valor mínimo).
  Exemplo: mfccMetric=mean

- ciclo: Indica a quantidade de ciclos respiratórios nos quais o áudio será processado. Valores recomendados estão entre 5 e 10.
  Exemplo: ciclo=5

Ajuste essas configurações conforme necessário para otimizar a análise de áudio. Certifique-se de salvar o arquivo após fazer as alterações e reiniciar o aplicativo para que as novas configurações tenham efeito.
