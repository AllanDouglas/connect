using System;
using System.Collections.Generic;
using UnityEngine;

public class TabuleiroScript
{

	// define o espaçamento lateral dos itens do grid
	private const float ESPACAMENTO_ENTRE_PECAS = 1.2f;

	public int _colunas;
	public int _linhas;
	public Transform transformPai;

//	private PecaBehaviourScript[] _dotsPrefabs; // lista de prefabs dos circulos
//	private ReservatorioBehaviourScript _reservatorioPrefab; // prafab do reservatorio
	private List<ContadorBehaviourScript> _listaDeReservatoriosAtivos = new List<ContadorBehaviourScript>(); // lista dos reservatorios
	private List<PecaBehaviourScript> _dotsPool = new List<PecaBehaviourScript>(); //pool de dotos
	private List<PecaBehaviourScript> _dotsAtivas = new List<PecaBehaviourScript>(); // pecas que estão no grid
	// retorna os dots ativos
	public List<PecaBehaviourScript> PecasAtivas {
		get { return  _dotsAtivas; }
	}
	private PecaBehaviourScript[,] _tabuleiro; // tabuleiro
	public  PecaBehaviourScript[,] Tabuleiro{
		get { return _tabuleiro; }
	}

//	private List<PecaBehaviourScript> _pecasSelecionadas = new List<PecaBehaviourScript>(); // pecas que foram tocadas
//	private Color _corDaLinha; // cor da linha        

	// adiciona uma peca ao pool
	public void AdicionaPecasPool(PecaBehaviourScript peca)
	{
		_dotsPool.Add (peca);
	}

	// adiciona as pecas ativas
	public void AdicionaPecasAtivas(PecaBehaviourScript peca)
	{
		_dotsAtivas.Add (peca);
	}

	//remove lista de pecas ativas e devolve para o pool
	public void Remover(List<PecaBehaviourScript> pecas)
	{
		foreach (PecaBehaviourScript peca in pecas)
		{
			Remover (peca);
		}
	}

	public void Remover(PecaBehaviourScript peca){
		//remove cada peca do tabuleiro
		_tabuleiro[peca.x, peca.y] = null;
		//remove das pecas ativas                
		_dotsAtivas.Remove(peca);
		//adiciona as pecas 
		_dotsPool.Add(peca);
		peca.Sair(); // retira a peca do tabuleiro                
	}

	// reposiciona as pecas do tabuleiro
	public void Reposicionar()
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

	//preencher
	public void Preencher(int colunas, int linhas)
	{

		_colunas = colunas;
		_linhas = linhas;

		_tabuleiro = new PecaBehaviourScript[colunas, linhas];
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
				peca.transform.parent = transformPai;
				//peca.gameObject.name = string.Format
				peca.transform.position = new Vector2(x * ESPACAMENTO_ENTRE_PECAS, y );
				peca.SetPosicao(x, y);
				_tabuleiro[x, y] = peca;
				peca.gameObject.SetActive(true);

			}
		}	
	} 

	// repreenche o tabuleiro
	public void Repreencher()
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

				peca.transform.parent = transformPai;
				//peca.gameObject.name = string.Format
				peca.transform.position = new Vector2(x * ESPACAMENTO_ENTRE_PECAS, y + 2);
				peca.SetPosicao(x, y);
				_tabuleiro[x, y] = peca;
				peca.gameObject.SetActive(true);

			}
		}
	}
	// verifica se ainda existem possibilidades
	public bool VerificaPossibilidades()
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

					if (pecaAtual.id == pecaDeCima.id &
						pecaAtual.Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA &
						pecaDeCima.Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA) 
						return true; // pecas iguais retornar true 

				}
				//verifica a peca da direita
				if (x + 1 < _colunas)
				{
					// verifica se esta peca tem o mesmo tipo da peca da direita
					PecaBehaviourScript pecaAtual = _tabuleiro[x, y];
					PecaBehaviourScript pecaDaDireita = _tabuleiro[x + 1, y];

					if (pecaAtual.id == pecaDaDireita.id &
						pecaAtual.Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA &
						pecaDaDireita.Condicao != PecaBehaviourScript.CondicaoEspecial.NEGRA
					) return true; // pecas iguais retornar true 
				}

			}

		}

		return false; // não existe nenhuma possibilidade de conexão
	}
	// mistura os itens do tabuleiro
	public void Misturar()
	{

		// varrendo as colunas
		for(int x = 0; x < _colunas; x++)
		{
			// varrendo as linhas
			for(int y =0; y < _linhas; y++)
			{
				// randomiza uma posicao nova
				int novoX = UnityEngine.Random.Range(0,_colunas-1);
				int novoY = UnityEngine.Random.Range(0, _linhas - 1);

				//peca atual
				PecaBehaviourScript pecaAtual = _tabuleiro[x, y];
				//peca randomizada
				PecaBehaviourScript pecaRandomizada = _tabuleiro[novoX, novoY];

				// a peça atual fica no lugar da randomizada 
				_tabuleiro[novoX, novoY] = pecaAtual;
				pecaAtual.SetPosicao(novoX,novoY);
				// a peça da posicao randomizada fica no lugar do da peca atual
				_tabuleiro[x, y] = pecaRandomizada;
				pecaRandomizada.SetPosicao(x,y);
				// troca a posicao 
				Vector3 novaPosicao = pecaRandomizada.transform.position; // guarda a posicao da peca randomizada
				pecaRandomizada.transform.position = pecaAtual.transform.position; // peca randomizada fica no lugar da peca de 
				pecaAtual.transform.position = novaPosicao; // peca atual fica no lugar da peca randomizada

			}
		}        

	}

}
