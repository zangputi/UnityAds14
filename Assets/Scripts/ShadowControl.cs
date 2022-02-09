using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowControl : MonoBehaviour
{
    public Light mLight;  // 主要用到光线组件的位置，与灯光无关

    public List<Material> mMatList = new List<Material>();  // 子节点所有模型涉及的材质的列表

    #region 内置函数

    private void Awake()
    {
        mLight = FindObjectOfType<Light>().GetComponent<Light>();
        SkinnedMeshRenderer[] renderlist = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var render in renderlist)
        {
            if (render == null)
                continue;

            var MyTest = render.materials;

            mMatList.AddRange(MyTest);

            //mMatList.Add(render.material);
        }
    }

    void Update()
    {
        UpdateShader();
    }

    /// <summary>
    /// 实时渲染影子
    /// </summary>
    private void UpdateShader()
    {
        Vector4 worldpos = transform.position;

        Vector4 projdir = mLight.transform.forward;

        foreach (var mat in mMatList)
        {
            if (mat == null)
                continue;

            mat.SetVector("_WorldPos", worldpos);
            mat.SetVector("_ShadowProjDir", projdir);
            // 影子的偏移，调整影子的相对位置
            mat.SetVector("_ShadowPlane", new Vector4(0.0f, 1.0f, 0.0f, -0.1f));
            mat.SetVector("_ShadowFadeParams", new Vector4(0.0f, 1.5f, 0.7f, 0.0f));
            mat.SetFloat("_ShadowFalloff", 0.5f);  // 影子的深浅，数字越大，影子越浅 取值范围0-1
        }
    }

    #endregion
}
