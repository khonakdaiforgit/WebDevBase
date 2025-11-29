namespace MyApp.WebMVC.Extensions
{
    public static class FileUploadExtensions
    {
        private static readonly string UploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        /// <summary>
        /// آپلود تصویر و برگرداندن URL عمومی
        /// </summary>
        /// <param name="file">فایل آپلود شده</param>
        /// <param name="subFolder">مثل: menu-items, gallery, logos</param>
        /// <returns>URL کامل قابل استفاده در img src</returns>
        public static async Task<string?> UploadImageAsync(this IFormFile file, string subFolder = "general")
        {
            if (file == null || file.Length == 0) return null;

            // فقط عکس قبول کن
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return null;

            // ساخت پوشه اگر وجود نداشت
            var folderPath = Path.Combine(UploadsFolder, subFolder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // نام فایل یکتا (با تاریخ و رندوم)
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            // ذخیره فایل
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // برگرداندن URL عمومی
            return $"/uploads/{subFolder}/{fileName}";
        }
    }
}
