using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    //图片数字
    public class ImageNumber : MonoBehaviour
    {
        [SerializeField, Tooltip("图片预设体")]
        ImageNumberCell imgPrefab;

        [SerializeField, Tooltip("字符串key")]
        private string StringKey;
        [SerializeField, Tooltip("图片文字资源路径")]
        private string Path;
        [SerializeField, Tooltip("单个图片的宽")]
        private float Width;
        [SerializeField, Tooltip("缩放")]
        private float ObjectScale = 1;
        [SerializeField, Tooltip("字符串值")]
        private string StringValue = "";

        string imgValue = "";

        void Awake()
        {
        }

        void Start()
        {
        }

        ImageNumberCell CreateCell()
        {
            var ret = Instantiate(imgPrefab, transform);
            return ret;
        }

        public void SetString(string str)
        {
            if (string.IsNullOrEmpty(Path))
            {
                return;
            }

            if(str == imgValue)
            {
                return;
            }

            imgValue = str;

            RemoveAllChild();

            char[] chars = str.ToString().ToCharArray();

            char[] keys = StringKey.ToCharArray();
            float imgX = chars.Length * -Width / 2 + Width / 2;
            //遍历字符数组
            foreach (char c in chars)
            {
                //var cell = _pool.Get();
                var cell = CreateCell();
                cell.show = true;
                cell.transform.localScale = new Vector3(ObjectScale, ObjectScale);
                cell.name = c.ToString();
                cell.transform.SetAsLastSibling();

                //初始化位置
                var pos = new Vector3(imgX, 0);
                cell.transform.localPosition = pos;
                imgX += Width;

                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];

                    if (key == c)
                    {
                        cell.imgPath = Path + "/" + key;
                        break;
                    }
                }
            }
        }

        public void UpdateOnEditor()
        {
            this.SetString(StringValue);
        }

        void RemoveAllChild()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ImageNumber))]
    public class ImageNumberInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ImageNumber num = target as ImageNumber;

            num.UpdateOnEditor();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
#endif
}