using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class EffectRenderer : MonoBehaviour
{
    [SerializeField]
    private BHEffect effect;

    private static Material defaultMat;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (defaultMat == null)
        {
            defaultMat = new Material(Shader.Find("Unlit/Texture"));
        }
        if (Camera.current.depthTextureMode != DepthTextureMode.Depth)
        {
            Camera.current.depthTextureMode = DepthTextureMode.Depth;
        }

        if (effect == null)
        {
            Graphics.Blit(source, destination, defaultMat);
            return;
        }
        effect.Render(source, destination);
    }

    public static void RenderMaterials(RenderTexture source, RenderTexture destination, List<Material> materials)
    {
        List<RenderTexture> tmpTextures = new List<RenderTexture>();
        RenderTexture currentSrc = source;
        RenderTexture currentDst = null;

        for (int i = 0; i < materials.Count; i++)
        {
            Material mat = materials[i];
            if (i == materials.Count - 1)
            {
                currentDst = destination;
            }
            else
            {
                currentDst = RenderTexture.GetTemporary(currentDst.descriptor);
                tmpTextures.Add(currentDst);
            }

            Graphics.Blit(currentSrc, currentDst, mat);
            currentSrc = currentDst;
        }

        if (currentDst != destination || materials.Count == 0)
        {
            Graphics.Blit(currentSrc, destination, defaultMat);
        }

        for (int i = 0; i < tmpTextures.Count; i++)
        {
            RenderTexture.ReleaseTemporary(tmpTextures[i]);
        }
    }

}
