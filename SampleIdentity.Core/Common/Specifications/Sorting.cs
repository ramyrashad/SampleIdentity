using System.ComponentModel.DataAnnotations;

namespace SampleIdentity.Core.Common.Specifications
{
    public class Sorting
    {
        public Sorting()
        {

        }

        public Sorting(int pageIndex, int pageSize, SortDirectionType sortDirection)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortDirection = sortDirection;
        }

        public int PageIndex { get; set; }

        [Range(1,50)]
        public int PageSize { get; set; }

        public SortDirectionType SortDirection { get; set; }

        public enum SortDirectionType
        {
            Ascending,
            Descending,
        }

        public bool IsDescending()
        {
            return SortDirection == SortDirectionType.Descending;
        }
    }
}
