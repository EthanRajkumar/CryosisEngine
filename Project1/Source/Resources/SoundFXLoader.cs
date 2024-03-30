using System.IO;
using System.Xml.Linq;

namespace CryosisEngine
{
    public class SoundFxLoader : ResourceLoader<SoundFxReference>
    {
        protected static SoundPool SoundPool { get; } = new SoundPool(64);

        public SoundFxLoader(string rootDirectory) : base(rootDirectory) { }

        /// <inheritdoc/>
        protected override SoundFxReference LoadItem(string contentPath)
        {
            string metaFilePath = $"{RootDirectory}/{WorkingDirectory}/{WorkingDirectory}_meta.xml", filePath, imagePath;

            string soundPath = contentPath + ".wav";
            float baseVolume = 1f, basePitch = 1f;

            if (File.Exists(metaFilePath))
            {
                string[] args = contentPath.Split('/');
                contentPath = args[args.Length - 1];

                XDocument xml = XDocument.Load(metaFilePath);
                XElement metaElement = xml.Root.Element(contentPath);

                if (metaElement != null)
                {
                    soundPath = metaElement.Attribute("FilePath").Value;
                    float.TryParse(metaElement.Attribute("BaseVolume").Value, out baseVolume);
                    float.TryParse(metaElement.Attribute("BasePitch").Value, out basePitch);
                }
            }

            return new SoundFxReference(SoundPool, $"{RootDirectory}/{WorkingDirectory}/{soundPath}", baseVolume, basePitch);
        }

        protected override void UnloadItem(SoundFxReference reference) { }
    }
}
