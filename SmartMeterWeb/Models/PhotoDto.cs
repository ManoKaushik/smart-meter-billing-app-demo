using Microsoft.AspNetCore.Mvc;

namespace SmartMeterWeb.Models
{
    public class PhotoDto
    {
        [FromForm(Name = "ConsumerName")]
        public string ConsumerName { get; set; }

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
