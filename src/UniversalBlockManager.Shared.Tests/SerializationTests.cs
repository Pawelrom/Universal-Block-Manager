using System.IO;
using UniversalBlockManager.Shared.Models;
using Xunit;

namespace UniversalBlockManager.Shared.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void TestSerializationRoundTrip()
        {
            var block = new BlockDefinition
            {
                Name = "Gniazdko_230V",
                Units = "mm",
                Header = new Header
                {
                    BasePoint = new Point { X = 10.5, Y = 10.5 },
                    ScaleFactor = 1.0
                }
            };

            block.Layers.Add(new Layer { Name = "Symbol" });
            block.Attributes.Add(new BlockAttribute { Key = "Producent", Value = "Schneider" });
            
            block.Geometry.Add(new Line { Layer = "Obrys", Start = new Point { X = 0, Y = 0 }, End = new Point { X = 10, Y = 10 } });
            block.Geometry.Add(new Circle { Layer = "Symbol", Center = new Point { X = 5, Y = 5 }, Radius = 30 });
            block.Geometry.Add(new PathElement { Layer = "Obrys", Data = "M 10 10 L 20 20" });

            const string testFile = "test_block.xml";
            block.Save(testFile);

            Assert.True(File.Exists(testFile));

            var loaded = BlockDefinition.Load(testFile);

            Assert.Equal(block.Name, loaded.Name);
            Assert.Equal(block.Geometry.Count, loaded.Geometry.Count);
            Assert.IsType<Line>(loaded.Geometry[0]);
            Assert.IsType<Circle>(loaded.Geometry[1]);
            Assert.IsType<PathElement>(loaded.Geometry[2]);

            // Clean up
            if (File.Exists(testFile)) File.Delete(testFile);
        }
    }
}
