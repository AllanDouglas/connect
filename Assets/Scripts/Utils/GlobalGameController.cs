using UnityEngine;
using System.Collections;

public class GlobalGameController
{

    public static ArrayList niveis; // niveis 
    public static int nivelSelecionado; // nivel 


    private static int nivelAtual; // nivel maximo que o jogador já chegou

    // salvar nivel atual
    public static void GravarNivel()
    {
        nivelAtual++;
        PlayerPrefs.SetInt("_nivelAtual_", nivelAtual);

    }
    // retorna o nivel atual do jogador
    public static int NivelAtual()
    {
        if (nivelAtual == 0)
        {
            nivelAtual = PlayerPrefs.GetInt("_nivelAtual_");
        }       
        
        return nivelAtual;
    }


   
}
