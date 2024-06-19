using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Testar : MonoBehaviour
{

    public GameObject Texts;
    public GameObject Images;

    public Image[] Vetor_image = new Image[3];
    public Text[] Vetor_Texts = new Text[4];
    public float threshold;

    public int caseflag;
    public bool preset;
    public float delta;

    void Start()
    {

        enabled = false;  // enabled variavel de monobehavior
    }

    // Update is called once per frame
    void Update()
    {




        switch (GlobalConfig.Instance.mfccMetric) {
            case "mean":
            for (int i = 0; i < 4; i++) {
                switch (i) {
                    case 0:
                        Vetor_Texts[i].text = "Threshold : " + threshold + " Delta : " + delta;
                        break;
                    case 1:
                        Vetor_Texts[i].text = "MFCC Inspiracao : " + MFCC6.mfcc_in;
                        break;
                    case 2:
                        Vetor_Texts[i].text = "MFCC Expiracao : " + MFCC6.mfcc_ex;
                        break;
                    case 3:
                        Vetor_Texts[i].text = "MFCC atual : " + MFCC6.Getvalue();
                        break;
                }
            }
                break;
            case "median":
            for (int i = 0; i < 4; i++) {
                switch (i) {
                    case 0:
                        Vetor_Texts[i].text = "Threshold : " + threshold + " Delta : " + delta;
                        break;
                    case 1:
                        Vetor_Texts[i].text = "MFCC Mediana Inspiracao : " + MFCC6.mediana_ins;
                        break;
                    case 2:
                        Vetor_Texts[i].text = "MFCC Mediana Expiracao : " + MFCC6.mediana_ex;
                        break;
                    case 3:
                        Vetor_Texts[i].text = "MFCC atual : " + MFCC6.Getvalue();
                        break;
                }
            }
                break;
            case "max":
            for (int i = 0; i < 4; i++) {
                switch (i) {
                    case 0:
                        Vetor_Texts[i].text = "Threshold : " + threshold + " Delta : " + delta;
                        break;
                    case 1:
                        Vetor_Texts[i].text = "MFCC Maior Inspiracao : " + MFCC6.maior_valor_ins;
                        break;
                    case 2:
                        Vetor_Texts[i].text = "MFCC Maior Expiracao : " + MFCC6.maior_valor_ex;
                        break;
                    case 3:
                        Vetor_Texts[i].text = "MFCC atual : " + MFCC6.Getvalue();
                        break;
                }
            }
                break;
            case "min":
            for (int i = 0; i < 4; i++) {
                switch (i) {
                    case 0:
                        Vetor_Texts[i].text = "Threshold : " + threshold + " Delta : " + delta;
                        break;
                    case 1:
                        Vetor_Texts[i].text = "MFCC Menor Inspiracao : " + MFCC6.menor_valor_ins;
                        break;
                    case 2:
                        Vetor_Texts[i].text = "MFCC Menor Expiracao : " + MFCC6.menor_valor_ex;
                        break;
                    case 3:
                        Vetor_Texts[i].text = "MFCC atual : " + MFCC6.Getvalue();
                        break;
                }
            }
                break;
            default:
                threshold = (MFCC6.mfcc_in + MFCC6.mfcc_ex) / 2f;
                break;
        }







     
       //if (Algs.VADTEST(Main.spectro) == 1)// temos respiracao
       if(MFCC6.Getvalue() >  (threshold + delta) || MFCC6.Getvalue() < (threshold -delta)) // temos respiracao 
       {
            Vetor_image[1].color = new Vector4(255, 0, 0, 255);
            if (MFCC6.Getvalue() > threshold)
            {
                Vetor_image[0].color = new Vector4(0, 255, 0, 255);
                Vetor_image[2].color = new Vector4(255, 0, 0, 255);
                //inspiracao
            }
            else 
            {

                Vetor_image[2].color = new Vector4(0, 255, 0, 255);
                Vetor_image[0].color = new Vector4(255, 0, 0, 255);
                // expiracao
            }


        }
        else  // n temos respiracao
        {

            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    Vetor_image[i].color = new Vector4(0, 255, 0, 255);
                }
                else 
                {
                    Vetor_image[i].color = new Vector4(255, 0, 0, 255);
                }
                
            }

        }
 

     

    }


    public void OnClick() {
        Texts.SetActive(true);
        Images.SetActive(true);

        if (preset == true) {
            MFCC6.mfcc_in = -0.4379355f;//-0.09678023f; 
            MFCC6.mfcc_ex = -0.667509f;//-0.6237471f;

            MFCC6.maior_valor_ex = -0.667509f;
            MFCC6.maior_valor_ins = -0.4379355f;
            MFCC6.menor_valor_ex = -0.667509f;
            MFCC6.menor_valor_ins = -0.4379355f;

            MFCC6.mediana_ins  = -0.4379355f;
            MFCC6.mediana_ex = -0.667509f;
        
        }


        switch (GlobalConfig.Instance.mfccMetric) {
            case "mean":
                // Calcula a média
                threshold = (MFCC6.mfcc_in + MFCC6.mfcc_ex) / 2f;
                break;
            case "median":
                threshold = (MFCC6.mediana_ins + MFCC6.mediana_ex) / 2f;
                break;
            case "max":
                threshold = (MFCC6.maior_valor_ex + MFCC6.maior_valor_ins) / 2f;
                break;
            case "min":
                threshold = (MFCC6.menor_valor_ex + MFCC6.menor_valor_ins) / 2f;
                break;
            default:
                threshold = (MFCC6.mfcc_in + MFCC6.mfcc_ex) / 2f;
                break;
        }


        threshold = (MFCC6.mfcc_in + MFCC6.mfcc_ex) / 2f;
        
        
        delta = GlobalConfig.Instance.delta;

        for (int i = 0; i < 3; i++)
        {

            Vetor_image[i].color = new Vector4(255, 0, 0, 255);
        }

        enabled = true;

    }


    IEnumerator Ciclo2() {

        yield return null;
    }


}
