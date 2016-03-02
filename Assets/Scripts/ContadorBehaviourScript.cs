using UnityEngine;
using UnityEngine.UI;


public class ContadorBehaviourScript : MonoBehaviour
{

    /** Eventos **/
    public delegate void RepositorioEvento(ContadorBehaviourScript contador);
    //diparado quanto o reservatorio entra no range
    public static event RepositorioEvento MetaAtingida;
    //dispara quando a meta e extrapolada
    public static event RepositorioEvento MetaUltrapassada;
    //disparada quando o valor maximo 
    public static event RepositorioEvento ValorMaximoAtingido;
    
   
    /** Fim das definições dos eventos **/

    /**identificação do tipo do reservatório**/
    public int id;
    /**valor inicial do reservatório**/
    public int valorInicial = 0;

    /** 
        limites de coleta, se apenas maximo for setado então 
        apenas esse será levado em consideração 
        e se for atingido irá disparar o evento o evento de 
        valor maximo atingido.

        Se os valores tanto de minimo quanto de maximo forem passados 
        então será conciderado o range, disparando o evento de meta

    **/
    public int metaMinima, metaMaxima;
    /** slider que representa graficamente o reservatorio**/
    public Slider contador;
    // cor do preenchimento
    public Color cor;
	public Color corDoBackground;
    // marca da cor dor reservatorio
    public Image marcador;
	public Image background;
	public Image metaUltrapassada;
    //labels
    public Text quantidadeAtual;
    public Text meta;

    /**valor maximo possivel sempre será igual ao valor maximo da meta mais 10 **/
    private int _valorMaximo;
    private int _valorAtual;
    private bool _verificacaoPorRange = false;

    private bool _metaAtingida = false;
    private bool _metaUltrapassada = false;


    /** adiciona a quantidade **/
    public void Adicionar(int quantidade)
    {
        this.contador.value += quantidade;
        this._valorAtual += quantidade;

        quantidadeAtual.text = _valorAtual.ToString();

        //verifica os valores da meta
        if (_valorAtual >= metaMinima & _valorAtual <= metaMaxima & !_metaAtingida)
        {
            _metaAtingida = true;
			if(MetaAtingida != null) 
            	MetaAtingida(this);
        }
        else
        // verifica se esta exatamente no valor maximo
        if (_valorAtual == metaMaxima & !_metaUltrapassada)
        {
				
            _metaUltrapassada = true;
			if(ValorMaximoAtingido != null)
            	ValorMaximoAtingido(this);
        }
        //verifica se o valor maximo da meta foi ultrapassada
        else if (_valorAtual > metaMaxima)
        {		
			//this.background.sprite = metaUltrapassada;	
			metaUltrapassada.gameObject.SetActive(true);

			if(MetaUltrapassada != null)
            	MetaUltrapassada(this);
        }

    }
    /** adiciona mais um **/
    public void Adicionar()
    {
        Adicionar(1);
    }

    // remove a quantidade 
    public void Remover(int quantidade)
    {
        this.contador.value -= quantidade;
    }

    //remove um 
    public void Remover()
    {
        Remover(1);
    }

    public void Configurar()
    {
        //define os valores inicial e maximo
        this.contador.value = valorInicial;
        this.contador.maxValue = metaMaxima;

        // define a cor do slider
        this.contador.fillRect.gameObject.GetComponent<Image>().color = cor;
        
        // define a cor do marcador
		if(marcador != null)
        	this.marcador.color = cor;
		
        //verifica se o reservatorio é por meta ou valor maximo
        if (metaMinima >= 0 & metaMaxima > 0)
        {          



            this._verificacaoPorRange = true;
			//metaMinima.ToString() +
			meta.text = metaMinima.ToString()+"|"+metaMaxima.ToString();
        }
        else if (metaMaxima > 0)
        {
            this._verificacaoPorRange = false;
			meta.text = "/"+metaMaxima.ToString();
        }
        //configura os textos
        quantidadeAtual.text = valorInicial.ToString();

    } 

    // Use this for initialization
    void Start()
    {
        //meta.text = "";
        quantidadeAtual.text = "0";

        this.contador.value = 0;
        
        // define a cor do slider
        this.contador.fillRect.gameObject.GetComponent<Image>().color = cor;
		this.metaUltrapassada.color = cor;
		this.background.color = corDoBackground;
    }

    void Update()
    {
        
    }
   
}
