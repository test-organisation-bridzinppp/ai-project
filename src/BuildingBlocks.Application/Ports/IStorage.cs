namespace BuildingBlocks.Application.Ports
{
    public interface IStorage
    {
        Task<IEnumerable<string>> GetFilesNames(string path, string extension);
        Task<StorageFile> GetFile(string path, string fileName);
        Task<bool> SaveFile(string path, string fileName, byte[] content);
    }

    public record StorageFile(string FileName, byte[] Content);
    
}
