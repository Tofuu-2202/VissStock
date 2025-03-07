using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface ITelegramBotService
    {
        // get all TelegramBot
        Task<ServiceResponse<PagedListResponseDTO<TelegramBotResponseDto>>> getAllTelegramBots(OwnerParameters ownerParameters, string searchByName);

        // get TelegramBot by id
        Task<ServiceResponse<TelegramBotResponseDto>> getTelegramBotById(int id);

        // update TelegramBot
        Task<ServiceResponse<TelegramBotResponseDto>> updateTelegramBot(TelegramBotRequestDto updateTelegramBotDTO, int id);

        // delete TelegramBot

        Task<ServiceResponse<TelegramBotResponseDto >> deleteTelegramBot(int id);

        // create TelegramBot
        Task<ServiceResponse<TelegramBotResponseDto>> createTelegramBot(TelegramBotRequestDto requestDto);

        Task<ServiceResponse<TelegramBotResponseDto>> Tool();
    }
}
