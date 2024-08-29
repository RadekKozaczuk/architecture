using System;

namespace Core.Dtos
{
    [Serializable]
    public class AllocationListDto
    {
#pragma warning disable RAD201
        public AllocationDto[]? allocations;
        public PaginationDto pagination;
#pragma warning restore RAD201
    }
}