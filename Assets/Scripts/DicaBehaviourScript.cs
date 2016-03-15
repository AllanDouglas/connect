using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Image))]

public class DicaBehaviourScript : MonoBehaviour
{

	public Text dica; // texto da dica que vai aparecer


	public Color cor{
		set { 
			_cor = value;
			_background.color = _cor;
		}
		get {return _cor; }
	}
	private Image _background; // imagem de background
	private Color _cor; // cor do bakground 
	private Animator _animator; // animator da da dica 

	void Start(){

		this._animator = GetComponent<Animator> (); // captura o component animator
		this._background = GetComponent<Image> (); // captura o component image

	}

	public void Entrar(){
		// executa a animação de entrada
		this._animator.SetTrigger ("entrando");
		
	}





}

