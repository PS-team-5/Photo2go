namespace Photo2GoAPI.Configuration;

public class ImageUploadOptions
{
    public const string SectionName = "ImageUpload";

    public long MaxFileSizeInBytes { get; set; } = 5 * 1024 * 1024;
}
