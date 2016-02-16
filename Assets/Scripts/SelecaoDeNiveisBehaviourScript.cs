using UnityEngine;
using System.Collections;

public class SelecaoDeNiveisBehaviourScript : MonoBehaviour {

    public Vector3 posicaoInicial; // posicao do primeiro botão
    public float ditanciaEntreBotoesEmX; // distancia entre os botões no eixo x
    public float ditanciaEntreBotoesEmY; // distancia entre os botões no eixo y
    public int quantidaeDeBotoesEmLinha; // quantidade de botões no eixo x
    public NivelButtonBehaviourScript nivelPrefab; // niveis 
    public Canvas canvas; // canvas pai dos botões

	// Use this for initialization
	void Start () {
        Setup();
	}
    // monta os niveis
    private void Setup()
    {
        //lê o arquivostring textNivel
        TextAsset textNivel = (TextAsset)Resources.Load("niveis");
        //carrego arquivo convertendo para json
        ArrayList niveis = ExternalInterface.MiniJSON.JsonDecode(textNivel.text) as ArrayList;
        // torna o array de niveis global
        GlobalGameController.niveis = niveis;

        //contador
        int index = 0;
        float novoX = posicaoInicial.x;
        float novoY = posicaoInicial.y;
        // varre o array de niveis
        foreach (Hashtable nivel in niveis)
        {
            //instacia um novo botão
            NivelButtonBehaviourScript nivelAtual = Instantiate(nivelPrefab) as NivelButtonBehaviourScript;
            // torna o botão filho do canvas
            nivelAtual.transform.SetParent(canvas.transform);
            nivelAtual.transform.localScale = new Vector3(1,1,1);
            nivelAtual.gameObject.SetActive(true);
            

            if(index == quantidaeDeBotoesEmLinha)
            {
                index = 0;
                // incrementa o y
                novoY += this.ditanciaEntreBotoesEmY;
                //volta o x para a posicao inicial
                novoX = posicaoInicial.x;

            }
            //configuração do nivel
            float id = (float)nivel["id"];
            nivelAtual.ConfigurarIndexDoNivel((int) id);

            Vector3 novaPosicao = Camera.main.WorldToViewportPoint(new Vector3(novoX, novoY, 0));
            Debug.Log(novaPosicao);

            //posiciona o botão
            nivelAtual.transform.position = novaPosicao;
            //depois de posicionar o atual incrementa o x
            novoX += this.ditanciaEntreBotoesEmX;
            // incrementa o index
            index++;

        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
