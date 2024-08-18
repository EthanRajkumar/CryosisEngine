using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    public class TextComponent : GameComponent
    {
        string _text;

        TextCenteringMode _centeringMode;

        TextureFont _font;

        public string Text
        {
            get => _text;

            set
            {
                if (value != _text)
                {
                    _text = value;
                    CalculateCenteringOffsets();
                }
            }
        }

        public Color Color { get; set; }

        public int[] CenteringOffsets { get; set; }

        public enum TextCenteringMode
        {
            None,
            Origin,
            Block
        }

        public TextCenteringMode CenteringMode
        {
            get => _centeringMode;

            set
            {
                if (value != _centeringMode)
                {
                    _centeringMode = value;
                    CalculateCenteringOffsets();
                }
            }
        }

        public TextureFont Font
        {
            get => _font;

            set
            {
                if (_font != value)
                {
                    _font = value;
                    CalculateCenteringOffsets();
                }
            }
        }

        public TextComponent(string text, Color color, TextCenteringMode centeringMode) : base()
        {
            _text = text;
            Color = color;
            _centeringMode = centeringMode;
        }

        public void CalculateCenteringOffsets()
        {
            string[] lines = Text.Split('\n');

            if (CenteringMode == TextCenteringMode.None)
            {
                CenteringOffsets = new int[lines.Length];
            }
            else
            {
                CenteringOffsets = CenteringMode == TextCenteringMode.Block ? Font.GetBlockCenteringOffsets(lines) : Font.GetCenteringOffsets(lines);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1)
        {
            if (Font == null)
                return;

            Point location = ((Parent.Transform.Position - offset) * viewportScale).ToPoint();
            float scale = Parent.Transform.GlobalScale * viewportScale;
            alpha *= Alpha;

            Font.DrawString(spriteBatch, Text, location.ToVector2(), Color * alpha, CenteringOffsets, scale, Parent.Transform.GlobalRotation);
        }

        public override void LoadContent(GameServiceContainer services)
        {
            if (ContentPaths == null)
                return;

            Font = services.GetService<TextureFontLoader>().LoadContent(ContentPaths[0]);
        }

        public override void UnloadContent(GameServiceContainer services)
        {
            services.GetService<TextureFontLoader>().UnloadContent(Font);
        }

        public static TextComponent FromXml(XElement element)
        {
            string text = element.Attribute("Text").Value;
            uint colorValue = XmlUtils.UIntFromAttribute(element, "ColorVal");
            Enum.TryParse(element.Attribute("CenteringMode").Value, out TextCenteringMode centeringMode);

            string[] contentPaths = XmlUtils.AttributeValue(element, "ContentPaths").Split('|');

            return new TextComponent(text, new Color(colorValue), centeringMode) { ContentPaths = contentPaths };
        }
    }
}
