using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CryosisEngine
{
    public class TextureFont
    {
        public TextureAtlas Atlas { get; set; }

        public TextureFontChar[] Characters { get; set; }

        public int CharacterSpacing { get; set; }

        public int SpaceWidth { get; set; }

        public int LineHeight { get; set; }

        public TextureFont(TextureFontChar[] characters, int characterSpacing, int spaceWidth, int lineHeight, TextureAtlas atlas)
        {
            Atlas = atlas;
            Characters = characters;
            CharacterSpacing = characterSpacing;
            SpaceWidth = spaceWidth;
            LineHeight = lineHeight;
        }



        /// <summary>
        /// Measures the expected draw size of a string using this <see cref="TextureFont"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Vector2 MeasureString(string s)
        {
            string[] lines = s.Split('\n');

            Vector2 size = new Vector2(0, lines.Length * LineHeight);

            int currentLineWidth;

            for (int i = 0; i < lines.Length; i++)
            {
                currentLineWidth = 0;

                for (int j = 0; j < lines[i].Length; j++)
                {
                    switch (lines[i][j])
                    {
                        case '§':
                            {
                                j += 10;
                                break;
                            }
                        case ' ':
                            {
                                currentLineWidth += SpaceWidth;
                                break;
                            }
                        default:
                            {
                                currentLineWidth += Characters[lines[i][j]].KerningWidth + CharacterSpacing;
                                break;
                            }
                    }
                }

                size.X = Math.Max(size.X, currentLineWidth);
            }

            return size;
        }

        public int MeasureLine(string text)
        {
            int length = 0;

            for(int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '§':
                        {
                            i += 10;
                            break;
                        }
                    case ' ':
                        {
                            length += SpaceWidth;
                            break;
                        }
                    default:
                        {
                            length += Characters[text[i]].KerningWidth + CharacterSpacing;
                            break;
                        }
                }
            }

            return length;
        }

        public virtual void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color baseColor, bool isCentered = false, float scale = 1, float rotation = 0)
        {
            Color color = baseColor;

            string[] lines = text.Split('\n');
            int[] centeringOffsets = isCentered ? GetCenteringOffsets(lines) : new int[lines.Length];

            Vector2 drawOffset = default;

            float cos = (float)Math.Cos(rotation), sin = (float)Math.Sin(rotation);
            Vector2 drawPosition;

            for(int i = 0; i < lines.Length; i++)
            {
                drawOffset.X = 0;

                for(int j = 0; j < lines[i].Length; j++)
                {
                    switch (lines[i][j])
                    {
                        case '§':
                            {
                                uint.TryParse(lines[i].Substring(j + 1, 10), out uint colorVal);
                                color = new Color(colorVal);

                                j += 10;
                                break;
                            }
                        case ' ':
                            {
                                drawOffset.X += SpaceWidth;
                                break;
                            }
                        default:
                            {
                                drawPosition = position + (new Vector2(centeringOffsets[i] + (drawOffset.X * cos) - (sin * drawOffset.Y), (drawOffset.X * sin) + (drawOffset.Y * cos)) * scale);
                                spriteBatch.Draw(Atlas.BaseTexture, drawPosition, Atlas.Frames[Characters[lines[i][j]].FrameID].Bounds, color, rotation, default, scale, SpriteEffects.None, 1f);
                                drawOffset.X += Characters[lines[i][j]].KerningWidth + CharacterSpacing;
                                break;
                            }
                    }
                }

                drawOffset.Y += LineHeight;
            }
        }

        public virtual void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color baseColor, int[] lineOffsets, float scale = 1, float rotation = 0)
        {
            Color color = baseColor;

            string[] lines = text.Split('\n');

            Vector2 drawOffset = default;

            float cos = (float)Math.Cos(rotation), sin = (float)Math.Sin(rotation);
            Vector2 drawPosition;

            for (int i = 0; i < lines.Length; i++)
            {
                drawOffset.X = lineOffsets[i];

                for (int j = 0; j < lines[i].Length; j++)
                {
                    switch (lines[i][j])
                    {
                        case '§':
                            {
                                uint.TryParse(lines[i].Substring(j + 1, 10), out uint colorVal);
                                color = new Color(colorVal);

                                j += 10;
                                break;
                            }
                        case ' ':
                            {
                                drawOffset.X += SpaceWidth;
                                break;
                            }
                        default:
                            {
                                drawPosition = position + (new Vector2((drawOffset.X * cos) - (sin * drawOffset.Y), (drawOffset.X * sin) + (drawOffset.Y * cos)) * scale);
                                spriteBatch.Draw(Atlas.BaseTexture, drawPosition, Atlas.Frames[Characters[lines[i][j]].FrameID].Bounds, color, rotation, default, scale, SpriteEffects.None, 1f);
                                drawOffset.X += Characters[lines[i][j]].KerningWidth + CharacterSpacing;
                                break;
                            }
                    }
                }

                drawOffset.Y += LineHeight;
            }
        }

        public int[] GetBlockCenteringOffsets(string[] lines)
        {
            int maxWidth = 0;
            int[] lengths = new int[lines.Length];
            int[] offsets = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                lengths[i] = MeasureLine(lines[i]);
                maxWidth = Math.Max(maxWidth, lengths[i]);
            }

            for (int i = 0; i < lines.Length; i++)
                offsets[i] = (maxWidth - lengths[i]) / 2;

            return offsets;
        }

        public int[] GetCenteringOffsets(string[] lines)
        {
            int maxWidth = 0;
            int[] lengths = new int[lines.Length];
            int[] offsets = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                lengths[i] = MeasureLine(lines[i]);
                maxWidth = Math.Max(maxWidth, lengths[i]);
            }

            for (int i = 0; i < lines.Length; i++)
                offsets[i] = -lengths[i] / 2;

            return offsets;
        }

        public static TextureFont FromXml(XElement element, TextureAtlas atlas)
        {
            TextureFontChar[] characters = new TextureFontChar[256];

            foreach (XElement elem in element.Elements("C"))
            {
                characters[elem.Attribute("Char").Value[0]] = TextureFontChar.FromXml(elem);
            }

            int characterSpacing = XmlUtils.IntFromAttribute(element, "CharSpacing");
            int spaceWidth = XmlUtils.IntFromAttribute(element, "SpaceWidth");
            int lineHeight = XmlUtils.IntFromAttribute(element, "LineHeight");

            return new TextureFont(characters, characterSpacing, spaceWidth, lineHeight, atlas);
        }
    }

    public class TextureFontChar
    {
        public int FrameID { get; }

        public int KerningWidth { get; }

        public TextureFontChar(int frameID, int kerningWidth)
        {
            FrameID = frameID;
            KerningWidth = kerningWidth;
        }

        public static TextureFontChar FromXml(XElement element)
        {
            int frameID = XmlUtils.IntFromAttribute(element, "FrameID");
            int kerningwidth = XmlUtils.IntFromAttribute(element, "KernWidth");

            return new TextureFontChar(frameID, kerningwidth);
        }
    }
}