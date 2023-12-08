using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class MyEffectRenderer : MonoBehaviour
{
    public List<Material> materials;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (materials != null && materials.Count != 0)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            foreach (Material material in materials)
            {
                Graphics.Blit(source, tmp, material);
                Graphics.Blit(tmp, source);
            }
            Graphics.Blit(source, destination);
            RenderTexture.ReleaseTemporary(tmp);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
