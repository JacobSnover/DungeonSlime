using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{

    private Dictionary<string, TextureRegion> _regions;

    /// <summary>
    /// Gets or sets the texture used for this texture atlas.
    /// </summary>
    public Texture2D Texture { get; set; }

    // Stores animation added to this atlas
    private Dictionary<string, Animation> _animations;

    /// <summary>
    /// Gets the dictionary of texture regions in this texture atlas, where the key is the name of the region and the value is the corresponding <see cref="TextureRegion"/> object.
    /// </summary>
    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a new texture atlas instance using the given texture.
    /// </summary>
    /// <param name="texture">The source texture represented by the texture atlas.</param>
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Adds a new texture region to the atlas with the specified name and rectangle coordinates.
    /// </summary>
    /// <param name="name">The name of the texture region.</param>
    /// <param name="x">The x-coordinate of the texture region.</param>
    /// <param name="y">The y-coordinate of the texture region.</param>
    /// <param name="width">The width of the texture region.</param>
    /// <param name="height">The height of the texture region.</param>
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        var region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    /// <summary>
    /// Retrieves the texture region associated with the specified name from the atlas.
    /// </summary>
    /// <param name="name">The name of the texture region to retrieve.</param>
    /// <returns>The texture region associated with the specified name.</returns>
    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    /// <summary>
    /// Removes the texture region associated with the specified name from the atlas.
    /// </summary>
    /// <param name="name">The name of the texture region to remove.</param>
    /// <returns>true if the texture region was found and removed; otherwise, false.</returns>
    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    /// <summary>
    /// Clears all texture regions from the atlas.
    /// </summary>
    public void Clear()
    {
        _regions.Clear();
    }

    /// <summary>
    /// Adds an animation to the texture atlas with the specified name and animation object.
    /// </summary>
    /// <param name="animationName">The name of the animation to add.</param>
    /// <param name="animation">The animation to add.</param>
    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }

    /// <summary>
    /// Retrieves the animation associated with the specified name from the texture atlas.
    /// </summary>
    /// <param name="animationName">The name of the animation to retrieve.</param>
    /// <returns>The animation associated with the specified name.</returns>
    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }

    /// <summary>
    /// Removes the animation associated with the specified name from the texture atlas.
    /// </summary>
    /// <param name="animationName">The name of the animation to remove.</param>
    /// <returns>true if the animation was found and removed; otherwise, false.</returns>
    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

    /// <summary>
    /// Creates a new texture atlas instance from an XML file that defines the texture and its regions.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                // The <Texture> element contains the content path for the Texture2D to load.
                // So we will retrieve that value then use the content manager to load the texture.
                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                // The <Regions> element contains individual <Region> elements, each one describing
                // a different texture region within the atlas.  
                //
                // Example:
                // <Regions>
                //      <Region name="spriteOne" x="0" y="0" width="32" height="32" />
                //      <Region name="spriteTwo" x="32" y="0" width="32" height="32" />
                // </Regions>
                //
                // So we retrieve all of the <Region> elements then loop through each one
                // and generate a new TextureRegion instance from it and add it to this atlas.
                var regions = root.Element("Regions")?.Elements("Region");

                if (regions != null)
                {
                    foreach(var region in regions)
                    {
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddRegion(name, x, y, width, height);
                        }
                    }
                }


                var animationElements = root.Element("Animations").Elements("Animation");

                if (animationElements != null)
                {
                    foreach(var animationElement in animationElements)
                    {
                        string name = animationElement.Attribute("name")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                        var frames = new List<TextureRegion>();

                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach(var frameElement in frameElements)
                            {
                                string regionName = frameElement.Attribute("region").Value;
                                TextureRegion region = atlas.GetRegion(regionName);
                                frames.Add(region);
                            }
                        }

                        Animation animation = new Animation(frames, delay);
                        atlas.AddAnimation(name, animation);
                    }
                }

                return atlas;
            }
        }
    }

    /// <summary>
    /// Creates a new sprite using the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="regionName">The name of the region to create the sprite with.</param>
    /// <returns>A new Sprite using the texture region with the specified name.</returns>
    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    /// <summary>
    /// Creates a new animated sprite using the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to use.</param>
    /// <returns>A new AnimatedSprite using the animation with the specified name.</returns>
    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);
    }

}
