using UnityEngine;

public static class SpriteConverter
{
    public static Sprite BytesToSprite(byte[] bytes)
    {
        // Create a new texture
        var texture = new Texture2D(2, 2);

        // Load the image data into the texture
        texture.LoadImage(bytes);

        // Create a sprite from the texture
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));

        return sprite;
    }
}
