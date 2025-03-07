using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface IConditionGroupService
    {
        Task<ServiceResponse<ConditionGroupPagedListResponseDto>> getAllConditionGroups(OwnerParameters ownerParameters, ConditionGroupFillterDto requestDto);

        Task<ServiceResponse<ConditionGroupResponseDto>> createConditionGroup(ConditionGroupRequestDto conditionGroupRequestDto);

        Task<ServiceResponse<ConditionGroupResponseDto>> updateConditionGroup(int id, ConditionGroupRequestDto conditionGroupRequestDto);

        Task<ServiceResponse<ConditionGroupResponseDto>> deleteConditionGroup(int id);
    }
}
