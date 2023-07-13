namespace BisUtils.P3D.Models.Data;

using Core.IO;
using Lod;

public interface IRVSelection
{
    IRVLod Parent { get; }
    List<byte> SelectedVertices { get; }
    List<bool> SelectedFaces { get; }
}

public class RVSelection : IRVSelection
{
    public IRVLod Parent { get; set; }

    public List<byte> SelectedVertices { get; private set; } = new();

    public List<bool> SelectedFaces { get; private set; } = new();

    public RVSelection(IRVLod lod) => Parent = lod;

    public void LoadSelection(BisBinaryReader reader, int sizeVert, int sizeFace, int sizeNorm)
    {
        if(sizeVert > 0)
        {
            SelectedVertices = reader.ReadIndexedList(it => it.ReadByte()).ToList();
        }

        EvaluateFaces(sizeFace);
        EvaluateFaces(sizeVert);
        if (sizeFace > 0)
        {
            SelectedFaces = reader.ReadIndexedList(it => it.ReadBoolean()).ToList();
        }
    }
    //TODO: Save

    private void EvaluatePoints(int count = -1) =>
        Evaluate(count, Parent.Vertices.Count, SelectedVertices, it => Enumerable.Repeat<byte>(0, it));

    private void EvaluateFaces(int count = -1) =>
        Evaluate(count, Parent.Faces.Count, SelectedFaces, it => Enumerable.Repeat(false, it));

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
        SelectedVertices.Clear();
        SelectedFaces.Clear();
        EvaluatePoints();
        EvaluateFaces();
    }

}
