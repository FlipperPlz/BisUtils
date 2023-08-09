namespace BisUtils.RvShape.Models.Data;

using Core.IO;
using Lod;

public interface IRvSelection
{
    IRvLod Parent { get; }
    List<byte> SelectedPoints { get; }
    List<bool> SelectedFaces { get; }
}

public class RvSelection : IRvSelection
{
    public IRvLod Parent { get; set; }

    public List<byte> SelectedPoints { get; private set; } = new();

    public List<bool> SelectedFaces { get; private set; } = new();

    public RvSelection(IRvLod lod) => Parent = lod;

    public void LoadSelection(BisBinaryReader reader, int sizeVert, int sizeFace)
    {
        if(sizeVert > 0)
        {
            SelectedPoints = reader.ReadIndexedList(it => it.ReadByte(), sizeVert).ToList();
        }

        EvaluateFaces(sizeFace);
        EvaluateFaces(sizeVert);
        if (sizeFace > 0)
        {
            SelectedFaces = reader.ReadIndexedList(it => it.ReadBoolean(), sizeFace).ToList();
        }
    }
    //TODO: Save

    private void EvaluatePoints(int count = -1) =>
        Evaluate(count, Parent.Points.Count, SelectedPoints, it => Enumerable.Repeat<byte>(0, it));

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
        SelectedPoints.Clear();
        SelectedFaces.Clear();
        EvaluatePoints();
        EvaluateFaces();
    }

}
