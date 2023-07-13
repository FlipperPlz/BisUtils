namespace BisUtils.P3D.Models.Data;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using FResults;
using Lod;
using Options;

public interface IRVSelection
{
    IRVLod Parent { get; }
    IReadOnlyList<byte> SelectedVertices { get; }
    IReadOnlyList<bool> SelectedFaces { get; }
}

public class RVSelection : IRVSelection
{
    public IRVLod Parent { get; set; }
    public IReadOnlyList<byte> SelectedVertices => vertices.AsReadOnly();
    public IReadOnlyList<bool> SelectedFaces => faces.AsReadOnly();

    private readonly List<byte> vertices = new();
    private readonly List<bool> faces = new();

    public RVSelection(IRVLod lod)
    {
        Parent = lod;
        EvaluateLod();
    }

    private void EvaluatePoints(int count = -1) =>
        Evaluate(count, Parent.Vertices.Count, vertices, it => Enumerable.Repeat<byte>(0, it));

    private void EvaluateFaces(int count = -1) =>
        Evaluate(count, Parent.Faces.Count, faces, it => Enumerable.Repeat(false, it));

    private static void Evaluate<T>(int count, int parentCount, List<T> collection, Func<int, IEnumerable<T>> generateItems)
    {
        var newItems = Math.Max(parentCount, count);

        if(newItems != collection.Count)
        {
            collection.AddRange(generateItems(newItems - collection.Count));
        }
    }

    private void EvaluateLod()
    {
        vertices.Clear();
        faces.Clear();
        EvaluatePoints();
        EvaluateFaces();
    }

}
