using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NivelButtonBehaviourScript : MonoBehaviour {

    public int indexDoNivel; // numero da fase
    public Text label; // label do nivel    
    public Image background; // a cor do background identifica se o nivel ja foi passado   
    public bool passou = false; // flag para controle se o jogador já passou desse nivel


    public void ConfigurarIndexDoNivel(int index)
    {
        this.indexDoNivel = index;
        this.ConfigurarRotulo(index.ToString());
           
    }

    public void ConfigurarRotulo(string rotulo)
    {
        label.text = rotulo;
    }

    public void Passar()
    {
        passou = true;
        background.gameObject.SetActive(false);
    }

    public void Start()
    {

      
    }
    
    
    // redireciona para o nivel selecionado
    public void IrParaONivel()
    {

    }




}
