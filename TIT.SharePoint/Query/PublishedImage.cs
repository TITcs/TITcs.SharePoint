using System.Xml.Linq;

namespace TITcs.SharePoint.Query
{
    public class PublishedImage
    {
        public string Src { get; set; }
        public string Alt { get; set; }
        public string Style { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public static implicit operator PublishedImage(string tagImage)
        {
            try
            {
                var publishedImage = new PublishedImage();

                var xElement = XElement.Parse(tagImage.Replace(">", "/>"));

                publishedImage.Src = xElement.Attribute("src").Value;
                publishedImage.Alt = xElement.Attribute("alt").Value;
                publishedImage.Style = xElement.Attribute("style").Value;

                var widthElement = xElement.Attribute("width");
                var heightElement = xElement.Attribute("height");

                publishedImage.Width = widthElement != null ? widthElement.Value : null;
                publishedImage.Height = heightElement != null ? heightElement.Value : null;

                return publishedImage;
            }
            catch
            {

                return new PublishedImage
                {
                    Src = "",
                    Alt = "",
                    Height = "",
                    Style = "",
                   Width = ""
                };
            }
        }

    }
}
