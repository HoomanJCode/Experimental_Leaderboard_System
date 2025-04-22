using System;
using UnityEngine;

public static class SpriteUtilities
{
    /// <summary>
    /// Converts a byte array to a Unity Sprite
    /// </summary>
    /// <param name="imageBytes">The raw image data</param>
    /// <param name="pixelsPerUnit">PPU setting for the sprite (default 100.0f)</param>
    /// <returns>A new Sprite or null if conversion fails</returns>
    public static Sprite BytesToSprite(byte[] imageBytes, float pixelsPerUnit = 100.0f)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            Debug.LogWarning("BytesToSprite: Empty or null byte array provided");
            return null;
        }

        try
        {
            // Create texture
            Texture2D texture = new Texture2D(2, 2);
            texture.name = "DynamicSpriteTexture";

            // Load image data
            if (!texture.LoadImage(imageBytes))
            {
                Debug.LogError("BytesToSprite: Failed to load image data");
                return null;
            }

            // Create sprite
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), // pivot at center
                pixelsPerUnit
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"BytesToSprite: Conversion failed - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Converts a Unity Sprite to a byte array (PNG format)
    /// </summary>
    /// <param name="sprite">The sprite to convert</param>
    /// <returns>Byte array of PNG data or null if conversion fails</returns>
    public static byte[] SpriteToByte(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("SpriteToByte: Null sprite provided");
            return null;
        }

        try
        {
            // Get the sprite's texture
            Texture2D texture = sprite.texture;

            // If the sprite uses only part of the texture, create a cropped version
            if (sprite.rect.width != texture.width || sprite.rect.height != texture.height)
            {
                var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                var pixels = texture.GetPixels(
                    (int)sprite.rect.x,
                    (int)sprite.rect.y,
                    (int)sprite.rect.width,
                    (int)sprite.rect.height
                );
                croppedTexture.SetPixels(pixels);
                croppedTexture.Apply();
                texture = croppedTexture;
            }

            // Encode to PNG
            return texture.EncodeToPNG();
        }
        catch (Exception ex)
        {
            Debug.LogError($"SpriteToByte: Conversion failed - {ex.Message}");
            return null;
        }
    }
}