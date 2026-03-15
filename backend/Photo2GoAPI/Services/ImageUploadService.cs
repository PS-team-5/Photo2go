using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Photo2GoAPI.Configuration;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class ImageUploadService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly ImageUploadOptions _options;

    public ImageUploadService(IOptions<ImageUploadOptions> options)
    {
        _options = options.Value;
    }

    public ImageUploadValidationResult Validate(IFormFile? image)
    {
        if (image is null)
        {
            return ImageUploadValidationResult.Failure("Privalomas failas `image` nebuvo pateiktas.");
        }

        if (image.Length == 0)
        {
            return ImageUploadValidationResult.Failure("Pateiktas failas yra tuscias.");
        }

        if (image.Length > _options.MaxFileSizeInBytes)
        {
            return ImageUploadValidationResult.Failure(
                $"Failas per didelis. Maksimalus leidziamas dydis yra {_options.MaxFileSizeInBytes} baitu.");
        }

        var extension = Path.GetExtension(image.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return ImageUploadValidationResult.Failure(
                "Netinkamas failo formatas. Leidziami formatai: jpg, jpeg, png, webp.");
        }

        if (string.IsNullOrWhiteSpace(image.ContentType) || !AllowedMimeTypes.Contains(image.ContentType))
        {
            return ImageUploadValidationResult.Failure(
                "Netinkamas MIME tipas. Leidziami tipai: image/jpeg, image/png, image/webp.");
        }

        return ImageUploadValidationResult.Success(new AnalyzeImageResponse
        {
            OriginalFileName = image.FileName,
            MimeType = image.ContentType,
            Size = image.Length
        });
    }
}

public class ImageUploadValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }
    public AnalyzeImageResponse? Data { get; }

    private ImageUploadValidationResult(bool isValid, string? errorMessage, AnalyzeImageResponse? data)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        Data = data;
    }

    public static ImageUploadValidationResult Success(AnalyzeImageResponse data) =>
        new(true, null, data);

    public static ImageUploadValidationResult Failure(string errorMessage) =>
        new(false, errorMessage, null);
}
