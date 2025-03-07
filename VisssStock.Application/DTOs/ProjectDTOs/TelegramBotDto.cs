namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class TelegramBotRequestDto
    {
        public string TelegramBotName { get; set; } = null!;

        public string Token { get; set; } = null!;
    }

    public class TelegramBotResponseDto
    {
        public int Id { get; set; }

        public string TelegramBotName { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
