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
	public InGameUIBehaviourScript  InGameUI;

    [Header("Interface de GameOver")]
    public GameOverBehaviourScript GameOverUI;

	[Header("Interface de Dica Inicial")]
	public DicaInicialBehaviourScript DicaInicial;

	[Header("Interface de Dica")]
	public DicaBehaviourScript InformacaoUI;
	public DicaBehaviourScript InformacaoMetaUI;
	public DicaBehaviourScript InformacaoAjudaUI;

    [Header("Interface de Pause")]
    public PauseGameBehaviourScript PauseUI;

	[Header("Interface de Level Clear")]
	public ClearLevelBehaviourScript LevelClearUI;

	[Header("Interface de Transform pai dos contadores")]
	public Transform PaiDosContadores;

    [Header("Linha de seleção")]
    public LineRenderer _linha; // linha que vai ligar 

    [Header("Reservatorios")]
    public ContadorBehaviourScript[] reservatorios;

	[Header("Limitador")]
	public Collider2D limitador;

	[Header("Audio de clipes")]
	public AudioClip ac_selecao;
	public AudioClip ac_mensagem;
	public AudioClip ac_vitoria;
	public AudioClip ac_metaAtingida;


    //Grid
	private TabuleiroScript _Tabuleiro = new TabuleiroScript(); // script do tabuleiro
	private GradeBehaiourScript _gradePrefab; // prafabe da grade para os casos de niveis com grades
	private PecaBehaviourScript[] _dotsPrefabs; // lista de prefabs dos circulos
    private ContadorBehaviourScript _reservatorioPrefab; // prafab do reservatorio
	private ParticleSystem _particulaSaidaPrefab; // prafab da particula
	private ParticleSystem _particulaCoringaPrefab; // prefab da particula de coringa
	private AudioSource _audioSourceFx; // fonte de audio dos efeitos sonoros
	private Color _corDaLinha; // cor da linha        
	private int _colunas; // quantidade de colunas
	private int _linhas; // quantidade de linhas
	//prefabs
	private GradeBehaiourScript[,] _pecasNegrasGrid; // grid para armezenar as pecas negraas quando houverem
	private List<ParticleSystem> _particulasDeSaida = new List<ParticleSystem>() ; // pool de particulas de saida
	private List<ParticleSystem> _particulasCoringa = new List<ParticleSystem>(); // pool de particulas de transformação de coringa
    private List<ContadorBehaviourScript> _listaDeReservatoriosAtivos = new List<ContadorBehaviourScript>(); // lista dos reservatorios    
    private List<PecaBehaviourScript> _dotsAtivas = new List<PecaBehaviourScript>(); // pecas que estão no grid    
    private List<PecaBehaviourScript> _pecasSelecionadas = new List<PecaBehaviourScript>(); // pecas que foram tocadas
	private PreDicaBehaviourScritp preDica; // prefab do predica

    //Nivel
	private Hashtable _informacaoDoNivel; // informacoes do nivel
	private ArrayList _posicoesPecasFortes = new ArrayList(); // posicoes das pecas fortes
	private ArrayList _posicoesPecasNegras = new ArrayList(); // posicoes das pecas negras
    private float _objetivo = 0; // quantidade de pontos que devem ser atingidos para passar do nivel
    private int _metas = 0; // quantidade de metas que devem ser batidas
    private float _chances = 0; // quantidade de chances que podem ser usadas para finalizar
    private int _metasAtingidas = 0; // quantidade de metas que foram atingidas
    private int _pontos = 0; // quantidade de pontos atuais 
	private int _qtdPecasNegras = 0; // quantidade de pecas negras no tabuleiro
	private int _qtdPecasFortes = 0; // quantidade de pecas negras no tabuleiro
	private int _qtdPecasNegrasRestantes = 0; // quantidade de pecas negras restantes no tabuleiro 
    private bool _jogando = true;

	// flags da ajuda
	private bool _removerQuadrante = false;
	private bool _removerTipo = false;

    // Use this for initialization
    void Start()
    {
        Setup();
    }
    // Remove uma chance do jogador
    private void RemoverChance()
    {
        _chances--; // remove uma chance
		InGameUI.movimentos.text = _chances.ToString(); // atualiza o texto

		if (_chances == 0)
        {
			if (!VerificarMetas ())
				GameOver("Ops! seus movimentos acabaram.");
        }

    }

	// verifica se todas as metas foram batidas
	private bool VerificarMetas(){

		List<PecaBehaviourScript> pecasNegras = _Tabuleiro.PecasAtivas.FindAll (peca => {
			return peca.Condicao == PecaBehaviourScript.CondicaoEspecial.NEGRA;
		});

		_qtdPecasNegrasRestantes = pecasNegras.Count;

		//Debug.Log ("### pecas fortes "+_qtdPecasFortes+", quantidade de pecas negras "+pecasNegras.Count);
		
		return (_metas == _metasAtingidas & _pontos >= _objetivo & 
			_chances >= 0 & pecasNegras.Count == 0 & _qtdPecasFortes == 0) ;

	}

    //trata dos eventos de cliques 
    private void PecaSelecionadaHandler(PecaBehaviourScript peca)
    {


        //verifica se o jogo está ativo
        //if (!_jogando) return;

		if (peca.Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA & _removerQuadrante) {
			_removerQuadrante = false;
			RemoverQuadrante (peca);
			return;
		}

		if (peca.Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA & _removerTipo) {
			_removerTipo = false;
			RemoverTipo (peca);
			return;
		}
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

				// toca o som da peca selecionada
				_audioSourceFx.PlayOneShot(this.ac_selecao);

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

					// toca o som da peca selecionada
					_audioSourceFx.PlayOneShot(this.ac_selecao);
						
					//desenhando a linha
                    //adicionando  line renderer na ultima peca adicionada 
                    // com dois pontos um na propria peca e outro da peca posterior
                    LineRenderer linha = ultimaPeca.gameObject.GetComponent<LineRenderer>();
                    linha.SetColors(_corDaLinha, _corDaLinha);

                    linha.SetVertexCount(2);

					Vector3 ultimaPosicao = new Vector3 (ultimaPeca.transform.position.x, ultimaPeca.transform.position.y, -1);
					Vector3 atualPosicao = new Vector3 (peca.transform.position.x, peca.transform.position.y, -1);

                    linha.SetPosition(0,
							ultimaPosicao
                        );
                    linha.SetPosition(1,
							atualPosicao
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


		// seta os textos da ui ingame
		InGameUI.movimentos.text = _chances.ToString();
		InGameUI.objetivo.text =   _objetivo.ToString();
		InGameUI.barraDepontos.maxValue = _objetivo;

        // tamanho do GRID
        float tamanho = (float)nivel["tabuleiro"];
        int tam = (int)tamanho;



        switch (tam)
        {
		case 3:
			_colunas = 6;
			_linhas = 6;
			limitador.transform.position = new Vector2 (0, -4);
                break;
            case 2:
                _colunas = 5;
                _linhas = 5;
			limitador.transform.position = new Vector2 (0, -2.56f);
                break;
            case 1:
                _colunas = 4;
                _linhas = 4;
			limitador.transform.position = new Vector2 (0, -2.56f);
                break;
        }
		//configurações de tabuleiro


		//pecas forte
		float pecasFortes = (float) nivel["fortes"];
		_qtdPecasFortes = (int) pecasFortes;
		//_posicoesPecasFortes = nivel["fortes"] as ArrayList;
		//pecas negras
		float pecasNegras = (float) nivel["negras"];
		_qtdPecasNegras = (int)pecasNegras;
		//_posicoesPecasNegras = nivel["negras"] as ArrayList;


		InGameUI.pecasForteRestantes.text = "X " + pecasFortes;
		InGameUI.pecasNegrasRestantes.text = "X " + pecasNegras;

		if (pecasFortes > 0 & PlayerPrefs.GetInt("_tutorial_pecas_fortes") != 2) {
			Debug.Log ("carregando a predica das pecas fortes");
			CarregaPreDicaForte ();
			PlayerPrefs.SetInt ("_tutorial_pecas_fortes",1);
		}

		if (pecasNegras > 0 & PlayerPrefs.GetInt("_tutorial_pecas_negras") != 2) {
			Debug.Log ("carregando a predica das pecas negras");
			PlayerPrefs.SetInt ("_tutorial_pecas_negras",1);
			CarregaPreDicaNegra ();
		}
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
        int pontos = (quantidade * 10);
		// pega a quantidade de pontos diferenção de 3
		if (quantidade > 3) {
			int extraPontos = quantidade - 3;
			extraPontos *= 10;
			// adiciona os pontos extras
			pontos += extraPontos;		
			if (quantidade > 4) {
				if (quantidade == 5) {
					InformacaoUI.dica.text = "Você está indo muito bem: " + pontos.ToString () + " pontos";
				} else if (quantidade >= 6 & quantidade <= 7) {
					InformacaoUI.dica.text = "Você é inacreditável: " + pontos.ToString () + " pontos";
				} else if (quantidade >= 8 & quantidade <= 9) {
					InformacaoUI.dica.text = "Isso deveria ser impossível: " + pontos.ToString () + " pontos";
				} else if (quantidade >= 10) {
					InformacaoUI.dica.text = "Super, ultra, mega. Fantasticooo!: " + pontos.ToString () + " pontos";			
				}
				InformacaoUI.Entrar ();
				_audioSourceFx.PlayOneShot (ac_mensagem);
			}
		}

		_pontos += pontos; // incrementa a quantidade nova ao total atual

		InGameUI.barraDepontos.value = _pontos; // pontos 
		InGameUI.pontos.text = _pontos.ToString();

        // verifica se a pontuação foi suficiante para atingir os objetivos
		if (_pontos >= _objetivo /*_pontos >= _objetivo & _metas == _metasAtingidas*/)
        {
			if(VerificarMetas())
				LevelClear();
        }


    }
	// trata as pecas selecionads
	private IEnumerator TratarPecasSelecionadas(){

		_jogando = false;
		Debug.Log ("não pode ligar");
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
			if(_pecasSelecionadas.Count >= 4 
				& _pecasSelecionadas [_pecasSelecionadas.Count - 1].Condicao 
				== PecaBehaviourScript.CondicaoEspecial.NORMAL){
				// recupera a ultima peca selecionada
				PecaBehaviourScript ultimaPeca = _pecasSelecionadas [_pecasSelecionadas.Count - 1];
				//tranforma ela em coringa
				ultimaPeca.TransformarEmCoringa();
				_particulasCoringa [0].gameObject.SetActive (false);
				_particulasCoringa [0].transform.position = ultimaPeca.transform.position;
				_particulasCoringa [0].transform.parent = ultimaPeca.transform;
				_particulasCoringa [0].gameObject.SetActive (true);
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
							if(yInicial + y < 0 | yInicial + y >= _linhas ) continue;

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

		List<PecaBehaviourScript> pecasNegras = _Tabuleiro.PecasAtivas.FindAll (peca => {
			return peca.Condicao == PecaBehaviourScript.CondicaoEspecial.NEGRA;
		});

		_qtdPecasNegrasRestantes = pecasNegras.Count;

		InGameUI.pecasForteRestantes.text = "X " + _qtdPecasFortes;
		InGameUI.pecasNegrasRestantes.text = "X " + _qtdPecasNegrasRestantes;


		yield return new WaitForSeconds (.5f);
		_jogando = true;
		yield return new WaitForSeconds (1f);
		//RePosicionar();// reposiciona as pecas do tabuleiro 
		//RePreencher(); // recoloca as pecas que estão faltando no tabuleiro
		if (!_Tabuleiro.VerificaPossibilidades ()) {
			_jogando = false;
			//Debug.Log ("não existem nenhuma conexão possível");
			InformacaoUI.dica.text = "Não ha nenhum movimento possível. Misturando o tabuleiro.";
			InformacaoUI.Entrar();
			yield return new WaitForSeconds (0.5f);
			while (!_Tabuleiro.VerificaPossibilidades ()) {
				_Tabuleiro.Misturar ();
				yield return new WaitForSeconds (0.2f);
			}
			_jogando = true;
		}



		//yield return new WaitForSeconds(0);
	}

    //trata o evento de quando o toque é liberado
    // lugar onde é removido as peças seleciodas
    private void ToqueLiberadoHandler()
    {
        //verifica se o jogo está ativo
		 if (!_jogando) return;
		StartCoroutine (TratarPecasSelecionadas());        
    }
   
    // controla a quantidade de metas atingidas
    private void MetaAtingida(ContadorBehaviourScript contador)
    {
        _metasAtingidas++;

		if(VerificarMetas()) 
        {
			LevelClear();
        }
		// exibe informacao
		InformacaoMetaUI.cor = contador.cor;
		InformacaoMetaUI.dica.text = "Atenção! Meta atingida";
		InformacaoMetaUI.Entrar ();
		_audioSourceFx.PlayOneShot (ac_mensagem);
    }
    // controla quando a meta é ultrapassada
    private void MetaUltrapassada(ContadorBehaviourScript contador)
    {
     
		GameOver("Ops! Você extrapolou uma meta.");
    }

    //gameover
	private void GameOver(string motivo)
    {

		// tocar animação do game over

		// tocar som de game Over
		// coloca o motivo do game over
		GameOverUI.motivo.text = motivo;

		_jogando = false;
		//remove o ingame 
		//this.InGameUI.SetActive(false);
		tabuleiro.gameObject.SetActive(false);

		GameOverUI.fortes.text = "Restantes: " + _qtdPecasFortes;
		GameOverUI.negras.text = "Restantes: " + _qtdPecasNegrasRestantes;

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
		//this.InGameUI.gameObject.SetActive (_jogando);
		// altera a visivilidade do tabuleiro
		//this.tabuleiro.gameObject.SetActive (_jogando);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


	// inicia o jogo
	public void Iniciar(){

		if (_jogando)
			return;

		// preenche o tabuleiro
		_Tabuleiro.transformPai = tabuleiro;
		_Tabuleiro.Preencher (_colunas, _linhas);


		// coloca as pecas pretas no tabuleiro
		if (_qtdPecasNegras > 0) {
			
			//			_qtdPecasNegas = _posicoesPecasNegras.Count;
			PosicionarPecasNegras (_Tabuleiro.PecasAtivas);
		}else
		// colocar as pecas fotes no tabuleiro
		if (_qtdPecasFortes > 0) {
				
			//			_qtdPecasFortes = _posicoesPecasFortes.Count;
			PosicionarPecasFortes (_Tabuleiro.PecasAtivas);
		}

		DicaInicial.Sair ();
		_jogando = true;
	}

    //Monta o tabuleiro
    private void Setup()
    {
        //o jogo sempre começa ativo
        _jogando = false;
        Time.timeScale = 1.0f;

		//captura o component de audio source
		_audioSourceFx = GetComponent<AudioSource>();

        //Configura o nivel
        ConfigurarNivel();

		DicaInicial.objetivo.text = _objetivo+" pontos" ;
		DicaInicial.movimento.text = "Com " + _chances+" movimentos";

		if(_qtdPecasFortes > 0)
			DicaInicial.Especial =Especiais.FORTES;
		else if(_qtdPecasNegras > 0)
			DicaInicial.Especial =Especiais.NEGRAS;
		
		DicaInicial.meta_1.text = reservatorios [0].meta.text;
		DicaInicial.meta_2.text = reservatorios [1].meta.text;
		DicaInicial.meta_3.text = reservatorios [2].meta.text;
		DicaInicial.meta_4.text = reservatorios [3].meta.text;

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

			ParticleSystem particula = Instantiate (_particulaSaidaPrefab) as ParticleSystem;
			particula.gameObject.SetActive (false);
			particula.transform.parent = transform;
			_particulasDeSaida.Add (particula);

		}
		// alimenta o pool de particulas coringa
		for(int i = 0; i < (_colunas * _linhas); i++){

			ParticleSystem particula = Instantiate (_particulaCoringaPrefab) as ParticleSystem;
			particula.gameObject.SetActive (false);
			particula.transform.parent = transform;
			_particulasCoringa.Add (particula);

		}


		// exibe dicas

		if (preDica != null) {

			Debug.Log ("Exibe a predica");

			preDica.gameObject.SetActive (true);
		}

		DicaInicial.gameObject.SetActive (true);


    }

	// posiciona as pecas negras no tabuleiro
	private void PosicionarPecasNegras(List<PecaBehaviourScript> pecas){
		// cria uma nova lista
		List<PecaBehaviourScript> novasPecas = new List<PecaBehaviourScript> ();
		//preenche a lista com as pecas 
		novasPecas.AddRange(pecas);
		// varre a lista nova
		for (int i = 0; i < _qtdPecasNegras; i++) {
			// randomiza uma posicao da lista
			int index = UnityEngine.Random.Range(0,novasPecas.Count);
			// pega a peca
			PecaBehaviourScript peca = novasPecas[index];
			//transforma a peca em peca forte
			peca.TransformarEmNegra();
			// remove da lista
			novasPecas.Remove(peca);

		}

	}

	// posiciona as pecas fortes no tabuleiro
	private void PosicionarPecasFortes(List<PecaBehaviourScript> pecas){
		// cria uma nova lista
		List<PecaBehaviourScript> novasPecas = new List<PecaBehaviourScript> ();
		//preenche a lista com as pecas 
		novasPecas.AddRange(pecas);
		// varre a lista nova
		for (int i = 0; i < _qtdPecasFortes; i++) {

			// randomiza uma posicao da lista
			int index = UnityEngine.Random.Range(0,novasPecas.Count);
			// pega a peca
			PecaBehaviourScript peca = novasPecas[index];
			//transforma a peca em peca forte
			peca.TransformarEmForte();
			// remove da lista
			novasPecas.Remove(peca);

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


    //carrega os recursos de prefabs do level
    private void CarregarRecursos(string caminho)
    {
        _dotsPrefabs = Resources.LoadAll<PecaBehaviourScript>(caminho + "circulos");
       // _reservatorioPrefab = Resources.Load<ContadorBehaviourScript>(caminho + "reservatorios/Reservatorio") as ContadorBehaviourScript;
		_particulaSaidaPrefab = Resources.Load<ParticleSystem> (caminho + "particula/ParticulaSaida") as ParticleSystem;
		_particulaCoringaPrefab = Resources.Load<ParticleSystem> (caminho + "particula/ParticulaTransformacaoCoringa") as ParticleSystem;
		_gradePrefab =  Resources.Load<GradeBehaiourScript> (caminho + "grade") as GradeBehaiourScript;
    }

    //carrega os recursos padrao
    private void CarregarRecursos()
    {
        CarregarRecursos(PADRAO);
    }
	// configura o proximo toque para remover quadrante
	public void RemoverQuadrante(){


		// verific se ainda tem chances
		//-- remover comentario if( AjudasHelper.RemoverQuadrante() > 0 & !_jogando) return;

		_removerTipo = false;
		_removerQuadrante = true;

		AjudasHelper.RemoverQuadrante (-1);

		InformacaoAjudaUI.dica.text = "Selecione um objeto";
		InformacaoAjudaUI.Entrar ();

	}

	// remove quadrante 
	private void RemoverQuadrante(PecaBehaviourScript peca){
		
		List<PecaBehaviourScript> pecasList = new List<PecaBehaviourScript> ();
		int QtdPecasParaPontos = 0;
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
				if(yInicial + y < 0 | yInicial + y >= _linhas ) continue;

				//posicao
				int xPosicao = xInicial + x;
				int yPosicao = yInicial + y;
				//recupera a peca
				PecaBehaviourScript pecaAtual =  _Tabuleiro.Tabuleiro[xPosicao,yPosicao];

				// verifica se a peca atual é a mesma da peca ou se ela ainda não existe dentro do array de pecas selecinadas
				if (pecaAtual != peca & !pecasList.Contains(pecaAtual)) {
					// se for diferente adiciona a peca dentro da lista de pecas selecionadas
					pecasList.Add(pecaAtual);
					// incrementa a quantidade de pecas para pontuar
					QtdPecasParaPontos++;
				}

			}
		}

		// lista que vai armazenar as pecas do tipo forte
		List<PecaBehaviourScript> pecasFortes = new List<PecaBehaviourScript> ();
		int indexPeca = 0;
		for (int i = 0; i < pecasList.Count; i++) {

			ParticleSystem particula = _particulasDeSaida [i];
			particula.gameObject.SetActive (false);


			PecaBehaviourScript _peca = pecasList [indexPeca];

			particula.transform.position = _peca.transform.position;
			particula.startColor = _peca.cor;
			particula.gameObject.SetActive (true);
			indexPeca++;
			// verifica se a peca é forte
			if (_peca.Condicao == PecaBehaviourScript.CondicaoEspecial.FORTE) {
				//pecasFortes.Add (_peca);
				_qtdPecasFortes--;
				_peca.TornarNormal (); // deixa a peca normal
			}


		}

		//remove as pecas fortes
		pecasList.RemoveAll (_peca => {
			return peca.Condicao == PecaBehaviourScript.CondicaoEspecial.FORTE;
		});

		pecasList.Add (peca);
		_Tabuleiro.Remover (pecasList);
		Pontuar (QtdPecasParaPontos);
	}

	//configura a ajuda para remover tipo
	public void RemoverTipo(){

		// verific se ainda tem chances
		// -- remover comentario if( AjudasHelper.RemoverTipo() <= 0 & _jogando) return;

		AjudasHelper.RemoverTipo (-1);

		_removerTipo = true;
		_removerQuadrante = false;

		InformacaoAjudaUI.dica.text = "Selecione uma cor.";
		InformacaoAjudaUI.Entrar ();

	}

	private void RemoverTipo(PecaBehaviourScript peca){
		int QtdPecasParaPontos = 0;
		int tipo = peca.id; // peca o id da peca principal

		List<PecaBehaviourScript> pecasList = new List<PecaBehaviourScript> ();

		// varre o tabuleiro
		for (int x = 0; x < _colunas; x++) {

			for (int y = 0; y < _linhas; y++) {

				if (tipo == _Tabuleiro.Tabuleiro [x, y].id & 
					_Tabuleiro.Tabuleiro [x, y].Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA) {

					pecasList.Add (_Tabuleiro.Tabuleiro [x, y]);
					QtdPecasParaPontos++;

				}

			}
		}

		// lista que vai armazenar as pecas do tipo forte
		List<PecaBehaviourScript> pecasFortes = new List<PecaBehaviourScript> ();
		int indexPeca = 0;
		for (int i = 0; i < pecasList.Count; i++) {

			ParticleSystem particula = _particulasDeSaida [i];
			particula.gameObject.SetActive (false);


			PecaBehaviourScript _peca = pecasList [indexPeca];

			particula.transform.position = _peca.transform.position;
			particula.startColor = _peca.cor;
			particula.gameObject.SetActive (true);
			indexPeca++;
			// verifica se a peca é forte
			if (_peca.Condicao == PecaBehaviourScript.CondicaoEspecial.FORTE) {
				//pecasFortes.Add (_peca);
				_qtdPecasFortes--;
				_peca.TornarNormal (); // deixa a peca normal
			}
		}
		//remove as pecas fortes
		pecasList.RemoveAll (_peca => {
			return peca.Condicao == PecaBehaviourScript.CondicaoEspecial.FORTE;
		});

		pecasList.Add (peca);
		_Tabuleiro.Remover (pecasList);
		Pontuar (QtdPecasParaPontos);


	}

    // usa a ajuda do tipo misturar
    public void Misturar()
    {
        // verifica se ainda existem ajudas para ser usadas
		if (AjudasHelper.Misturar() <= 0 & _jogando)
        {

			InformacaoAjudaUI.dica.text = "Misturando o tabuleiro!!";
			InformacaoAjudaUI.Entrar ();

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
		if( AjudasHelper.MaisCinco() <= 0 & _jogando) return;

		InformacaoAjudaUI.dica.text = "Mais 5 movimentos para você.";
		InformacaoAjudaUI.Entrar ();

		// remove uma chance
		AjudasHelper.MaisCinco(-1);
		// executa a animação

		// adiciona mais cinco
		_chances += 5;
		// atualiza a label
		InGameUI.movimentos.text = _chances.ToString();

	}

	// remove 5 pecas aleatorias 
	public void Sorte()
	{

	}
		
	// carreca p ŕefab da dica forte
	private void CarregaPreDicaForte(){

		this.preDica = Resources.Load<PreDicaBehaviourScritp> ("Prefabs/PreDicaPecasFortes") 
			as PreDicaBehaviourScritp;
		this.preDica = Instantiate (preDica) as PreDicaBehaviourScritp;

		this.preDica.transform.parent = DicaInicial.transform.parent;
		this.preDica.transform.localScale = new Vector3 (1,1,0);
		this.preDica.transform.localPosition = new Vector3 (0,230,0);
	}

	// carreca prefab da dica negra
	private void CarregaPreDicaNegra(){

		this.preDica = Resources.Load<PreDicaBehaviourScritp> ("Prefabs/PreDicaPecasNegras")
			as PreDicaBehaviourScritp;
		this.preDica = Instantiate (preDica) as PreDicaBehaviourScritp;
		this.preDica.gameObject.SetActive (true);

		this.preDica.transform.parent = DicaInicial.transform.parent;
		this.preDica.transform.localScale = new Vector3 (1,1,0);
		this.preDica.transform.localPosition = new Vector3 (0,230,0);
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