using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class UserTelegram
{
    public int Id { get; set; }

    public string ChatId { get; set; } = null!;

    public int UserId { get; set; }

    public int TelegramBotId { get; set; }
}
