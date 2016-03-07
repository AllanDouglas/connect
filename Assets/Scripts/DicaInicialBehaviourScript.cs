using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DicaInicialBehaviourScript : MonoBehaviour
{

	private Vector2 posicaoDeSaida;

	public Text objetivo;

	public Text movimento;

	public Text meta_1, meta_2, meta_3, meta_4;


	private Animator _animator; 

	// Use this for initialization
	void Start ()
	{

		_animator = GetComponent<Animator> ();

	}	

	public void Desativar(){

		gameObject.SetActive (false);

	}

	public void Sair(){

		_animator.SetTrigger ("saindo");

		
	}

	private IEnumerator Saindo(){
	
		while (transform.position.y > -912) {
			
			transform.position = Vector2.Lerp (transform.position, posicaoDeSaida, Time.deltaTime * 2);
			yield return new WaitForSeconds(.2f);
		}

		yield return null;
	}

}

