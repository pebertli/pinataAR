using System.Collections;
using UnityEngine;
using TMPro;

public class CandyController : MonoBehaviour {

    public enum CandyType
    {
        Regular = 0,
        Splash
    }

    Renderer _render;
    TextMeshPro _text;

    public GameObject TextBonusPrefab;
    public GameObject CollectParticlePrefab;
    public CandyType Type;
    public int Points;



	// Use this for initialization
	void Start () {
        _render = GetComponent<Renderer>();        
	}

    IEnumerator Fade(bool destroy)
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = _render.material.color;
            c.a = f;
            _render.material.color = c;
            yield return new WaitForSeconds(.01f);
        }
        if(destroy)
            Destroy(gameObject);
    }

    public void Hit()
    {
        if (Type == CandyType.Splash)
        {
            ActivateSplash(7);
            StartCoroutine("Fade", true);
        }
        else if (Type == CandyType.Regular)
        {

            StartCoroutine("Fade", true);

            //show score earned with this candy
            //billboard
            var lookPos = transform.position - Camera.main.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            Instantiate<GameObject>(CollectParticlePrefab, transform.position, Quaternion.identity);
            GameObject g = Instantiate<GameObject>(TextBonusPrefab, transform.position, rotation);
            _text = g.GetComponentInChildren<TextMeshPro>();
            _text.SetText(((int)Points).ToString());
        }
    }

    private void ActivateSplash(int amount)
    {
        SplatController splatController = GameController.Instance.gameObject.GetComponent<SplatController>();
        splatController.DoSplash(amount);
    }
}
