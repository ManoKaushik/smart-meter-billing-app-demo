
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;


namespace SmartMeterWeb.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public ConsumerService(AppDbContext context, IWebHostEnvironment env, IConfiguration config)
        {
            _context = context;
            _env = env;
            _config = config;
        }

        public async Task<ConsumerDto> GetConsumerDetailsAsync(string Name)
        {
            var consumer = await _context.Consumers.FirstOrDefaultAsync(c => c.Name == Name);
            if (consumer == null)
                throw new Exception("Invalid name");

            var photoUrl = string.IsNullOrEmpty(consumer.PhotoPath)
            ? null
            : $"{_config["AppBaseUrl"]}/{consumer.PhotoPath.Replace("\\", "/")}";

            return new ConsumerDto
            {
                ConsumerId = consumer.ConsumerId,
                Name = consumer.Name,
                Phone = consumer.Phone,
                Email = consumer.Email,
                Status = consumer.Status,
                PhotoUrl = photoUrl
            };
        }



        public async Task<IActionResult> UploadConsumerPhotoAsync(string ConsumerName, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new BadRequestObjectResult("No file uploaded");

            var consumer = await _context.Consumers.FirstOrDefaultAsync(c => c.Name == ConsumerName);
            if (consumer == null)
                return new NotFoundObjectResult("Invalid consumer name");

            // Create the uploads folder if it doesn't exist
            var uploadsFolder = Path.Combine(_env.WebRootPath, "consumer_photos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName);
            var safeName = ConsumerName.Replace(" ", "_");
            var uniqueFileName = $"{safeName}{extension}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save relative path to database
            consumer.PhotoPath = Path.Combine("consumer_photos", uniqueFileName);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Message = "Photo uploaded successfully",
                FilePath = consumer.PhotoPath
            });
        }

    }
}
