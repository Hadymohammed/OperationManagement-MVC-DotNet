namespace OperationManagement.Data.Common
{
    public class FilesManagement
    {
        public static async Task<string> ComponentPhoto(IFormFile file, string ComponentName, int ComponentId, int PhotoNumber)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string ext = Path.GetExtension(file.FileName);
                    var fileName = ComponentName + "_" + ComponentId.ToString() + "_" + PhotoNumber.ToString() + ext;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\resources\\ComponentPhotos", fileName);//NeedCompletePath
                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSrteam);
                    }
                    return ".\\resources\\ComponentPhotos\\" + fileName;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }
        public static async Task<string> SaveOrderAttachement(IFormFile file, string OrderNumber, string title)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string ext = Path.GetExtension(file.FileName);
                    var fileName = OrderNumber + "_" + title + ext;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\resources\\OrderAttachements", fileName);//NeedCompletePath
                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSrteam);
                    }
                    return ".\\resources\\OrderAttachements\\" + fileName;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        public static bool DeleteFile(string filePath)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\", filePath);
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public static async Task<bool> SaveFileAsync(IFormFile file, string filePath)
        {
            try
            {
                using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSrteam);
                }
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
            return false;
        }
    }
}
