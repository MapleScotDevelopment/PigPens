public class FenceLocation
{
    public enum Type
    {
        Horizontal,
        Vertical
    }

    public Type fenceType;
    public int row;
    public int col;

    public FenceLocation(Type fenceType, int row, int col)
    {
        this.fenceType = fenceType;
        this.row = row;
        this.col = col;
    }
}