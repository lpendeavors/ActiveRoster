using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ActiveRoster.Helpers
{
  public static class ImageHelper
  {
    public static void ImportImages(string[] files)
    {
      foreach(string file in files)
      {
        string playerNumber = Path.GetFileNameWithoutExtension(file);
        if (Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".jpeg")
        {
          using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
          {
            using (Image original = Image.FromStream(fs))
            {
              var resizedImg = ResizeImage(original, 150, 150);
              DataHelper.SaveImage(playerNumber, resizedImg);
            }
          }
        }
      }
    }

    public static byte[] ImageToByteArray(Image imageIn)
    {
      using (var ms = new MemoryStream())
      {
        imageIn.Save(ms, ImageFormat.Jpeg);
        return ms.ToArray();
      }
    }

    public static Image ByteArrayToImage(this byte[] byteArrayIn)
    {
      using (var ms = new MemoryStream())
      {
        var returnImage = Image.FromStream(ms);
        return returnImage;
      }
    }

    private static Image ResizeImage(Image image, int width, int height)
    {
      var destRect = new Rectangle(0, 0, width, height);
      var destImage = new Bitmap(width, height);

      destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

      using (var graphics = Graphics.FromImage(destImage))
      {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighSpeed;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (var wrapMode = new ImageAttributes())
        {
          wrapMode.SetWrapMode(WrapMode.TileFlipXY);
          graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        }
      }

      return destImage;
    }
  }
}
