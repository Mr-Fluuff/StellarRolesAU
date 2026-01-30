using Reactor.Utilities.Extensions;
using System;
using System.Reflection;
using UnityEngine;

namespace StellarRoles.Utilities.Assets
{
    /// <summary>
    /// A utility class for various sprite-related operations.
    /// </summary>
    public static class SpriteTools
    {
        /// <summary>
        /// Loads and returns a texture from a resource path using the specified assembly.
        /// </summary>
        /// <param name="resourcePath">The path to the resource within the assembly.</param>
        /// <param name="assembly">The assembly from which to load the resource.</param>
        /// <returns>A <see cref="Texture2D"/> object loaded from the specified resource path.</returns>
        /// <exception cref="ArgumentException">Thrown when the resource cannot be found in the specified assembly.</exception>
        public static Texture2D LoadTextureFromResourcePath(string resourcePath, Assembly assembly)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
            };
            var myStream = assembly.GetManifestResourceStream(resourcePath);
            if (myStream != null)
            {
                var buttonTexture = myStream.ReadFully();
                tex.LoadImage(buttonTexture, false);
            }
            else
            {
                throw new ArgumentException($"Resource not found: {resourcePath}");
            }

            tex.name = resourcePath;
            return tex;
        }

        /// <summary>
        /// Loads and returns a <see cref="Sprite"/> from a resource path using the specified assembly.
        /// </summary>
        /// <param name="resourcePath">The path to the resource within the assembly.</param>
        /// <param name="assembly">The assembly from which to load the resource.</param>
        /// <param name="pixelsPerUnit">The number of pixels per unit for the sprite.</param>
        /// <returns>A <see cref="Sprite"/> object created from the texture loaded from the specified resource path.</returns>
        public static Sprite LoadSpriteFromPath(string resourcePath, Assembly assembly, float pixelsPerUnit)
        {
            var tex = LoadTextureFromResourcePath(resourcePath, assembly);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.name = resourcePath;
            return sprite;
        }
    }
}
