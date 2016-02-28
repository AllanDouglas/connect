using UnityEngine;
using System.Collections;

public class TouchBehaviourScript : MonoBehaviour
{

    //evento dispara pecas
    public delegate void OnPecaTocada(PecaBehaviourScript peca);
    //evento disparado quando tocamos uma peca
    public static event OnPecaTocada PecaTocada;
    //evento
    public delegate void OnTouchLiberado();
    //evento disparado quando o toqueo foi liberado depois de ter tocado em alguma peca
    public static event OnTouchLiberado ToqueLiberado;

    private PecaBehaviourScript _peca; // peca tocada atualmente
    private bool _iniciouToque = false; // flag que marca o momento do inicio do toque


    public TextMesh debug;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //implementações do android com touchs
        if (Input.touchCount > 0)
        {
            //inicio do toque
            if (Input.GetTouch(0).phase != TouchPhase.Ended)
            {
                Vector3 touch = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                
                Tocando(touch);
            }else
            //soltar
            if ( _iniciouToque)
            {
                Solto();
            }

        }


#if UNITY_EDITOR
        //implementações do mouse
        //verifica se o mouse está precionado
        if (Input.GetMouseButton(0))
        {
            Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Tocando(click);

        }
        //verifica se o botão do mouse foi souto e já havia iniciado o toque
        if (Input.GetMouseButtonUp(0) & _iniciouToque)
        {
            Solto();

        }
#endif

    }

    private void Solto()
    {
        _iniciouToque = false; // liberando o toque

        if (ToqueLiberado != null)
        {
            
            ToqueLiberado();
        }
    }

    private void Tocando(Vector3 posicao)
    {
        RaycastHit2D hit = Physics2D.Raycast(posicao,
                Vector2.zero, 2, LayerMask.GetMask("Pecas"));

        if (hit & PecaTocada != null)
        {
            _iniciouToque = true; // iniciando o toque
            PecaBehaviourScript novaPeca = hit.transform.GetComponent<PecaBehaviourScript>();
            //verifica se a peca tocada é a mesma anterior mente
			if ( PecaTocada != null & (novaPeca != _peca | novaPeca.EhCoringa()))
            {
				_peca = novaPeca;
                //dispara o evento
                PecaTocada(_peca);
				_peca = null;
            }

        }
    }
}
