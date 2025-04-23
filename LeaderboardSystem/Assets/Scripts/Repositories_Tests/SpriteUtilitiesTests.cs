using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpriteUtilitiesTests
{
    private Texture2D _testTexture;
    private byte[] _testTextureBytes;
    private Sprite _testSprite;

    [SetUp]
    public void Setup()
    {
        // Create a simple test texture (4x4 red square)
        _testTexture = new Texture2D(4, 4);
        for (int x = 0; x < _testTexture.width; x++)
        {
            for (int y = 0; y < _testTexture.height; y++)
            {
                _testTexture.SetPixel(x, y, Color.red);
            }
        }
        _testTexture.Apply();
        _testTextureBytes = _testTexture.EncodeToPNG();

        // Create a test sprite from the texture
        _testSprite = Sprite.Create(
            _testTexture,
            new Rect(0, 0, _testTexture.width, _testTexture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up any remaining test assets
        var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
        foreach (var tex in textures)
        {
            if (tex != null && tex.name.Contains("TestTexture"))
                Object.DestroyImmediate(tex);
        }

        var sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var sprite in sprites)
        {
            if (sprite != null && sprite.name.Contains("TestSprite"))
                Object.DestroyImmediate(sprite);
        }
    }
    [Test]
    public void BytesToSprite_WithValidData_ReturnsSprite()
    {
        // Arrange - Create a simple 2x2 texture with distinct colors
        var testTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        testTexture.SetPixel(0, 0, Color.red);
        testTexture.SetPixel(1, 0, Color.green);
        testTexture.SetPixel(0, 1, Color.blue);
        testTexture.SetPixel(1, 1, Color.white);
        testTexture.Apply();
        var textureBytes = testTexture.EncodeToPNG();

        // Act
        var result = SpriteUtilities.BytesToSprite(textureBytes);

        // Assert
        Assert.IsNotNull(result, "Sprite should not be null");
        Assert.IsNotNull(result.texture, "Sprite texture should not be null");
        Assert.AreEqual(2, result.texture.width, "Texture width should match");
        Assert.AreEqual(2, result.texture.height, "Texture height should match");

        // Verify the texture contains our test colors
        var resultTexture = result.texture;
        Assert.AreEqual(Color.red, resultTexture.GetPixel(0, 0), "Pixel (0,0) should be red");
        Assert.AreEqual(Color.green, resultTexture.GetPixel(1, 0), "Pixel (1,0) should be green");
        Assert.AreEqual(Color.blue, resultTexture.GetPixel(0, 1), "Pixel (0,1) should be blue");
        Assert.AreEqual(Color.white, resultTexture.GetPixel(1, 1), "Pixel (1,1) should be white");

        // Cleanup
        Object.DestroyImmediate(testTexture);
        Object.DestroyImmediate(result.texture);
    }


    [Test]
    public void BytesToSprite_WithNullInput_ReturnsNull()
    {
        // Act
        var result = SpriteUtilities.BytesToSprite(null);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void BytesToSprite_WithEmptyArray_ReturnsNull()
    {
        // Act
        var result = SpriteUtilities.BytesToSprite(new byte[0]);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void BytesToSprite_WithInvalidImageData_ReturnsNull()
    {
        // Arrange - Create data that's definitely not a valid image
        // Using random bytes that don't match any image header
        var invalidData = new byte[] {
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
    };

        // Act
        LogAssert.Expect(LogType.Error, "BytesToSprite: Failed to load image data");
        var result = SpriteUtilities.BytesToSprite(invalidData);
        // Assert
        Assert.IsNull(result, "Should return null for invalid image data");

        // Also test with corrupted PNG data
        var testTexture = new Texture2D(2, 2);
        var validData = testTexture.EncodeToPNG();
        // Corrupt the PNG header
        validData[0] = 0x00;
        validData[1] = 0x00;

        LogAssert.Expect(LogType.Error, "BytesToSprite: Failed to load image data");
        result = SpriteUtilities.BytesToSprite(validData);
        Assert.IsNull(result, "Should return null for corrupted PNG data");

        Object.DestroyImmediate(testTexture);
    }

    [Test]
    public void BytesToSprite_WithCustomPixelsPerUnit_UsesCorrectValue()
    {
        // Arrange
        const float testPPU = 50f;

        // Act
        var result = SpriteUtilities.BytesToSprite(_testTextureBytes, testPPU);

        // Assert
        Assert.AreEqual(testPPU, result.pixelsPerUnit);
    }
}