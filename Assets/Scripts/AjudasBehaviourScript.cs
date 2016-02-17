using UnityEngine;
using System.Collections;

public class AjudasBehaviourScript : MonoBehaviour {


    public enum Ajudas {
            Sorte,
            MaisCinco,
            Misturar,
            RemoverQuadrante
    }

    // ajudas
    private static int _maisCinco; // quantidade de ajudas do tipo mais 5
    private static int _sortes; // quantidades de ajuda do tipo sorte
    private static int _misturar; // quantidade de ajudas do tipo misturar
    private static int _removeQuatrante; // quantidade de ajudas do tipo remover quadrante

    // adciona a quantidade da ajuda passada
    public static void UsarAjuda(Ajudas ajuda)
    {
        
        switch (ajuda)
        {
            case Ajudas.MaisCinco:
                MaisCinco(-1);
                break;
            case Ajudas.Misturar:
                Misturar(-1);
                break;
            case Ajudas.Sorte:
                Sortes(-1);
                break;
            case Ajudas.RemoverQuadrante:
                RemoverQuadrante(-1);
                break;
        }

    }

    // adciona a quantidade da ajuda passada
    public static void Ajuda(Ajudas ajuda, int quantidade)
    {
        if (quantidade < 0) return;

        switch (ajuda)
        {
            case Ajudas.MaisCinco:
                MaisCinco(quantidade);
                break;
            case Ajudas.Misturar:
                Misturar(quantidade);
                break;
            case Ajudas.Sorte:
                Sortes(quantidade);
                break;
            case Ajudas.RemoverQuadrante:
                RemoverQuadrante(quantidade);
                break;
        }        

    }

    // retorna a quantidade da ajuda passada
    public static int Ajuda(Ajudas ajuda)
    {

        switch (ajuda)
        {
            case Ajudas.MaisCinco:
                if (_maisCinco == 0)
                {
                    _maisCinco = PlayerPrefs.GetInt("_mainCinco_");
                }
                return _maisCinco;
              
            case Ajudas.Misturar:
                if (_misturar == 0)
                {
                    _misturar = PlayerPrefs.GetInt("_misturar_");
                }
                return _misturar;
            case Ajudas.Sorte:
                if (_sortes == 0)
                {
                    _sortes = PlayerPrefs.GetInt("_sortes_");
                }
                return _sortes;
            case Ajudas.RemoverQuadrante:
                if (_removeQuatrante == 0)
                {
                    _removeQuatrante = PlayerPrefs.GetInt("_removeQuadrante_");
                }
                return _removeQuatrante;
        }
        // se chegar aqui é porque deu alguma coisa errada
        return -1;

    }
    // retonar a quantidade de ajudas do tipo misturar
    public static int Misturar()
    {
        if(_misturar == 0)
        {
            _misturar = PlayerPrefs.GetInt("_misturar_");
        }
        return _misturar;
    }
    // adiciona a quatidade de ajudas do tipo misturar
    public static void Misturar(int quantidade)
    {
        _misturar += quantidade;
        PlayerPrefs.SetInt("_misturar_", _misturar);
    }
    // usa a ajuda do tipo misturar
    public static void UsarMisturar()
    {
        Misturar(-1);
    }

    // retorna a quantidade de tipo removeQuadrante
    public static int RemoverQuadrante()
    {
        if(_removeQuatrante == 0)
        {
            _removeQuatrante = PlayerPrefs.GetInt("_removerQuadrante");
        }
        return _removeQuatrante;
    }
    // adiciona a quantidade de ajudas
    public static void RemoverQuadrante(int quantidade)
    {
        _removeQuatrante += quantidade;
        PlayerPrefs.SetInt("_quantidadeQuadrante_",quantidade);
        
    }
    //usa uma ajuda do tipo removerQuadrante
    public static void UsarRemoverQuadrante()
    {
        RemoverQuadrante(-1);
    }

    //quantidade de sortes
    public static int Sortes()
    {
        if(_sortes == 0)
        {
            _sortes = PlayerPrefs.GetInt("_sortes_");
        }
        return _sortes;
    }
    // adiciona a quantidade de sortes
    public static void Sortes(int quantidade)
    {
        _sortes += quantidade;
        PlayerPrefs.SetInt("_sortes_", _sortes);

    }
    // usa uma sorte
    public static void UsarSorte()
    {
        Sortes(-1);
    }

    //retorna a quantidade de ajudas mais 5
    public static int MaisCinco()
    {
        if (_maisCinco == 0)
        {
            _maisCinco = PlayerPrefs.GetInt("_maisCinco_");
        }
        return _maisCinco;
    }
    // adiciona quantidade de ajudas do tipo mais cinco
    public static void MaisCinco(int quantidade)
    {
        _maisCinco += quantidade;
        PlayerPrefs.SetInt("_maisCinco_", _maisCinco);

    }
    // usa uma ajuda do tipo mais cinco
    public static void UsarMaisCinco()
    {
        MaisCinco(-1);
    }

}
