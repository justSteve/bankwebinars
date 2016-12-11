using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Glimpse.AspNet.Tab;
using Image = System.Drawing.Image;
using Size = GemBox.Document.Size;

namespace CUWebinars.Web.Helpers
{
    public class DomainHelpers
    {
        public static string BuildAdditionalLocationsCaption(OrderRow orderRow)
        {
            var caption = new StringBuilder(20);

            foreach (var additionalLocation in orderRow.AdditionalLocation)
            {
                caption.AppendFormat("{0},", additionalLocation.Email);
            }

            return caption.Length > 0 ? caption.ToString().Substring(0, caption.Length - 1) : string.Empty;
        }

        public static Size ResizePhoto(string presenterPhotoFull)
        {
            var pic = presenterPhotoFull.Replace("https://www.bankwebinars.com/", "~/");
            if (presenterPhotoFull.Contains("cuwebinars"))
                pic = presenterPhotoFull.Replace("https://www.cuwebinars.com/","~/");
            var image = Image.FromFile(HttpContext.Current.Server.MapPath(pic));

            Size original = new Size(image.Width, image.Height);

            int maxSize = 100;

            float percent = (new List<float> { (float)maxSize / (float)original.Width, (float)maxSize / (float)original.Height }).Min();

            Size resultSize = new Size((int)Math.Floor(original.Width * percent), (int)Math.Floor(original.Height * percent));
            return resultSize;
        }
    }
}