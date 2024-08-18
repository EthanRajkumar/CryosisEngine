using System;

using System.IO;
using System.Xml.Linq;

namespace CryosisEngine
{
    public class TextureFontLoader : ResourceLoader<TextureFont>
    {
        public TextureAtlasLoader AtlasLoader { get; set; }

        public TextureFontLoader(string rootDirectory, TextureAtlasLoader atlasLoader) : base(rootDirectory)
        {
            AtlasLoader = atlasLoader;
        }

        /// <inheritdoc/>
        protected override TextureFont LoadItem(string contentPath)
        {
            string metaFilePath = $"{RootDirectory}/{WorkingDirectory}/{WorkingDirectory}_meta.xml", filePath, atlasPath;

            if (File.Exists(metaFilePath))
            {
                string[] args = contentPath.Split('/');
                contentPath = args[args.Length - 1];

                XDocument xml = XDocument.Load(metaFilePath);
                XElement metaElement = xml.Root.Element(contentPath);

                if (metaElement != null)
                    filePath = metaElement.Attribute("FontPath").Value;
                else
                    filePath = $"{contentPath}.xml";
            }
            else
                filePath = $"{contentPath}.xml";

            if (!File.Exists($"{RootDirectory}/{WorkingDirectory}/{filePath}"))
                filePath = $"DefaultFont.xml";

            XDocument fontDocument = XDocument.Load($"{RootDirectory}/{WorkingDirectory}/{filePath}");
            atlasPath = fontDocument.Root.Attribute("AtlasPath").Value;

            return TextureFont.FromXml(fontDocument.Root, AtlasLoader.LoadContent(atlasPath));
        }

        protected override void UnloadItem(TextureFont font)
        {
            AtlasLoader.UnloadContent(font.Atlas);
        }
    }
}