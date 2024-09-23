#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;

namespace Core.Dtos
{
    [Serializable]
    public class PaginationDto
    {
#pragma warning disable RAD201
        public int limit;
        public int offset;
#pragma warning restore RAD201
    }
}