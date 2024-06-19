using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;



public class Calibrar : MonoBehaviour
{


    public int ciclos;
    public int threshold_mfcc;

    public GameObject[] Panels = new GameObject[4];
    public GameObject EventTxt;



    public void Click()
    {

        ciclos = GlobalConfig.Instance.ciclo;
        EventTxt.SetActive(true);
        Text text = EventTxt.GetComponent<Text>();
        foreach (GameObject p in Panels)
        {
            p.SetActive(true);
            p.GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
        }


        StartCoroutine(Ciclo2());


    }




    // Update is called once per frame



    IEnumerator Ciclo2()
    {
        float[] media = new float[2];
        float media_de_menor_valor_ins = 0;
        float media_de_maior_valor_ins = 0;

        float media_de_menor_valor_ex = 0;
        float media_de_maior_valor_ex = 0;

        float media_de_mediana_ex = 0;
        float media_de_mediana_in = 0;

        //cvs
        string strFilePath = @"D:\Caio\Data.csv";
        string strSeperator = ";";
        float[] csv_ins = new float[ciclos];
        float[] csv_ex = new float[ciclos];

        StringBuilder sbOutput = new StringBuilder();


        for (int i = 0; i < ciclos; i++)
        {
            Panels[0].GetComponent<Image>().color = new Vector4(255, 255, 0, 255);
            Panels[1].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
            Panels[2].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
            Panels[3].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);

            //
            yield return waitForKeyPress(KeyCode.UpArrow); // inspiiiracao

            MFCC6.SetFlag(true); // para update no main

            Panels[0].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);
            Panels[1].GetComponent<Image>().color = new Vector4(255, 255, 0, 255);
            // para update no main

            yield return waitForKeyRealese(KeyCode.UpArrow);
            MFCC6.SetFlag(false);// termina update da inpiracao

            lock (MFCC6.lockMFCC6)
            { //lock para couroutine
                Debug.Log("Ins");
                float aux = MFCC6.MediaMfcc6();
                media[0] = media[0] + aux;
                media_de_maior_valor_ins = media_de_maior_valor_ins + MFCC6.maior_valor;
                media_de_menor_valor_ins = media_de_menor_valor_ins + MFCC6.menor_valor;
                media_de_mediana_in = media_de_mediana_in + MFCC6.MedianaMfcc6();
                
                csv_ins[i] = aux;
                //sbOutput.AppendLine(string.Join(strSeperator, MFCC6.MediaMfcc6()));
                MFCC6.ResetCont();
                MFCC6.ResetMenor();
                MFCC6.ResetMaior();
                MFCC6.ResetVetor();
            }


