using UnityEngine;
using UnityEngine.UI;

public class PreDicaBehaviourScritp : MonoBehaviour
{

	public Button botaoOk;

	// Use this for initialization
	void Start ()
	{
		botaoOk.onClick .AddListener(Ok);

		Time.timeScale = 0.0f;

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void Ok(){
		Time.timeScale = 1.0f;
		gameObject.SetActive (false);
	}


}

