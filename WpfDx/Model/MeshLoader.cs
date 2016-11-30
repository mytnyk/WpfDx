using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace DemoAnimat.Model
{
    internal class MeshLoader
    {
        public Vector3[] Load(string file_path)
        {
            var data = new List<Vector3>();
            using (var file_stream = new StreamReader(file_path))
            {
                string line;
                while ((line = file_stream.ReadLine()) != null)
                {
                    var coords = line.Replace(',', '.').Split(' ').Select(v => float.Parse(v, CultureInfo.InvariantCulture)).ToArray();
                    data.Add(new Vector3(coords[0], coords[1], coords[2]));
                    data.Add(new Vector3(coords[3], coords[4], coords[5]));
                    data.Add(new Vector3(coords[6], coords[7], coords[8]));
                }
            }
            return data.ToArray();
        }
    }
}
