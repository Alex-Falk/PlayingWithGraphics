using System.Globalization;
using JeremyAnsel.Media.WavefrontObj;
using OpenTK.Mathematics;

namespace Testy;

public class OBJLibrary
{
    private const string MeshFilePath = "Meshes";
    private const string MeshFileExtension = ".obj";
    
    public static OBJLibrary Instance { get; } = new OBJLibrary();
    
    public Dictionary<string, OBJMesh> Meshes { get; private set; } = new Dictionary<string, OBJMesh>();
    
    private OBJLibrary()
    {
        LoadMeshes();
    }
    
    private void LoadMeshes()
    {
        if (!Directory.Exists(MeshFilePath))
        {
            return;
        }
        
        Directory.EnumerateFiles(MeshFilePath, $"*{MeshFileExtension}")
            .ToList()
            .ForEach(file =>
            {
                var meshName = Path.GetFileNameWithoutExtension(file);
                var mesh = LoadObjExternalPackage(file);
                
                Meshes[meshName] = mesh;
            });
    }
    
    public class OBJMesh
    {
        public List<Vector3> Vertices { get; } = new();
        public List<Vector3> Normals { get; } = new();
        public List<Vector2> TexCoords { get; } = new();
        public List<int> Indices { get; } = new();
        public List<float> VertexColours { get; } = new();
        public List<(int v, int vt, int vn)[]> Faces { get; } = new();
    }

    private static OBJMesh LoadObjExternalPackage(string filePath)
    {
        var file = ObjFile.FromFile(filePath);
        var result = new OBJMesh();
        
        int vertIndex = 0;
        foreach (var face in file.Faces)
        {
            foreach (var vertex in face.Vertices)
            {
                // Add position
                var pos = file.Vertices[vertex.Vertex - 1];
                result.Vertices.Add(new Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z));

                // Add normal
                var normal = file.VertexNormals[vertex.Normal - 1];
                result.Normals.Add(normal.Convert());
                
                // Add color
                result.VertexColours.Add(1.0f); // Default color value, can be changed later
                result.VertexColours.Add(1.0f);
                result.VertexColours.Add(1.0f);

                // Add index
                result.Indices.Add(vertIndex++);
            }
        }
        
        return result;
    }
    
    public static OBJMesh LoadOBJ(string filePath)
    {
        var mesh = new OBJMesh();
        foreach (var line in File.ReadLines(filePath))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("v "))
            {
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                mesh.Vertices.Add(new Vector3(
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                    float.Parse(parts[3], CultureInfo.InvariantCulture)));
            }
            else if (trimmed.StartsWith("vn "))
            {
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                mesh.Normals.Add(new Vector3(
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                    float.Parse(parts[3], CultureInfo.InvariantCulture)));
            }
            else if (trimmed.StartsWith("vt "))
            {
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                mesh.TexCoords.Add(new Vector2(
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture)));
            }
            else if (trimmed.StartsWith("f "))
            {
                var parts = trimmed.Substring(2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var face = parts.Select(p =>
                {
                    var indices = p.Split('/');
                    int v = int.Parse(indices[0]) - 1;
                    int vt = indices.Length > 1 && indices[1] != "" ? int.Parse(indices[1]) - 1 : -1;
                    int vn = indices.Length > 2 ? int.Parse(indices[2]) - 1 : -1;
                    return (v, vt, vn);
                }).ToArray();
                mesh.Faces.Add(face);
            }
        }
        return mesh;
    }
}