using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Animator))]

public class DicaBehaviourScript : MonoBehaviour
{

	public Text dica; // texto da dica que vai aparecer

	private Animator _animator; // animator da da dica 

	void Start(){

		this._animator = GetComponent<Animator> (); // captura o component animator

	}

	public void Entrar(){
		// executa a animação de entrada
		this._animator.SetTrigger ("entrando");
		
	}





}

