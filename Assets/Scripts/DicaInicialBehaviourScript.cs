using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Especiais
{
	FORTES, NEGRAS
}

public class DicaInicialBehaviourScript : MonoBehaviour
{



	private Vector2 posicaoDeSaida;

	public Text objetivo;

	public Text movimento;

	public Text meta_1, meta_2, meta_3, meta_4;

	public GameObject EspecialPanel, PecaForte, PecaNegra;

	private Especiais especial;

	public Especiais Especial {
		set { especial = value; 

//			//EspecialPanel.SetActive (true);
//
//			if (especial == Especiais.FORTES) {
//				PecaForte.SetActive (true);
//				PecaNegra.SetActive (false);
//			} else {
//				PecaForte.SetActive (false);
//				PecaNegra.SetActive (true);
//			}

		} get{  return especial; }
	}

	private Animator _animator; 

	// Use this for initialization
	void Start ()
	{

		_animator = GetComponent<Animator> ();

		//EspecialPanel.SetActive (false);
		//PecaForte.SetActive(false);
		//PecaNegra.SetActive (false);

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

