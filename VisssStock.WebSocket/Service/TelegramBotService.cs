using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Newtonsoft.Json;

namespace VissStockApp.Service
{
    internal class TelegramBotService
    {
        public static async Task StartBot(TelegramBotClient botClient, string botToken, CancellationTokenSource cancellationTokenSource, Dictionary<long, string> userPendingUsername, Dictionary<long, bool> userAwaitingConfirmation)
        {
            var lastUpdateId = await GetLastUpdateIdAsync(botClient);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // Chọn loại update mà bạn muốn nhận
            };

            botClient.StartReceiving(
                updateHandler: async (client, update, token) =>
                {
                    if (update.Id > lastUpdateId)
                    {
                        await HandleUpdateAsync(client, botToken, update, token, userPendingUsername, userAwaitingConfirmation);
                    }
                },
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationTokenSource.Token
            );

            Console.WriteLine("Bot is running!");
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine($"Polling error: {exception.Message}");
            return Task.CompletedTask;
        }

        public static async Task<int> GetLastUpdateIdAsync(TelegramBotClient botClient)
        {
            try
            {
                var updates = await botClient.GetUpdatesAsync(limit: 1);
                return updates != null && updates.Length > 0 ? updates.Max(u => u.Id) : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching updates: {ex.Message}");
                return 0; // Nếu có lỗi thì trả về 0 để xử lý tất cả tin nhắn mới
            }
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, string token, Update update, System.Threading.CancellationToken cancellationToken, Dictionary<long, string> userPendingUsername, Dictionary<long, bool> userAwaitingConfirmation)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message != null && update.Message.Type == MessageType.Text)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var messageText = message.Text;

                    if (string.IsNullOrWhiteSpace(messageText))
                    {
                        // Xử lý trường hợp text là null hoặc chỉ chứa khoảng trắng
                        await botClient.SendTextMessageAsync(chatId, "The message is empty or contains only whitespace. Please enter a valid @username.");
                        return; // Kết thúc xử lý để tránh các lỗi tiếp theo
                    }

                    if (messageText == "/start")
                    {
                        Console.WriteLine($"Processing /start command...");

                        // Đảm bảo người dùng không có khóa chatId trong dictionaries trước khi gán giá trị
                        if (!userPendingUsername.ContainsKey(chatId))
                        {
                            userPendingUsername[chatId] = null; // Đánh dấu người dùng đang chờ username
                        }
                        else
                        {
                            userPendingUsername[chatId] = null; // Cập nhật giá trị của khóa chatId
                        }

                        if (!userAwaitingConfirmation.ContainsKey(chatId))
                        {
                            userAwaitingConfirmation[chatId] = false; // Đánh dấu người dùng chưa xác nhận
                        }
                        else
                        {
                            userAwaitingConfirmation[chatId] = false; // Cập nhật giá trị của khóa chatId
                        }

                        await botClient.SendTextMessageAsync(chatId, "Please enter a @username.");
                    }
                    else if (userPendingUsername.ContainsKey(chatId) && userPendingUsername[chatId] == null)
                    {
                        // Xử lý tin nhắn khi người dùng nhập @username
                        userPendingUsername[chatId] = messageText;

                        var keyboard = new ReplyKeyboardMarkup(new[]
                        {
                        new KeyboardButton("Yes"),
                        new KeyboardButton("No")
                    })
                        {
                            ResizeKeyboard = true
                        };

                        await botClient.SendTextMessageAsync(
                            chatId,
                            $"You entered: {messageText}. Is this correct?",
                            replyMarkup: keyboard
                        );

                        userAwaitingConfirmation[chatId] = true; // Đánh dấu người dùng đang chờ xác nhận
                    }
                    else if (userAwaitingConfirmation.ContainsKey(chatId) && userAwaitingConfirmation[chatId])
                    {
                        var telegramUsername = userPendingUsername[chatId];
                        await HandleConfirmationUserNameResponseAsync(botClient, token, chatId, telegramUsername, messageText, userPendingUsername, userAwaitingConfirmation);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling update: {ex.Message}");
            }
        }

        public static async Task HandleConfirmationUserNameResponseAsync(ITelegramBotClient botClient, string token, long chatId, string telegramUserName, string messageText, Dictionary<long, string> userPendingUsername, Dictionary<long, bool> userAwaitingConfirmation)
        {
            if (messageText.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                // Xử lý khi người dùng xác nhận đúng
                var xxxx = userPendingUsername[chatId];

                // Gọi API để cập nhật người dùng
                string apiResponse = await RequestApi.CallUpdateUserAPI(token, userPendingUsername[chatId],chatId.ToString(), telegramUserName);
                Console.WriteLine("API Response: " + apiResponse);
                await botClient.SendTextMessageAsync(chatId, $"Username '{userPendingUsername[chatId]}' confirmed.");

                // Kiểm tra kết quả trả về từ API
                // Deserialize the API response to get the Message property
                var response = JsonConvert.DeserializeObject<Program.ServiceResponse<object>>(apiResponse);
                string apiMessage = response?.Message ?? "Unknown error occurred.";

                // Kiểm tra kết quả trả về từ API
                if (response != null && response.Status)
                {
                    Console.WriteLine("API Response: " + apiResponse);

                    // Xóa bàn phím
                    var keyboard = new ReplyKeyboardRemove();
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Thank you! The operation is now closed.",
                        replyMarkup: keyboard
                    );

                    // Cập nhật trạng thái
                    userPendingUsername[chatId] = null;
                    userAwaitingConfirmation[chatId] = false;
                }
                else
                {
                    // Xử lý khi API call thất bại
                    await botClient.SendTextMessageAsync(chatId, $"Failed to update user: {apiMessage}. Please try again later or check the username.");
                }
            }
            else if (messageText.Equals("No", StringComparison.OrdinalIgnoreCase))
            {
                // Xử lý khi người dùng từ chối và yêu cầu nhập lại
                await botClient.SendTextMessageAsync(chatId, "Please enter a new @username.");

                // Xóa bàn phím
                var keyboard = new ReplyKeyboardRemove();
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Operation has been reset. Please enter a new @username.",
                    replyMarkup: keyboard
                );

                // Cập nhật trạng thái
                userPendingUsername[chatId] = null; // Đánh dấu người dùng cần nhập lại username
                userAwaitingConfirmation[chatId] = false;
            }
        }

        public static async Task SendMessageToChatAsync(Dictionary<string, TelegramBotClient> TelegramBot, string chatId, string messageText)
        {
            try
            {


                var token = chatId.Split(";")[1];
                var chatIdx = chatId.Split(";")[2];

                var botClient = TelegramBot[token];
                // Gửi tin nhắn đến chat ID
                await botClient.SendTextMessageAsync(
                    chatId: chatIdx,
                    text: messageText,
                    parseMode: ParseMode.Markdown
                );

                //sent to admin chat
                await botClient.SendTextMessageAsync(
                    chatId: 6542597563,
                    text: messageText,
                    parseMode: ParseMode.Markdown
                );

                Console.WriteLine($"Message sent to chat ID {chatId}: {messageText}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
    }
}
