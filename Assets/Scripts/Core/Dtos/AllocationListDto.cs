#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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