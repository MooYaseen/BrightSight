using AutoMapper;
using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Extensions;
using Graduation.Helpers;
using Graduation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Graduation.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Graduation.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

public MessagesController(
    IMapper mapper,
    IUserRepository userRepository,
    IMessageRepository messageRepository,
    IOptions<CloudinarySettings> config)
{
    _mapper = mapper;
    _userRepository = userRepository;
    _messageRepository = messageRepository;

    var acc = new Account(
        config.Value.CloudName,
        config.Value.ApiKey,
        config.Value.ApiSecret
    );

    _cloudinary = new Cloudinary(acc);
}



        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content,
            ImageUrl = createMessageDto.ImageUrl,
            AudioUrl = createMessageDto.AudioUrl, // 👈 تمت الإضافة هنا
            MessageType = createMessageDto.MessageType
            };


            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");

        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<string>> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // مجلد الحفظ في مشروع الـ Back-end
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // توليد اسم فريد للملف
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // إنشاء الـ URL الذي سيتم استخدامه في الفرونت-إند لعرض الصورة
            var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{uniqueFileName}";

            return Ok(imageUrl); // إرجاع الـ URL للصورة المرفوعة
        }

[HttpPost("upload-audio")]
public async Task<ActionResult<string>> UploadAudio(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("No audio file uploaded.");

    var uploadParams = new RawUploadParams
{
    File = new FileDescription(file.FileName, file.OpenReadStream())
};
// RawUploadParams يستخدم ResourceType.Raw تلقائياً فلا داعي لتحديدها


    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
        return StatusCode((int)uploadResult.StatusCode, "Failed to upload audio");

    return Ok(uploadResult.SecureUrl.ToString());
}






    [HttpGet("unread-count/{linkedUsername}")]
        public async Task<ActionResult<int>> GetUnreadCountFrom(string linkedUsername)
        {
        var currentUsername = User.GetUsername();

    var count = await _messageRepository.CountUnreadMessagesFrom(linkedUsername, currentUsername);

    return Ok(count);
    }



    }
}