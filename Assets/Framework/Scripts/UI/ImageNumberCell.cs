using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class ImageNumberCell : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Image image;
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("123");
    }

    public string imgPath
    {
        set
        {
            if(image == null)
            {
                image = GetComponent<Image>();
            }
            image.sprite = Resources.Load<Sprite>(value);
            image.SetNativeSize();
        }
    }

    public bool show
    {
        set
        {
            if(canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            //canvasGroup.alpha = value ? 1 : 0;
            gameObject.SetActive(value);
        }
    }
}
