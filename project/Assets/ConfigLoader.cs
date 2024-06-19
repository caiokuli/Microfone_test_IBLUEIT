using System.IO;
using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    void Start()
    {
        string configFilePath = GetConfigFilePath();
        ReadConfigFile(configFilePath);
        Debug.Log("TERMINOU DE LER AQUIVO");
    }

    string GetConfigFilePath()
    {
        // Obt�m o diret�rio onde o execut�vel do jogo est� localizado
        string directoryPath = Path.GetDirectoryName(Application.dataPath);

        // Define o nome do arquivo de configura��o
        string configFileName = "configs.txt";

        // Combina o diret�rio com o nome do arquivo para criar o caminho completo
        string configFilePath = Path.Combine(directoryPath, configFileName);

        return configFilePath;
    }

    void ReadConfigFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    GlobalConfig.Instance.SetConfig(key, value);
                }
            }
        }
        else
        {
            Debug.LogError("Arquivo de configura��o n�o encontrado em: " + filePath);
        }
    }
}