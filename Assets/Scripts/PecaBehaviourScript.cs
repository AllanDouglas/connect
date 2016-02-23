using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class PecaBehaviourScript : MonoBehaviour
{

    //public enum Tipo { SOMA, SUBTRACAO};
    // eventos
    public delegate void OnMouseClick(PecaBehaviourScript peca);
    public static event OnMouseClick OnPecaClicada;
    [Header("Sprite da peca")]
    public SpriteRenderer sprite;
    [Header("Identificador único da peca")]
    public int id;
	[Header("Sprite da tranformação do coringa")]
	public Sprite spriteCoringa;
	//[Header("Rotulo mostrando o tipo da peca")]
    //public TextMesh label; // label do tipo
    //[Header("Tipo da peca para contagem")]
    //public Tipo tipo;


    //posicao no tabuleiro 
    private int _x; 
    private int _y;
	// flega o status de coringa
	private bool _coringa = false;

	public bool Coringa{
		get { return _coringa; }
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

	public void TransformaEmCoringa(){
		_coringa = true;
		this.sprite.color = Color.white;
		this.sprite.sprite = spriteCoringa;
		this._linha.SetColors (Color.white,Color.white);
	}



    void OnMouseDown()
    {
        //Debug.Log("##### Peca " + gameObject.name +" id " +id+ " clicada ##########");
        if (OnPecaClicada != null)
        {
            OnPecaClicada(this);
        }
    }

    void Awake()
    {
        //seta a camada 
        gameObject.layer = LayerMask.NameToLayer("Pecas");
        //adiciona component de linha
        _linha = gameObject.AddComponent<LineRenderer>();
    }
    //sai do tabuleiro
    public void Sair()
    {
        // x = -1;
        // y = -1;
        _linha.SetVertexCount(0);
        _linha.enabled = false;
        gameObject.name = "desativada"; // deve ser removido para o metodo da classe Peca
		gameObject.SetActive(false);
    
    }

	private void Desativar(){
		gameObject.SetActive(false);
	}

	// Use this for initialization
    void Start()
    {
        //configura a camada da peca
        this.sprite.sortingLayerName = "Pecas";
        //pega a cor do sprite
        this._cor = this.sprite.color;
        _linha.SetVertexCount(2);
        _linha.material = new Material(Shader.Find("Sprites/Default"));
        _linha.SetWidth(0.2f, 0.2f);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
