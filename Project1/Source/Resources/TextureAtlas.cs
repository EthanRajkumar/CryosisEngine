using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    /// <summary>
    /// Represents a region of a texture atlas.
    /// </summary>
    public class TextureFrame
    {
        /// <summary>
        /// The bounds (in pixels) of the frame. Bounds exceeding a <see cref="Texture2D"/>'s dimensions will fill resulting gaps with edge pixels.
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// The flip mode of this frame (horizontal/vertical). Does not support both forms of flipping--requires manual rotation of 180 degrees.
        /// </summary>
        public SpriteEffects FlipMode { get; set; }

        public TextureFrame(Rectangle bounds, SpriteEffects flipMode)
        {
            Bounds = bounds;
            FlipMode = flipMode;
        }
        
        /// <summary>
        /// Deserializes a <see cref="TextureFrame"/> from valid XML. Requires a <see cref="TextureLoader"/> to attach texture data.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static TextureFrame FromXml(XElement element)
            => new TextureFrame(XmlUtils.RectFromXml(element), (SpriteEffects)XmlUtils.IntFromAttribute(element, "F"));
    }

    /// <summary>
    /// Represents a texture atlas to derive multiple sprites and/or animation frames from.
    /// </summary>
    public class TextureAtlas
    {
        List<TextureFrame> _frames;

        Texture2D _baseTexture;

        /// <summary>
        /// The graphical data to source individual frames from.
        /// </summary>
        public Texture2D BaseTexture
        {
            get => _baseTexture;

            set
            {
                if (_baseTexture != null && Frames[0].Bounds == _baseTexture.Bounds)
                    Frames.RemoveAt(0);

                if (value != null)
                {
                    _baseTexture = value;
                    Frames.Insert(0, new TextureFrame(_baseTexture.Bounds, SpriteEffects.None));
                }
            }
        }

        /// <summary>
        /// The collection of frames associated with this <see cref="TextureAtlas"/>. The first frame will always contain the entire image.
        /// </summary>
        public List<TextureFrame> Frames
        {
            get => _frames;

            protected set
            {
                _frames = value ?? new List<TextureFrame>();

                if (BaseTexture != null && (_frames.Count == 0 || _frames[0].Bounds != BaseTexture.Bounds))
                    _frames.Insert(0, new TextureFrame(BaseTexture.Bounds, default));
            }
        }

        public TextureAtlas(Texture2D baseTexture, List<TextureFrame> frames)
        {
            Frames = frames;
            BaseTexture = baseTexture;
        }

        /// <summary>
        /// Deserializes a <see cref="TextureAtlas"/> from an XML element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static TextureAtlas FromXml(XElement element, Texture2D baseTexture)
        {
            List<TextureFrame> frames = new List<TextureFrame>();

            foreach (XElement elem in element.Elements())
                frames.Add(TextureFrame.FromXml(elem));

            return new TextureAtlas(baseTexture, frames);
        }
    }
}
