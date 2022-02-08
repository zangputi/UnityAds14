using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Game
{
    public class HarmText : MonoBehaviour
    {
        [SerializeField, Tooltip("数字组件")]
        ImageNumber number;

        [SerializeField, Tooltip("动画组件")]
        Animator animator;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Awake()
        {
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void PlayHarm()
        {
            gameObject.SetActive(true);
            int num = Random.Range(800, 900);
            number.SetString("b" + num);
            animator.Play("Pop", 0, 0.0f);

            CanvasGroup cg = number.transform.GetComponent<CanvasGroup>();
            cg.alpha = 1.0f;
        }
    }
}