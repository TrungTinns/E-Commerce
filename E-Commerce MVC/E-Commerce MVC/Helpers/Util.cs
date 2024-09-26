using System.Text;

namespace E_Commerce_MVC.Helpers
{
    public class Util
    {
        public static string UploadImg(IFormFile Image, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Image.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Image.CopyTo(myfile);
                }
                return Image.FileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GenerateRandomKey(int lenght = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhnujmikolpPLOKIMJUNHYBGTVFRCDEXSWZAQ!@#$%^&*";
            var sb = new StringBuilder();
            var rd = new Random();
            for(int i = 0; i< lenght; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
    }
}
