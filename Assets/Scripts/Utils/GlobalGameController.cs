using UnityEngine;
using System.Collections;

public class GlobalGameController
{

    public static ArrayList niveis; // niveis 
    public static int nivelSelecionado; // nivel



    private static int _nivelAtual; // nivel maximo que o jogador já chegou
   


    // salvar nivel atual
    public static void GravarNivel()
    {
        _nivelAtual++;
        PlayerPrefs.SetInt("_nivelAtual_", _nivelAtual);

    }
    // retorna o nivel atual do jogador
    public static int NivelAtual()
    {
        if (_nivelAtual == 0)
        {
            _nivelAtual = PlayerPrefs.GetInt("_nivelAtual_");
        }

        return _nivelAtual;
    }



}
