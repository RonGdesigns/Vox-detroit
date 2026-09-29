namespace VoxDetroit.Voxels
{
    public interface IVoxelBlockSource
    {
        bool TryGetBlock(int worldX, int worldY, int worldZ, out BlockId block);
    }
}
