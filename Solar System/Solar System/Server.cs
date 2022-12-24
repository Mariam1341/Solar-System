using Microsoft.VisualBasic.FileIO;

namespace Solar_System
{
    public class Server
    {
        public static Dictionary<string, float> GetPlanetInfo(string planetName)
        {
            Dictionary<string, float> data = new();

            using (TextFieldParser parser = new(@"../../../database.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    if (parser == null)
                        break;

                    string[] fields = parser.ReadFields();
                    if (fields[0].Equals(planetName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        data.Add("radius", float.Parse(fields[1]));
                        data.Add("disFromSun", float.Parse(fields[2]));
                        data.Add("rotationPeriod", float.Parse(fields[3]));
                        data.Add("orbitalPeriod", float.Parse(fields[4]));
                        data.Add("axialTilt", float.Parse(fields[5]));
                    }
                }
            }
            return data;
        }

        public static string GetTexturePath(string planetName)
        {
            return "../../../Textures/" + planetName.ToLower() + ".jpg";
        }

        public static string GetShaderPath(string shaderName, string typeFolder)
        {
            return "../../../Shaders/" + typeFolder + "/" + shaderName.ToLower();
        }



    }
}
