using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelBehaviourScript : MonoBehaviour
{
    // define o espaçamento lateral dos itens do grid
    private const float ESPACAMENTO_ENTRE_PECAS = 1.2f;
    // referencia da tela 
    private const float LARGURA_DE_REFERENCIA = 800;


    //caminhos para os recursos 
    private const string PADRAO = "Prefabs/padrao/";

    [Header("Objeto pai das pecas")]
    public Transform tabuleiro;

	[Header("Interface de in game")]
	public GameObject  InGameUI;

    [Header("Interface de GameOver")]
    public GameOverBehaviourScript GameOverUI;

    [Header("Interface de Pause")]
    public PauseGameBehaviourScript PauseUI;

	[Header("Interface de Level Clear")]
	public ClearLevelBehaviourScript LevelClearUI;

    [Header("Linha de seleção")]
    public LineRenderer _linha; // linha que vai ligar 

    [Header("Reservatorios")]
    public ContadorBehaviourScript[] reservatorios;

    [Header("Lables")]
    public Text pontos;
    public Text chances;

    //Grid

	private TabuleiroScript _Tabuleiro = new TabuleiroScript(); // script do tabuleiro

    private PecaBehaviourScript[] _dotsPrefabs; // lista de prefabs dos circulos
    private ContadorBehaviourScript _reservatorioPrefab; // prafab do reservatorio
	private ParticleSystem _particulaPrefab; // prafab da particula

	private List<ParticleSystem> _particulasDeSaida = new List<ParticleSystem>() ; // pool de particulas de saida
    private List<ContadorBehaviourScript> _listaDeReservatoriosAtivos = new List<ContadorBehaviourScript>(); // lista dos reservatorios
    //private List<PecaBehaviourScript> _dotsPool; //pool de dotos
    private List<PecaBehaviourScript> _dotsAtivas = new List<PecaBehaviourScript>(); // pecas que estão no grid
    //private PecaBehaviourScript[,] _tabuleiro; // tabuleiro
    private List<PecaBehaviourScript> _pecasSelecionadas = new List<PecaBehaviourScript>(); // pecas que foram tocadas
    private Color _corDaLinha; // cor da linha        
    private int _colunas; // quantidade de colunas
    private int _linhas; // quantidade de linhas

    //Nivel
	private Hashtable _informacaoDoNivel; // informacoes do nivel
	private ArrayList _posicoesPecasFortes = new ArrayList(); // posicoes das pecas fortes
	private ArrayList _posicoesPecasNegras = new ArrayList(); // posicoes das pecas negras
    private float _objetivo = 0; // quantidade de pontos que devem ser atingidos para passar do nivel
    private int _metas = 0; // quantidade de metas que devem ser batidas
    private float _chances = 0; // quantidade de chances que podem ser usadas para finalizar
    private int _metasAtingidas = 0; // quantidade de metas que foram atingidas
    private int _pontos = 0; // quantidade de pontos atuais 
	private int _qtdPecasNegas = 0; // quantidade de pecas negras no tabuleiro
	private int _qtdPecasFortes = 0; // quantidade de pecas negras no tabuleiro
    private bool _jogando = true;

    // Use this for initialization
    void Start()
    {
        Setup();
    }
    // Remove uma chance do jogador
    private void RemoverChance()
    {
        _chances--; // remove uma chance
        chances.text = _chances.ToString(); // atualiza o texto

		if (_chances == 0 & !VerificarMetas())
        {
            GameOver();
        }

    }

	// verifica se todas as metas foram batidas
	private bool VerificarMetas(){

		List<PecaBehaviourScript> pecasNegas = _Tabuleiro.PecasAtivas.FindAll (peca => {
			return peca.Condicao == PecaBehaviourScript.CondicaoEspecial.NEGRA;
		});
		
		return (_metas == _metasAtingidas & _pontos >= _objetivo & 
			_chances >= 0 & pecasNegas.Count == 0 & _qtdPecasFortes == 0) ;

	}

    //trata dos eventos de cliques 
    private void PecaSelecionadaHandler(PecaBehaviourScript peca)
    {

        //verifica se o jogo está ativo
        if (!_jogando) return;

		Debug.Log (peca.EhNegra());

        //verifica se a peca já está dentro das pecas selecionadas 
		if (!_pecasSelecionadas.Contains(peca) & !peca.EhNegra())
        {
			
            int quantidadeDeElementos = _pecasSelecionadas.Count;

            //pega a peca atual
            //adiciona a lista de selecionadas se a lista está vazia
            if (quantidadeDeElementos == 0)
            {
                _pecasSelecionadas.Add(peca);

				if(peca.EhCoringa())
					_corDaLinha = Color.gray;
				else
					_corDaLinha = peca.cor;
            }
            else
            //se a lista não está vazia verifica se a nova peca está             
            //pega a cor da peca - pega apenas uma vez - 
            //adejacente com a ultima peca e da mesma cor que a anterior
            //se a quantidade de pecas selecionadas for maior que um
            //, vai desenhando uma reta entre as pecas
            if (
					(peca.EhCoringa() | peca.id == _pecasSelecionadas[quantidadeDeElementos - 1].id | _pecasSelecionadas[quantidadeDeElementos - 1].EhCoringa() )
                )
            {
                //recupera a ultima peca
                PecaBehaviourScript ultimaPeca = _pecasSelecionadas[quantidadeDeElementos - 1];

                //verificando a distancia
                if ( // verifica se a peca é um coringa

                    // eixo X
                    ((Math.Abs(peca.x - ultimaPeca.x) == 1) & (peca.y - ultimaPeca.y == 0)) |
                     //eixo y
                     ((peca.x - ultimaPeca.x == 0) & (Math.Abs(peca.y - ultimaPeca.y) == 1))

                    )
                {
						
					//desenhando a linha
                    //adicionando  line renderer na ultima peca adicionada 
                    // com dois pontos um na propria peca e outro da peca posterior
                    LineRenderer linha = ultimaPeca.gameObject.GetComponent<LineRenderer>();
                    linha.SetColors(_corDaLinha, _corDaLinha);

                    linha.SetVertexCount(2);
                    linha.SetPosition(0,
                        ultimaPeca.transform.position
                        );
                    linha.SetPosition(1,
                        peca.transform.position
                        );
                    // Fim do desenho da linha

                    //finalmente adiciona a peca
                    _pecasSelecionadas.Add(peca);
                    //_linha.SetVertexCount(quantidadeDeElementos + 1);
                    //  _linha.SetPosition(quantidadeDeElementos, peca.transform.position);

                    //  _linha.gameObject.SetActive(true);
					}
            }

        }

    }
    //monta o nivel de acordo com os paramentros
    private void ConfigurarNivel()
    {

        //desativa todos os contadores
        foreach(ContadorBehaviourScript contador in reservatorios)
        {
            contador.gameObject.SetActive(false);
        }

        Debug.Log("######### Configurando o Nivel ##########");

        //lê o arquivostring textNivel
        TextAsset textNivel = (TextAsset) Resources.Load("niveis");
        
        //carrego arquivo convertendo para json
        ArrayList niveis = ExternalInterface.MiniJSON.JsonDecode(textNivel.text) as ArrayList;

        // aqui tem que ser recuperado o nivel selecionado para buscar no array 
        // a posicao do array é igual ao nivel -1 para corresponder ao array
        // para fim de testes uzarei a posicao 0 iremos sortear o nivel

		int index = UnityEngine.Random.Range (0, niveis.Count);

        Hashtable nivel = niveis[index] as Hashtable; // recupera o nivel atual
		_informacaoDoNivel = nivel;

        // chances de objetivo
		_chances =  (float)nivel["chances"];
        _objetivo = (float)nivel["objetivo"];


        chances.text = _chances.ToString();
        pontos.text = "0 /" + _objetivo.ToString();
        // tamanho do GRID
        float tamanho = (float)nivel["tabuleiro"];
        int tam = (int)tamanho;

        switch (tam)
        {
            case 3:
                _colunas = 6;
                _linhas = 6;
                break;
            case 2:
                _colunas = 5;
                _linhas = 5;
                break;
            case 1:
                _colunas = 4;
                _linhas = 4;
                break;
        }
		//configurações de tabuleiro


		//pecas forte
		_posicoesPecasFortes = nivel["fortes"] as ArrayList;
		//pecas negras
		_posicoesPecasNegras = nivel["negras"] as ArrayList;

		// fim das configurações do tabuleiro


        // metas 
        ArrayList metas = nivel["metas"] as ArrayList;

        if (metas.Count > 0)
        {
            this._metas = metas.Count; // quantidade de metas que devem ser batidas

            Hashtable meta; // meta atual do nivel selecionado
            ContadorBehaviourScript reservatorio = null; // resevatorio que será selecionado
            for (int i = 0; i < _metas; i++) // para cada meta ativo um contado
            {
                meta = metas[i] as Hashtable; // captura a meta atual
				//procura o contador correpondente
                foreach (ContadorBehaviourScript contador in reservatorios)
                {
                    float contadorId = (float)meta["contador_id"];
                    if (contador.id == (int)contadorId)
                    {
						reservatorio = contador; // selecionamos o contador
                        break;
                    }
                }

                float minima = (float)meta["minima"];
                float maxima = (float)meta["maxima"];



                reservatorio.metaMinima = (int)minima;
                reservatorio.metaMaxima = (int)maxima;
                reservatorio.gameObject.SetActive(true);
                reservatorio.Configurar();
                _listaDeReservatoriosAtivos.Add(reservatorio);
            }
        }

    }
    // pontua
    private void Pontuar(int quantidade)
    {
		// adiciona a quantidade de pontos
        _pontos += (quantidade * 10);
		// pega a quantidade de pontos diferenção de 3
		if (quantidade > 3) {
			int extraPontos = quantidade - 3;
			extraPontos *= 10;
			// adiciona os pontos extras
			_pontos += extraPontos;
		}

        pontos.text = _pontos.ToString()+" /"+_objetivo.ToString();

        // verifica se a pontuação foi suficiante para atingir os objetivos
        if (_pontos >= _objetivo & _metas == _metasAtingidas)
        {
			LevelClear();
        }


    }

    //trata o evento de quando o toque é liberado
    // lugar onde é removido as peças seleciodas
    private void ToqueLiberadoHandler()
    {
        //verifica se o jogo está ativo
        if (!_jogando) return;

        //verifica a quantidade de pecas ativas, ser for maior que 1 
        if (_pecasSelecionadas.Count > 1)
        {
			// guarda a quantidade de pecas selecionadas inicialmente
			int QtdPecasParaPontos = _pecasSelecionadas.Count;
            
            //procura o contador corespondente
            ContadorBehaviourScript contador;
			// verifica se a peca da primeira posicao é um coringa
			if (!_pecasSelecionadas [0].EhCoringa()) {
				foreach (ContadorBehaviourScript reservatorio in _listaDeReservatoriosAtivos) {
					if (reservatorio.id == _pecasSelecionadas [0].id) {
						contador = reservatorio;
						contador.Adicionar (_pecasSelecionadas.Count);
						break;
					}
				}
			}

			// se a quantidade de pecas selecionadas for maior ou igual a 5 então 
			// a ultima peca deve ser transformada em coringa
			// e removida das pecas selecionadas para remoção
			// desde que ela não seja coringa
			if(_pecasSelecionadas.Count >= 5 
				& _pecasSelecionadas [_pecasSelecionadas.Count - 1].Condicao 
						== PecaBehaviourScript.CondicaoEspecial.NORMAL){
				// recupera a ultima peca selecionada
				PecaBehaviourScript ultimaPeca = _pecasSelecionadas [_pecasSelecionadas.Count - 1];
				//tranforma ela em coringa
				ultimaPeca.TransformarEmCoringa();
				// remove a peca da lista
				_pecasSelecionadas.Remove(ultimaPeca);
			}

			// varre as pecas buscando pecas coringa
			for(int i = 0; i < _pecasSelecionadas.Count; i++){
				// peca da vez
				PecaBehaviourScript peca = _pecasSelecionadas [i];
				// verifica se a peca é um coringa
				if (peca.EhCoringa()) {
					// recupera as pecas no quadrande
					/*
						*###
						*#O#
						*###
					*/
					// intera até 9
					int xInicial = peca.x - 1;
					int yInicial = peca.y - 1;

					for(int x = 0;x < 3; x++){ // colunas

						// verifica se o x está dentro do tabuleiro
						if(xInicial + x < 0 | xInicial + x >= _colunas ) continue;

						for (int y = 0; y < 3; y++) { // linhas
							if(yInicial + y < 0 | xInicial + y >= _linhas ) continue;

							//posicao
							int xPosicao = xInicial + x;
							int yPosicao = yInicial + y;
							//recupera a peca
							PecaBehaviourScript pecaAtual =  _Tabuleiro.Tabuleiro[xPosicao,yPosicao];

							// verifica se a peca atual é a mesma da peca ou se ela ainda não existe dentro do array de pecas selecinadas
							if (pecaAtual != peca & !_pecasSelecionadas.Contains(pecaAtual)) {
								// se for diferente adiciona a peca dentro da lista de pecas selecionadas
								_pecasSelecionadas.Add(pecaAtual);
								// incrementa a quantidade de pecas para pontuar
								QtdPecasParaPontos++;
							}

						}
					}

				}

			}

			// exibe uma particula de explosão no lugar de uma peca
			int indexPeca = 0;

			int quantidadePecasSelecionadas = _pecasSelecionadas.Count;
			// lista que vai armazenar as pecas do tipo forte
			List<PecaBehaviourScript> pecasFortes = new List<PecaBehaviourScript> ();

			for (int i = 0; i < _pecasSelecionadas.Count; i++) {

				ParticleSystem particula = _particulasDeSaida [i];
				particula.gameObject.SetActive (false);


				PecaBehaviourScript peca = _pecasSelecionadas [indexPeca];

				particula.transform.position = peca.transform.position;
				particula.startColor = peca.cor;
				particula.gameObject.SetActive (true);
				indexPeca++;
				// verifica se a peca é forte
				if (peca.Condicao == PecaBehaviourScript.CondicaoEspecial.FORTE) {
					pecasFortes.Add (peca);
					_qtdPecasFortes--;
					peca.TornarNormal (); // deixa a peca normal
				}

			}
			//remove as pecas fortes
			_pecasSelecionadas.RemoveAll (peca => {
				Debug.Log(peca.Condicao);
				return pecasFortes.Contains(peca);
			});
			// pontua de acordo com a quantidade de pecas selecionadas
			Pontuar(QtdPecasParaPontos);
			// remove as pecas do tabuleiro
			_Tabuleiro.Remover (_pecasSelecionadas);
            // a cada combinação certa descontamos uma chance
            RemoverChance();
        }
        _pecasSelecionadas.Clear();// limpa lista     
		_Tabuleiro.Reposicionar();
		_Tabuleiro.Repreencher ();
        //RePosicionar();// reposiciona as pecas do tabuleiro 
        //RePreencher(); // recoloca as pecas que estão faltando no tabuleiro
		if (!_Tabuleiro.VerificaPossibilidades()) Debug.Log("não existem nenhuma conexão possível");
    }
   
    // controla a quantidade de metas atingidas
    private void MetaAtingida(ContadorBehaviourScript contador)
    {
       
        _metasAtingidas++;

		if(VerificarMetas()) 
        {
			LevelClear();
        }

    }
    // controla quando a meta é ultrapassada
    private void MetaUltrapassada(ContadorBehaviourScript contador)
    {
     
        GameOver();
    }

    //gameover
    private void GameOver()
    {
		_jogando = false;
		//remove o ingame 
		//this.InGameUI.SetActive(false);
		tabuleiro.gameObject.SetActive(false);

        GameOverUI.gameObject.SetActive(true);
    }

	// level clear
	private void LevelClear()
	{
		_jogando = false;
		
		LevelClearUI.gameObject.SetActive (true);
	}

    //pausa o game
    public void Pausar()
    {
		//inverte o status da pause
        _jogando = !_jogando;
        Time.timeScale =(_jogando)?1.0f:0.0f;
        //ativa ou não a interface de pause
        PauseUI.gameObject.SetActive(!_jogando);
		// altera a visibilidade da interface de ingameui
		this.InGameUI.SetActive (_jogando);
		// altera a visivilidade do tabuleiro
		this.tabuleiro.gameObject.SetActive (_jogando);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Monta o tabuleiro
    private void Setup()
    {
        //o jogo sempre começa ativo
        _jogando = true;
        Time.timeScale = 1.0f;

        //Configura o nivel
        ConfigurarNivel();

        //posiciona a camera
        float auxPosicao = (_colunas - 1f) / 2f;
        auxPosicao *= ESPACAMENTO_ENTRE_PECAS;
        Camera.main.transform.position = new Vector3(auxPosicao, Camera.main.transform.position.y, -10);

        //configura eventos
        TouchBehaviourScript.PecaTocada += PecaSelecionadaHandler;
        TouchBehaviourScript.ToqueLiberado += ToqueLiberadoHandler;

        ContadorBehaviourScript.MetaAtingida += MetaAtingida;
		ContadorBehaviourScript.MetaUltrapassada += MetaUltrapassada;
		//ContadorBehaviourScript.ValorMaximoAtingido += MetaUltrapassada;

        //carregando os recursos
        CarregarRecursos();
        CarregarPool();

		// alimenta o pool de particulas
		for(int i = 0; i < (_colunas * _linhas); i++){

			ParticleSystem particula = Instantiate (_particulaPrefab) as ParticleSystem;
			particula.gameObject.SetActive (false);
			particula.transform.parent = transform;
			_particulasDeSaida.Add (particula);

		}
		// preenche o tabuleiro
		_Tabuleiro.transformPai = tabuleiro;
		_Tabuleiro.Preencher (_colunas, _linhas);


		// coloca as pecas pretas no tabuleiro
		if (_posicoesPecasNegras.Count > 0) {
			_qtdPecasNegas = _posicoesPecasNegras.Count;
			PosicionarPecasNegras (_Tabuleiro.Tabuleiro);
		}
		// colocar as pecas fotes no tabuleiro
		if (_posicoesPecasFortes.Count > 0) {
			_qtdPecasFortes = _posicoesPecasFortes.Count;
			PosicionarPecasFortes (_Tabuleiro.Tabuleiro);
		}
			


    }
	// carrega as pecas que precisão de dois hits para sair 
	private void PosicionarPecasFortes(PecaBehaviourScript[,] pecas){
		
		foreach (Hashtable posicao in _posicoesPecasFortes) {
			float x =(float) posicao ["coluna"];
			float y =(float) posicao ["linha"];
			//recupera a peca na posicao
			PecaBehaviourScript peca = pecas[(int)x,(int) y];
			//tranforma a peca em peca forte
			peca.TransformarEmForte();

		}

	}

	// coloca as bolas negras no tabuleiro
	private void PosicionarPecasNegras(PecaBehaviourScript[,] pecas){

		foreach (Hashtable posicao in _posicoesPecasNegras) {
			float x =(float) posicao ["coluna"];
			float y =(float) posicao ["linha"];
			//recupera a peca na posicao
			PecaBehaviourScript peca = pecas[(int)x,(int) y];
			//tranforma a peca em peca forte
			peca.TransformarEmNegra();

		}

	}

    //carrega o pool de peças do jogo
    private void CarregarPool()
    {
        if (_dotsPrefabs.Length <= 0) throw new UnityException("recursos não carregados");

        for (int i = 0; i < _dotsPrefabs.Length; i++)
        {
            //para cada tipo de dot colocamos um reservatorio

            for (int j = 0; j < 15; j++)
            {
                PecaBehaviourScript peca = Instantiate(_dotsPrefabs[i]) as PecaBehaviourScript;
				peca.transform.parent = tabuleiro;
                peca.gameObject.SetActive(false);
                //_dotsPool.Add(peca);
				_Tabuleiro.AdicionaPecasPool(peca);

            }
        }

    }


    /***
    ** Posiciona os reservatorios de 
    ** acordo com a quantidade da lista
    **/
    private void PosicionaReservatorios()
    {
        // pega o tamanho da tela
        float tamanhoDaTela = 800;
        // divide pela quantidade de reservatorios ativos, no minimo 2 
        int divisor = (_listaDeReservatoriosAtivos.Count > 2) ? _listaDeReservatoriosAtivos.Count : 2;
        // divide o tamanho da tela pelo divisor
        float raiz = tamanhoDaTela / divisor;
        // para cada reservatorio
        for (int i = _listaDeReservatoriosAtivos.Count - 1; i >= 0; i--)
        {

            Vector3 novaPosicao = Camera.main.ScreenToWorldPoint(new Vector3(raiz * i, 0, 0));

            _listaDeReservatoriosAtivos[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(novaPosicao.x, -529, 0);
            _listaDeReservatoriosAtivos[i].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
    }

    //carrega os recursos de prefabs do level
    private void CarregarRecursos(string caminho)
    {
        _dotsPrefabs = Resources.LoadAll<PecaBehaviourScript>(caminho + "circulos");
       // _reservatorioPrefab = Resources.Load<ContadorBehaviourScript>(caminho + "reservatorios/Reservatorio") as ContadorBehaviourScript;
		_particulaPrefab = Resources.Load<ParticleSystem> (caminho + "particula/ParticulaSaida") as ParticleSystem;
    }

    //carrega os recursos padrao
    private void CarregarRecursos()
    {
        CarregarRecursos(PADRAO);
    }
    // usa a ajuda do tipo misturar
    public void Misturar()
    {
        // verifica se ainda existem ajudas para ser usadas
        if (AjudasHelper.Misturar() > 0)
        {
            // remove uma ajuda
            AjudasHelper.Misturar(-1);
            // executa a animação 

            // mistura o tabuleiro
			_Tabuleiro.Misturar();
        }


    }
	// almenta a quantidade de chances restantes
	public void MaisCinco()
	{
		// verific se ainda tem chances
		if( AjudasHelper.MaisCinco() <= 0) return;
		// remove uma chance
		AjudasHelper.MaisCinco(-1);
		// executa a animação

		// adiciona mais cinco
		_chances += 5;
		// atualiza a label
		chances.text = _chances.ToString();

	}

	// remove 5 pecas aleatorias 
	public void Sorte()
	{
		// verific se ainda tem chances
		if( AjudasHelper.Sortes() <= 0) return;
		// remove uma chance
		AjudasHelper.Sortes(-1);
		// executa a animação

		// sorteia cinco posicoes aleatorias
		List<PecaBehaviourScript> pecasEscolhidas = new List<PecaBehaviourScript>();

		int index = 0;

		PecaBehaviourScript[,] tabuleiro = _Tabuleiro.Tabuleiro; // recupera o tabuleiro atual

		while(index < 5)
		{
			
			// randomiza o x
			int x = UnityEngine.Random.Range(0,_colunas -1);
			// randomixa o y
			int y = UnityEngine.Random.Range(0,_linhas -1);
			//recupera a pecas randomizada
			PecaBehaviourScript pecaEscolhida = tabuleiro [x, y];

			for(int i = 0; i < 5; i++ )
			{
				
				if (pecasEscolhidas[i] == pecaEscolhida)
					break;
				pecasEscolhidas.Add(pecaEscolhida);

				//incremeta o contador
				index++;
				break;
			}
			
		}

		_Tabuleiro.Remover (pecasEscolhidas); // remove as pecas escolhidas do tabuleiro

		_Tabuleiro.Reposicionar();// reposiciona as pecas do tabuleiro 
		_Tabuleiro.Repreencher(); // recoloca as pecas que estão faltando no tabuleiro
	
	}
		

    // Update is called once per frame
    void Update()
    {

    }

    // antes de destruir o nivel
    public void OnDestroy()
    {
        // reseta os eventos
        TouchBehaviourScript.PecaTocada -= PecaSelecionadaHandler;
        TouchBehaviourScript.ToqueLiberado -= ToqueLiberadoHandler;
        ContadorBehaviourScript.MetaAtingida -= MetaAtingida;
        ContadorBehaviourScript.MetaUltrapassada -= MetaUltrapassada;
		ContadorBehaviourScript.ValorMaximoAtingido -= MetaUltrapassada;
    }

    

}