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
        // Obtém o diretório onde o executável do jogo está localizado
        string directoryPath = Path.GetDirectoryName(Application.dataPath);

        // Define o nome do arquivo de configuração
        string configFileName = "configs.txt";

        // Combina o diretório com o nome do arquivo para criar o caminho completo
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
            Debug.LogError("Arquivo de configuração não encontrado em: " + filePath);
        }
    }
}