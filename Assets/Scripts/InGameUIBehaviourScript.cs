using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameUIBehaviourScript : MonoBehaviour
{
	[Header("Labels")]
	public Text pontos;
	public Text objetivo;
	public Text movimentos;
	public Text pecasForteRestantes;
	public Text pecasNegrasRestantes;

	[Header("Slider dos pontos")]
	public Slider barraDepontos;


	[Header("Botões")]
	public Button pauseButton;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

