using OpenTK.Mathematics;

namespace Testy;

public class MaterialLibrary
{
    private const string MaterialFilePath = "Meshes";
    private const string MaterialFileExtension = ".mtl";
    
    public static MaterialLibrary Instance { get; } = new MaterialLibrary();
    
    public class Material
    {
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }
    }
    
    public Dictionary<string, Material> Materials { get; private set; } = new Dictionary<string, Material>();

    
    private MaterialLibrary()
    {
        LoadMaterials();
    }

    private void LoadMaterials()
    {
        if (!Directory.Exists(MaterialFilePath))
        {
            return;
        }
        
        Directory.EnumerateFiles(MaterialFilePath, $"*{MaterialFileExtension}")
            .ToList()
            .ForEach(file =>
            {
                var materialName = Path.GetFileNameWithoutExtension(file);
                var material = ParseMtlFile(file);
                
                Materials[materialName] = material;
            });
    }
    
    //thanks copilot:
    private Material ParseMtlFile(string filePath)
    {
        var material = new Material
        {
            Ambient = new Vector3(1.0f, 1.0f, 1.0f),
            Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
            Specular = new Vector3(1.0f, 1.0f, 1.0f)
        };

        foreach (var line in File.ReadLines(filePath))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("Ka "))
            {
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                material.Ambient = new Vector3(
                    float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }
            else if (trimmed.StartsWith("Kd "))
            {
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                material.Diffuse = new Vector3(
                    float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }
            else if (trimmed.StartsWith("Ks "))
            {
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                material.Specular = new Vector3(
                    float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }
        }
        return material;
    }
}