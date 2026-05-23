using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurnitureStore
{
    public class ProductImageManager
    {
        private static ProductImageManager _instance;
        private static readonly object _lock = new object();

        public static ProductImageManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProductImageManager();
                    }
                    return _instance;
                }
            }
        }

        private ProductImageManager() { }

        public string CalculateImageHash(byte[] imageData)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(imageData);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public string FindImageByHash(string targetHash)
        {
            if (string.IsNullOrEmpty(targetHash)) return null;

            string[] possibleDirs = {
                Path.Combine(Application.StartupPath, "Resources", "Product"),
                Path.Combine(Application.StartupPath, "..", "..", "Resources", "Product")
            };

            foreach (string dir in possibleDirs)
            {
                if (!Directory.Exists(dir)) continue;

                try
                {
                    foreach (string filePath in Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.ToLower().EndsWith(".jpg") || f.ToLower().EndsWith(".jpeg") || f.ToLower().EndsWith(".png")))
                    {
                        try
                        {
                            byte[] fileData = File.ReadAllBytes(filePath);
                            string fileHash = CalculateImageHash(fileData);

                            if (fileHash == targetHash)
                            {
                                return filePath;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception)
                {
                    // Пропускаем недоступные директории
                }
            }

            return null;
        }

        public string FindExistingImageByHash(string targetHash)
        {
            string imagePath = FindImageByHash(targetHash);
            return imagePath != null ? Path.GetFileName(imagePath) : null;
        }

        public string GetPlugImagePath()
        {
            string[] possiblePaths = {
                Path.Combine(Application.StartupPath, "Resources", "plug.png"),
                Path.Combine(Application.StartupPath, "..", "..", "Resources", "plug.png")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }

        public Image LoadImageFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        return Image.FromStream(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки загрузки
            }
            return null;
        }

        public Image LoadImageByHash(string photoHash)
        {
            if (string.IsNullOrEmpty(photoHash)) return null;

            string imagePath = FindImageByHash(photoHash);
            return imagePath != null ? LoadImageFromFile(imagePath) : null;
        }

        public async Task<Image> LoadImageByHashAsync(string photoHash)
        {
            return await Task.Run(() => LoadImageByHash(photoHash));
        }

        public string GenerateUniqueFileName(string baseName, string extension)
        {
            string[] possibleDirs = {
                Path.Combine(Application.StartupPath, "Resources", "Product"),
                Path.Combine(Application.StartupPath, "..", "..", "Resources", "Product")
            };

            string fileName = baseName + extension;
            int counter = 1;

            foreach (string dir in possibleDirs)
            {
                if (!Directory.Exists(dir)) continue;

                while (File.Exists(Path.Combine(dir, fileName)))
                {
                    fileName = $"{baseName}({counter}){extension}";
                    counter++;
                }
            }

            return fileName;
        }

        public bool SaveImageToProductDirectory(byte[] imageData, string fileName)
        {
            try
            {
                string sourceDir = Path.Combine(Application.StartupPath, "..", "..", "Resources", "Product");
                string debugDir = Path.Combine(Application.StartupPath, "Resources", "Product");

                Directory.CreateDirectory(sourceDir);
                Directory.CreateDirectory(debugDir);

                string sourcePath = Path.Combine(sourceDir, fileName);
                string debugPath = Path.Combine(debugDir, fileName);

                File.WriteAllBytes(sourcePath, imageData);
                File.WriteAllBytes(debugPath, imageData);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] CompressImageIfNeeded(byte[] imageData, long maxSizeBytes = 3 * 1024 * 1024)
        {
            if (imageData.Length <= maxSizeBytes)
                return imageData;

            try
            {
                using (var inputStream = new MemoryStream(imageData))
                using (var originalImage = Image.FromStream(inputStream))
                {
                    ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders()
                        .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                    if (jpgEncoder == null)
                        return null;

                    long quality = 90;

                    while (quality >= 10)
                    {
                        using (var ms = new MemoryStream())
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);

                            encoderParams.Param[0] = new EncoderParameter(
                                System.Drawing.Imaging.Encoder.Quality,
                                quality);

                            originalImage.Save(ms, jpgEncoder, encoderParams);

                            if (ms.Length <= maxSizeBytes)
                            {
                                return ms.ToArray();
                            }
                        }

                        quality -= 10;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public bool ValidateImageFile(string filePath)
        {
            try
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (fileExtension != ".jpg" &&
                    fileExtension != ".jpeg" &&
                    fileExtension != ".png")
                {
                    return false;
                }

                using (var image = Image.FromFile(filePath))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}