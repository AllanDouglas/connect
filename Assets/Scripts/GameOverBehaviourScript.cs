using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverBehaviourScript : MonoBehaviour {


	// label de objetivo
	public Text motivo;
	// label de objetivo
	public Text pontos;
	// label de movimentos
	public Text movimento;
	//label de pecas fotes
	public Text fortes;
	// label pecas negras
	public Text negras;
	// objetivo 01
	public Toggle objetivo;
	// objetivo 02
	public Toggle metas;
	// objetivo 03
	public Toggle movimentos;
	// objetivo 04
	public Toggle pecas_fortes;
	// objetivo 05
	public Toggle pecas_negras;
	// contadores
	public ContadorBehaviourScript[] contadores;

	// Use this for initialization
	void Start () {
	
	}

	public void ConfigurarContador(ContadorBehaviourScript contador, int valorAtual,int  minimo, int maximo){
		// procura o contador correspondente
		ContadorBehaviourScript _contador = null;

		for (int i = 0; i < contadores.Length; i++) {

			if (contador.id == contadores [i].id) {
				_contador = contadores [i];
				break;
			}

		}

		_contador.contador.maxValue = maximo;
		_contador.contador.value = valorAtual;

		_contador.meta.text = minimo + "|" + maximo;
		_contador.quantidadeAtual.text = valorAtual.ToString ();
		//_contador.gameObject.SetActive (false);
		//_contador.Configurar ();

	}
	

}