            Panels[0].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
            Panels[1].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);

            /*
            while (tempo > 0)
            {
                tempo = tempo - Time.deltaTime;
                Panels[1].GetComponentInChildren<Text>().text = "Espere " + tempo;
            }
            tempo = 2;
            */
            yield return new WaitForSeconds(2.5f);
            Panels[1].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
            Panels[2].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);
            Panels[2].GetComponent<Image>().color = new Vector4(255, 255, 0, 255);

            yield return waitForKeyPress(KeyCode.DownArrow);// expiiracao
            // job aqui


            MFCC6.SetFlag(true);// para update no main
            Panels[2].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);
            Panels[3].GetComponent<Image>().color = new Vector4(255, 255, 0, 255);
            
            yield return waitForKeyRealese(KeyCode.DownArrow);
            
            MFCC6.SetFlag(false);// termina update da expiracao
            lock (MFCC6.lockMFCC6)
            { //lock para couroutine
                Debug.Log("Ex");
                float aux = MFCC6.MediaMfcc6();
                media[1] = media[1] + aux;
                media_de_maior_valor_ex = media_de_maior_valor_ex + MFCC6.maior_valor;
                media_de_menor_valor_ex = media_de_menor_valor_ex + MFCC6.menor_valor;
                media_de_mediana_ex = media_de_mediana_ex + MFCC6.MedianaMfcc6();
                
                
                csv_ex[i] = aux;
                //sbOutput.AppendLine(string.Join(strSeperator, MFCC6.MediaMfcc6()));
                MFCC6.ResetCont();
                MFCC6.ResetMenor();
                MFCC6.ResetMaior();
                MFCC6.ResetVetor();
            }

            // fim de job aqui

            Panels[2].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
            Panels[3].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);

            yield return new WaitForSeconds(2.5f);
            /*
            while (tempo > 0)
            {
                tempo = tempo - Time.deltaTime;
                Panels[3].GetComponentInChildren<Text>().text = "Espere " + tempo;
            }
            */
            Panels[1].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
            EventTxt.GetComponent<Text>().text = "Eventos: " + (i + 1);


        }

        // ins csv
        sbOutput.Append("INS:");
        for (int i = 0; i < ciclos; i++)
        {
            sbOutput.Append(csv_ins[i]);
            sbOutput.Append(";");

        }
        sbOutput.AppendLine();
        sbOutput.Append("EX:");
        for (int i = 0; i < ciclos; i++)
        {
            sbOutput.Append(csv_ex[i]);
            sbOutput.Append(";");
        }
        sbOutput.AppendLine();

        media[0] = media[0] / (float)ciclos;
        media[1] = media[1] / (float)ciclos;
        sbOutput.AppendLine(string.Join(strSeperator, media[0]));
        sbOutput.AppendLine(string.Join(strSeperator, media[1]));

        File.AppendAllText(strFilePath, sbOutput.ToString());


        media_de_menor_valor_ins = media_de_menor_valor_ins / (float)ciclos;
        media_de_maior_valor_ins = media_de_maior_valor_ins / (float)ciclos;

        media_de_menor_valor_ex = media_de_menor_valor_ex / (float)ciclos;
        media_de_maior_valor_ex = media_de_maior_valor_ex / (float)ciclos;

        media_de_mediana_ex = media_de_mediana_ex / (float)ciclos;
        media_de_mediana_in = media_de_mediana_in / (float)ciclos;


        MFCC6.mediana_ex =  media_de_mediana_ex;
        MFCC6.mediana_ins = media_de_mediana_in;
        MFCC6.menor_valor_ex = media_de_menor_valor_ex;
        MFCC6.maior_valor_ex = media_de_maior_valor_ex;
        MFCC6.menor_valor_ins = media_de_menor_valor_ins;
        MFCC6.maior_valor_ins = media_de_maior_valor_ins;


        MFCC6.mfcc_in = media[0];
        MFCC6.mfcc_ex = media[1];
        Debug.Log("MFCC 6 IN " + MFCC6.mfcc_in);
        Debug.Log("MFCC 6 EX " + MFCC6.mfcc_ex);



        EventTxt.SetActive(false);
        foreach (GameObject p in Panels)
        {
            p.SetActive(false);
        }



        yield return null;

    }



    private IEnumerator waitForKeyPress(KeyCode key)
    {
        bool done = false;
        while (!done) // essentially a "while true", but with a bool to break out naturally
        {
            if (Input.GetKeyDown(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }

        // now this function returns
    }


    private IEnumerator waitForKeyRealese(KeyCode key)
    {
        bool done = false;
        while (!done) // essentially a "while true", but with a bool to break out naturally
        {
            if (Input.GetKeyUp(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }

        // now this function returns
    }


    /*int Ciclo() {
        float tempo = 2;

        Panels[0].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);
        Panels[1].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
        Panels[2].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
        Panels[3].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);


        while (!Input.GetKey("up")) {
            Panels[0].GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
            Panels[1].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);

        }

        Panels[0].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
        Panels[1].GetComponent<Image>().color = new Vector4(255, 255, 255, 255);


        while (tempo > 0) {
            tempo =  tempo - Time.deltaTime;
            Panels[1].GetComponentInChildren<Text>().text = "Espere " + tempo; 
        }
        tempo = 2;

        Panels[1].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
        Panels[2].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);


        while (!Input.GetKey("down")) {
            Panels[2].GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
            Panels[3].GetComponent<Image>().color = new Vector4(0, 255, 0, 255);

        }

        Panels[2].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);
        Panels[3].GetComponent<Image>().color = new Vector4(255, 255, 255, 255);

        while (tempo > 0)
        {
            tempo = tempo - Time.deltaTime;
            Panels[3].GetComponentInChildren<Text>().text = "Espere " + tempo;
        }
        Panels[1].GetComponent<Image>().color = new Vector4(255, 0, 0, 255);



        return 1;
    }*/
}
