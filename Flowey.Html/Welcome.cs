using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreHtmlToImage;
namespace Flowey.Html
{
    public class Welcome
    {
        private string _username;
        private string _avatarURL;
        public Welcome(string username, string avatarURL) 
        {
            _username = username;
            _avatarURL = avatarURL;
        }

        public async Task<byte[]> CreateImage()
        {
            string css = "\n<style>\n    .bg {\n      background-size: cover;\n       background-position: center;\n      background-image: url(\"https://cdn.discordapp.com/attachments/673739882589454377/673759882658447385/2.png\");\n        background-repeat: no-repeat }\n    </style>";

            string html = String.Format("" +
                "<head>\n" +
                "<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css\" integrity=\"sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm\" crossorigin=\"anonymous\">\n" +
                "</head>\n" +
                "<body>\n" +
                "<section class=\"section text-center\">\n" +
                "<div class=\"card welcome-card\" style=\"height: 100%; width: 100%;\">\n" +
                "<!--Card Image-->\n" +
                "<div class=\"bg card-up\">\n" +
                "<br>\n"+
                "<div class=\"avatar mx-auto white\">\n" +
                "<img src=\"{0}\" alt=\"avatar mx-auto white\" class=\"rounded-circle img-fluid\">\n" +
                "</div>\n" +
                "<br>\n" +
                "</div>\n" +
                "<!--Card Body-->\n" +
                "<div class=\"card-body card-body-cascade text-center\">\n" +
                "<h4 class=\"card-title\"><strong>Welcome {1} <br>to {2}</strong></h4>\n" +
                "<h5 class=\"card-text\">Please read the rules and enjoy your stay!</h5>\n" +
                "<br>\n" +
                "</div>\n" +
                "</div>\n" +
                "</section>\n" +
                "</body>\n" +
                "<script src=\"https://code.jquery.com/jquery-3.2.1.slim.min.js\" integrity=\"sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN\" crossorigin=\"anonymous\"></script>\n" +
                "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js\" integrity=\"sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q\" crossorigin=\"anonymous\"></script>\n" +
                "<script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js\" integrity=\"sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl\" crossorigin=\"anonymous\"></script>\n" +
                "</html>"
              ,_avatarURL, _username, "Flowers");
            var converter = new CoreHtmlToImage.HtmlConverter().FromHtmlString(html + css);
            var jpgBytes = converter;
            return jpgBytes;
        }
    }
}
