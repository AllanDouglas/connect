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
    [Header("Interface de GameOver")]
    public GameOverBehaviourScript GameOverUI;

    [Header("Interface de Pause")]
    public PauseGameBehaviourScript PauseUI;

    [Header("Linha de seleção")]
    public LineRenderer _linha; // linha que vai ligar 

    [Header("Reservatorios")]
    public ReservatorioBehaviourScript[] reservatorios;

    [Header("Lables")]
    public Text pontos;
    public Text chances;

    //Grid
    private PecaBehaviourScript[] _dotsPrefabs; // lista de prefabs dos circulos
    private ReservatorioBehaviourScript _reservatorioPrefab; // prafab do reservatorio
    private List<ReservatorioBehaviourScript> _listaDeReservatoriosAtivos = new List<ReservatorioBehaviourScript>(); // lista dos reservatorios
    private List<PecaBehaviourScript> _dotsPool; //pool de dotos
    private List<PecaBehaviourScript> _dotsAtivas = new List<PecaBehaviourScript>(); // pecas que estão no grid
    private PecaBehaviourScript[,] _tabuleiro; // tabuleiro
    private List<PecaBehaviourScript> _pecasSelecionadas = new List<PecaBehaviourScript>(); // pecas que foram tocadas
    private Color _corDaLinha; // cor da linha        
    private int _colunas;
    private int _linhas;

    //Nivel
    private float _objetivo = 0; // quantidade de pontos que devem ser atingidos para passar do nivel
    private int _metas = 0; // quantidade de metas que devem ser batidas
    private float _chances = 0; // quantidade de chances que podem ser usadas para finalizar
    private int _metasAtingidas = 0; // quantidade de metas que foram atingidas
    private int _pontos = 0; // quantidade de pontos atuais 
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

        if (_chances == 0)
        {
            GameOver();
        }

    }

    //trata dos eventos de cliques 
    private void PecaClickHandler(PecaBehaviourScript peca)
    {
        //verifica se a peca já está dentro das pecas selecionadas 
        if (!_pecasSelecionadas.Contains(peca))
        {

            int quantidadeDeElementos = _pecasSelecionadas.Count;

            //pega a peca atual
            //adiciona a lista de selecionadas se a lista está vazia
            if (quantidadeDeElementos == 0)
            {
                _pecasSelecionadas.Add(peca);
                _corDaLinha = peca.cor;

            }
            else
            //se a lista não está vazia verifica se a nova peca está             
            //pega a cor da peca - pega apenas uma vez - 
            //adejacente com a ultima peca e da mesma cor que a anterior
            //se a quantidade de pecas selecionadas for maior que um
            //, vai desenhando uma reta entre as pecas
            if (
                (peca.id == _pecasSelecionadas[quantidadeDeElementos - 1].id)
                )
            {
                //recupera a ultima peca
                PecaBehaviourScript ultimaPeca = _pecasSelecionadas[quantidadeDeElementos - 1];

                //verificando a distancia
                if (
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
        foreach(ReservatorioBehaviourScript contador in reservatorios)
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
        // para fim de testes uzarei a posicao 0

        Hashtable nivel = niveis[0] as Hashtable;
        // chances de objetivo
        _chances = (float)nivel["chances"];
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

        // metas 
        ArrayList metas = nivel["metas"] as ArrayList;
        if (metas.Count > 0)
        {
            this._metas = metas.Count; // quantidade de metas que devem ser batidas

            Hashtable meta; // meta atual do nivel selecionado
            ReservatorioBehaviourScript reservatorio = null; // resevatorio que será selecionado
            for (int i = 0; i < _metas; i++) // para cada meta ativo um contado
            {
                meta = metas[i] as Hashtable; // captura a meta atual
                //procura o contador correpondente

                foreach (ReservatorioBehaviourScript contador in reservatorios)
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
        _pontos += (quantidade * 10);
        pontos.text = _pontos.ToString()+" /"+_objetivo.ToString();

        // verifica se a pontuação foi suficiante para atingir os objetivos
        if (_pontos >= _objetivo & _metas == _metasAtingidas)
        {
            GameOver();
        }


    }

    //trata o evento de quando o toque é liberado
    // lugar onde é removido as peças seleciodas
    private void ToqueLiberadoHandler()
    {

        //verifica a quantidade de pecas ativas, ser for maior que 1 
        if (_pecasSelecionadas.Count > 1)
        {
            // pontua de acordo com a quantidade de pecas selecionadas
            Pontuar(_pecasSelecionadas.Count);
            //procura o contador corespondente
            ReservatorioBehaviourScript contador;
            foreach (ReservatorioBehaviourScript reservatorio in _listaDeReservatoriosAtivos)
            {
                if (reservatorio.id == _pecasSelecionadas[0].id)
                {
                    contador = reservatorio;
                    contador.Adicionar(_pecasSelecionadas.Count);
                    break;
                }
            }


            foreach (PecaBehaviourScript peca in _pecasSelecionadas)
            {
                //remove cada peca do tabuleiro
                _tabuleiro[peca.x, peca.y] = null;
                //remove das pecas ativas                
                _dotsAtivas.Remove(peca);
                //adiciona as pecas 
                _dotsPool.Add(peca);
                peca.Sair(); // retira a peca do tabuleiro                

            }

            // a cada combinação certa descontamos uma chance
            RemoverChance();

        }
        _pecasSelecionadas.Clear();// limpa lista     
        RePosicionar();// reposiciona as pecas do tabuleiro 
        RePreencher(); // recoloca as pecas que estão faltando no tabuleiro
        if (!VerificaPossibilidades()) Debug.Log("não existem nenhuma conexão possível");
    }
    //baixa as pecas 
    private void RePosicionar()
    {
        //procura por pecas espacos nulos
        for (int x = 0; x < _colunas; x++) //colunas
        {
            for (int y = 0; y < _linhas; y++)// linhas
            {
                //verifica se a posicao estfá vazia
                if (_tabuleiro[x, y] == null)
                {
                    // baixa as pecas que estão a cima dela
                    for (int j = y; j < _linhas; j++)
                    {
                        //verifica se é a ultima linha
                        if (j == _linhas - 1)
                        {
                            // _tabuleiro[x, j] = null; // deixa a casa de cima nula para a entrada de nova peca
                            break;
                        }
                        PecaBehaviourScript peca = _tabuleiro[x, j + 1];
                        if (peca)// verifica se tem alguma peca acima
                        { // quando encontra a primeira peca de cima vazia 
                          //para a interação e deixa a posicao de cima vazia
                            peca.SetPosicao(x, y);
                            _tabuleiro[x, y] = peca; // coloca a peca de cima no local da peca vazia encontrada
                            _tabuleiro[x, j + 1] = null; // a posicao de cima fica vazia 
                            break;
                        }// se não encontrar nenhuma acima dela para para a proxima
                    }

                }
            }
        }

    }

    // coloca as pecas que estão faltando no tabuleiro
    private void RePreencher()
    {
        //preenchendo o tabuleiro
        for (int x = 0; x < _colunas; x++)
        {
            for (int y = 0; y < _linhas; y++)
            {
                if (_tabuleiro[x, y] != null) continue;

                PecaBehaviourScript peca = _dotsPool[UnityEngine.Random.Range(0, _dotsPool.Count)];
                //adiciona a peca a lista de em uso
                _dotsAtivas.Add(peca);
                //remove a lista de poll
                _dotsPool.Remove(peca);

                peca.transform.parent = transform;
                //peca.gameObject.name = string.Format
                peca.transform.position = new Vector2(x * ESPACAMENTO_ENTRE_PECAS, y + 2);
                peca.SetPosicao(x, y);
                _tabuleiro[x, y] = peca;
                peca.gameObject.SetActive(true);

            }
        }
    }
    // controla a quantidade de metas atingidas
    private void MetaAtingida(ReservatorioBehaviourScript contador)
    {
       
        _metasAtingidas++;

        Debug.Log("### Quantidade de metas atingidas "+ _metasAtingidas);

        if(_metas == _metasAtingidas & _pontos >= _objetivo)
        {
            GameOver();
        }

    }
    // controla quando a meta é ultrapassada
    private void MetaUltrapassada(ReservatorioBehaviourScript contador)
    {
     
        GameOver();
    }

    //gameover
    private void GameOver()
    {
        GameOverUI.gameObject.SetActive(true);
    }

    //pausa o game
    public void Pausar()
    {
        //inverte o status da pause
        _jogando = !_jogando;
        Time.timeScale =(_jogando)?1.0f:0.0f;
        //ativa ou não a interface de pause
        PauseUI.gameObject.SetActive(!_jogando);
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
        TouchBehaviourScript.PecaTocada += PecaClickHandler;
        TouchBehaviourScript.ToqueLiberado += ToqueLiberadoHandler;

        ReservatorioBehaviourScript.MetaAtingida += MetaAtingida;
        ReservatorioBehaviourScript.MetaUltrapassada += MetaUltrapassada;

        //carregando os recursos
        CarregarRecursos();
        CarregarPool();

        //montando o grid
        _tabuleiro = new PecaBehaviourScript[_colunas, _linhas];
        //preenchendo o tabuleiro
        for (int x = 0; x < _colunas; x++)
        {
            for (int y = 0; y < _linhas; y++)
            {
                PecaBehaviourScript peca = _dotsPool[UnityEngine.Random.Range(0, _dotsPool.Count)];
                //adiciona a peca a lista de em uso
                _dotsAtivas.Add(peca);
                //remove a lista de poll
                _dotsPool.Remove(peca);
                peca.transform.parent = transform;
                //peca.gameObject.name = string.Format
                peca.transform.position = new Vector2(x * ESPACAMENTO_ENTRE_PECAS, y * ESPACAMENTO_ENTRE_PECAS);
                peca.SetPosicao(x, y);
                _tabuleiro[x, y] = peca;
                peca.gameObject.SetActive(true);

            }
        }
    }

    //carrega o pool de peças do jogo
    private void CarregarPool()
    {
        if (_dotsPrefabs.Length <= 0) throw new UnityException("recursos não carregados");

        _dotsPool = new List<PecaBehaviourScript>();

        for (int i = 0; i < _dotsPrefabs.Length; i++)
        {
            //para cada tipo de dot colocamos um reservatorio



            for (int j = 0; j < 10; j++)
            {
                PecaBehaviourScript peca = Instantiate(_dotsPrefabs[i]) as PecaBehaviourScript;
                peca.transform.parent = transform;
                peca.gameObject.SetActive(false);
                _dotsPool.Add(peca);

            }
        }

    }

    //verifica se ainda existem chances de alguem se conectar
    private bool VerificaPossibilidades()
    {
        for (int x = 0; x < _colunas; x++)
        { // para cada coluna
            for (int y = 0; y < _linhas; y++)
            {// para cada linha
                //verifica a peca de cima se for possivel
                if (y + 1 < _linhas)
                {
                    // verifica se esta peca tem o mesmo tipo da peca de cima
                    PecaBehaviourScript pecaAtual = _tabuleiro[x, y];
                    PecaBehaviourScript pecaDeCima = _tabuleiro[x, y + 1];

                    if (pecaAtual.id == pecaDeCima.id) return true; // pecas iguais retornar true 

                }
                //verifica a peca da direita
                if (x + 1 < _colunas)
                {
                    // verifica se esta peca tem o mesmo tipo da peca da direita
                    PecaBehaviourScript pecaAtual = _tabuleiro[x, y];
                    PecaBehaviourScript pecaDaDireita = _tabuleiro[x + 1, y];

                    if (pecaAtual.id == pecaDaDireita.id) return true; // pecas iguais retornar true 
                }

            }

        }

        return false; // não existe nenhuma possibilidade de conexão
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
        _reservatorioPrefab = Resources.Load<ReservatorioBehaviourScript>(caminho + "reservatorios/Reservatorio") as ReservatorioBehaviourScript;
    }

    //carrega os recursos padrao
    private void CarregarRecursos()
    {
        CarregarRecursos(PADRAO);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDestroy()
    {
        // reseta os eventos
        //configura eventos
        TouchBehaviourScript.PecaTocada -= PecaClickHandler;
        TouchBehaviourScript.ToqueLiberado -= ToqueLiberadoHandler;

        ReservatorioBehaviourScript.MetaAtingida -= MetaAtingida;
        ReservatorioBehaviourScript.MetaUltrapassada -= MetaUltrapassada;
    }

    

}