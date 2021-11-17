using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public GameObject fill;
    public int width;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBar(float percent)
    {
        //fill.transform.localPosition = new Vector2((1 - percent) * -width, 0);

        fill.GetComponent<RectTransform>().sizeDelta = new Vector2(percent * width, fill.GetComponent<RectTransform>().rect.height);
    }
}
