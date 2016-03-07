using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class PecaBehaviourScript : MonoBehaviour
{

	public enum CondicaoEspecial {CORINGA, NEGRA, FORTE, NORMAL}

    //public enum Tipo { SOMA, SUBTRACAO};
    // eventos
//    public delegate void OnMouseClick(PecaBehaviourScript peca);
//    public static event OnMouseClick OnPecaClicada;
    [Header("Sprite da peca")]
    public SpriteRenderer spriteRenderer;
    [Header("Identificador único da peca")]
    public int id;
	[Header("Sprite padrão")]
	public Sprite _spritePadrao; // sprite padrão da peca
	[Header("Sprite da tranformação do coringa")]
	public Sprite spriteCoringa;
	[Header("Sprite da tranformação do forte")]
	public Sprite spriteForte;
	//[Header("Rotulo mostrando o tipo da peca")]
    //public TextMesh label; // label do tipo
    //[Header("Tipo da peca para contagem")]
    //public Tipo tipo;

	private CondicaoEspecial _condicao = CondicaoEspecial.NORMAL; // condicao especial da peca
	public CondicaoEspecial Condicao{
		get{ return _condicao; } 
	}

	//public bool debug;

    //posicao no tabuleiro 
    private int _x; 
    private int _y;


	public bool EhCoringa(){
		return _condicao == CondicaoEspecial.CORINGA;
	}

	public bool EhNegra(){
		return _condicao == CondicaoEspecial.NEGRA;
	}


	private Color _cor; // cor do sprite
    private LineRenderer _linha; // linha


    //encapsulamento da cor
    public Color cor
    {
        get {
            return _cor;
        }        
    }


    public int y
    {
        get
        {
            return _y;
        }
        private set
        {
            _y = value;
        }
    }

    public int x
    {
        get
        {
            return _x;
        }
        private set
        {
            _x = value;
        }
    }

    public void SetPosicao(int x, int y)
    {
		this.x = x;
        this.y = y;
        
        gameObject.name = string.Format("({0},{1})", x, y);        
        _linha.enabled = true;

    }
	// transforma a peca em peca forte
	public void TransformarEmForte(){
		_condicao = CondicaoEspecial.FORTE;
		this.spriteRenderer.sprite = spriteForte;
	}

	// transforma em boa negra removendo
	public void TransformarEmNegra(){
		_condicao = CondicaoEspecial.NEGRA;
		this.spriteRenderer.color = Color.gray;	
	}

	// tranforma a peca em coringa
	public void TransformarEmCoringa(){
		_condicao = CondicaoEspecial.CORINGA;
		this.spriteRenderer.color = Color.white;
		this.spriteRenderer.sprite = spriteCoringa;
		this._linha.SetColors (Color.gray,Color.gray);
	}

	public void TornarNormal(){
		
		this.spriteRenderer.sprite = this._spritePadrao;
		this._condicao = CondicaoEspecial.NORMAL;
		this.spriteRenderer.color = this._cor;
		this._linha.SetColors (_cor,_cor);
		_linha.SetVertexCount(0);
		//_linha.enabled = false;
	}
		

    void Awake()
    {
		//seta a camada 
        gameObject.layer = LayerMask.NameToLayer("Pecas");
        //adiciona component de linha
        _linha = gameObject.AddComponent<LineRenderer>();
		//configura a camada da peca
		this.spriteRenderer.sortingLayerName = "Pecas";
		//pega a cor do sprite
		this._cor = this.spriteRenderer.color;
		//this._spritePadrao = this.spriteRenderer.sprite;

		_linha.SetVertexCount(2);
		_linha.material = new Material(Shader.Find("Sprites/Default"));
		_linha.SetWidth(0.2f, 0.2f);
    }
    //sai do tabuleiro
    public void Sair()
    {
        // x = -1;
        // y = -1;
        _linha.SetVertexCount(0);
        _linha.enabled = false;
        gameObject.name = "desativada"; // deve ser removido para o metodo da classe Peca

		// quando a peca sai do tabuleiro ela deve voltar ao normal
		_condicao = CondicaoEspecial.NORMAL;

		this.spriteRenderer.sprite = _spritePadrao;
		this.spriteRenderer.color = this._cor;

		gameObject.SetActive(false);
    
    }

	private void Desativar(){
		gameObject.SetActive(false);
	}


}
