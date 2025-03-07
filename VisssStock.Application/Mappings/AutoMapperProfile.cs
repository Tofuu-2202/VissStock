using AutoMapper;
using Newtonsoft.Json;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;
using Type = VisssStock.Domain.DataObjects.Type;

namespace VisssStock.WebAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();
            CreateMap<RoleDTO, Role>();
            CreateMap<Role, RoleDTOResponse>();
            CreateMap<Role, RoleDTO>();
            CreateMap<Role, RoleDTOUser>();

            CreateMap(typeof(PagedList<>), typeof(PagedListResponseDTO<>))
            .ConvertUsing(typeof(PagedListTypeConverter<>));

            //// Generic mapping for PagedListResponseDTO to PagedListResponseDTO
            //CreateMap(typeof(PagedListResponseDTO<>), typeof(PagedListResponseDTO<>))
            //    .ConvertUsing(typeof(PagedListResponseDTOTypeConverter<,>));

            // map transaction
            CreateMap<Transaction, TransactionResponseDto>();
            //.ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
            //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))

            CreateMap<Transaction, TransactionRequestDto>().ReverseMap();


            // Exchange mapper (Convert nguoc de convert response khi create)
            CreateMap<Exchange, ExchangeRequestDTO>();
            CreateMap<Exchange, ExchangeResponseDTO>();
            CreateMap<ExchangeRequestDTO, Exchange>();

            CreateMap<Type, TypeRequestDTO>();
            CreateMap<Type, TypeResponseDTO>();
            CreateMap<TypeRequestDTO, Type>();

            CreateMap<Stock, StockRequestDTO>();
            CreateMap<Stock, StockResponseDTO>()
                .ForMember(s => s.TypeResponse, opt => opt.MapFrom(src => src.Type))
                .ForMember(s => s.ExchangeResponse, opt => opt.MapFrom(src => src.Exchange))
                .ForMember(s => s.ScreenerResponse, opt => opt.MapFrom(src => src.Screener));
            CreateMap<StockRequestDTO, Stock>();

            // StockGroupStock mapping
            CreateMap<StockGroupStock, StockGroupStockRequestDTO>();
            CreateMap<StockGroupStock, StockGroupStockResponseDTO>()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.StockGroup, opt => opt.MapFrom(src => src.StockGroup));
            CreateMap<StockGroupStockRequestDTO, StockGroupStock>();

            CreateMap<StockGroup, StockGroupRequestDTO>();
            CreateMap<StockGroup, StockGroupResponseDTO>()
                .ForMember(dest => dest.Interval, opt => opt.MapFrom(src => src.Interval))
                .ForMember(dest => dest.ConditionGroup, opt => opt.MapFrom(src => src.ConditionGroup))
                .ForMember(dest => dest.TelegramBot, opt => opt.MapFrom(src => src.TelegramBot));

            CreateMap<StockGroupRequestDTO, StockGroup>();
            CreateMap<StockGroup, StockGroupResponse1DTO>().ReverseMap();

            CreateMap<Screener, ScreenerRequestDTO>();
            CreateMap<Screener, ScreenerResponseDTO>();
            CreateMap<ScreenerRequestDTO, Screener>();

            // StockGroupStock mapping
            CreateMap<StockGroupStock, StockGroupStockRequestDTO>();
            CreateMap<StockGroupStock, StockGroupStockResponseDTO>()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.StockGroup, opt => opt.MapFrom(src => src.StockGroup));
            CreateMap<StockGroupStockRequestDTO, StockGroupStock>();

            // StockGroupIndicator mapping
            CreateMap<StockGroupIndicator, StockGroupIndicatorRequestDTO>();
            CreateMap<StockGroupIndicator, StockGroupIndicatorResponseDTO2>();
            CreateMap<StockGroupIndicatorRequestDTO, StockGroupIndicator>();

            //indicator mapping
            CreateMap<IndicatorResponseDTO, Indicator>().ReverseMap();
            CreateMap<CreateIndicatorDTO, Indicator>().ReverseMap();
            CreateMap<UpdateIndicatorDTO, Indicator>().ReverseMap();

            //telegrambot mapping
            CreateMap<TelegramBotRequestDto, TelegramBot>().ReverseMap();
            CreateMap<TelegramBotResponseDto, TelegramBot>().ReverseMap();
            CreateMap<TelegramBotResponsexDTO, TelegramBot>().ReverseMap();

            //interval mapping
            CreateMap<IntervalResponseDTO, Interval>().ReverseMap();
            CreateMap<UpdateIntervalDTO, Interval>().ReverseMap();
            CreateMap<CreateIntervalDTO, Interval>().ReverseMap();

            //stockgroupindicator mapping
            CreateMap<CreateStockGroupIndicatorDTO, StockGroupIndicator>().ReverseMap();
            CreateMap<StockGroupIndicator, StockGroupIndicatorResponseDTO>()
                .ForMember(dest => dest.StockGroup, opt => opt.MapFrom(src => src.StockGroup))
                .ForMember(dest => dest.Indicator, opt => opt.MapFrom(src => src.Indicator));
            CreateMap<StockGroupIndicator, UpdateStockGroupIndicatorDTO>().ReverseMap();

            //ConditionGroupResponseDto
            CreateMap<ConditionGroupRequestDto, ConditionGroup>().ReverseMap();
            CreateMap<ConditionGroup, ConditionGroupResponseDto>()
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.StockGroups));

            //ConditionGroupResponsexDto
            CreateMap<ConditionGroup, ConditionGroupResponsexDto>();

            //IndicatorDraftRequestDto
            CreateMap<IndicatorDraftRequestDto, IndicatorDraft>().ReverseMap();
            CreateMap<IndicatorDraft, IndicatorDraftResponseDto>()
                .ForMember(dest => dest.IndicatorId1Navigation, opt => opt.MapFrom(src => src.IndicatorId1Navigation))
                .ForMember(dest => dest.IndicatorId2Navigation, opt => opt.MapFrom(src => src.IndicatorId2Navigation))
                .ForMember(dest => dest.StockGroup, opt => opt.MapFrom(src => src.StockGroup));


            //tradingview
            CreateMap<StockGroup, StockGroupResponsexDTO>()
                .ForMember(dest => dest.Interval, opt => opt.MapFrom(src => src.Interval))
               .ForMember(dest => dest.StockGroupIndicators, opt => opt.MapFrom(src => src.StockGroupIndicators))
               .ForMember(dest => dest.Stocks, opt => opt.MapFrom(src => src.StockGroupStocks.Select(sgs => sgs.Stock)))
               .ForMember(dest => dest.ConditionGroup, opt => opt.MapFrom(src => src.ConditionGroup));
            //CreateMap<StockGroup, StockGroupResponsexDTO>()
            //    .ForMember(dest => dest.StockGroupIndicators, opt => opt.MapFrom(src => src.StockGroupIndicators))
            //    .ForMember(dest => dest.Stocks, opt => opt.MapFrom(src => src.StockGroupStocks.Select(sgs => sgs.Stock)));
            CreateMap<StockGroupIndicator, StockGroupIndicatorxResponseDTO>();

            //AlertLogRequestDto
            CreateMap<AlertLogRequestDto, AlertLog>().ReverseMap();
            CreateMap<AlertLog, AlertLogResponseDto>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<AlertLogDataConverted>(src.DataJson)));

        }
    }

    public class PagedListTypeConverter<TSource> : ITypeConverter<PagedList<TSource>, PagedListResponseDTO<TSource>> where TSource : class
    {
        public PagedListResponseDTO<TSource> Convert(PagedList<TSource> source, PagedListResponseDTO<TSource> destination, ResolutionContext context)
        {
            return new PagedListResponseDTO<TSource>(source);
        }
    }
}