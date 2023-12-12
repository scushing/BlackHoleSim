using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Repaints screen with effects from list of materials
/// </summary>
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class MyEffectRenderer : MonoBehaviour
{
    // Taking list allows multiple effects to be rendered
    public List<Material> materials;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Known bug where all objects appear behind Black Hole. Cause is here. Black hole effect is applied
        // on top of initially rendered frame. Fix would require implenting a bvh a check for ray cast collision
        // with object vs effect radius before rendering effect. Not enough time to implement prior to project submission.

        if (materials != null && materials.Count != 0)
        {
            // Placeholder as screen is repainted
            RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            foreach (Material material in materials)
            {
                // Repaints screen with provied effect
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
