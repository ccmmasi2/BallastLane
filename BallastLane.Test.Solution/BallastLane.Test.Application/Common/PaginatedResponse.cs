namespace BallastLane.Test.Application.Common
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items        { get; set; } = Enumerable.Empty<T>();
        public int            TotalRecords { get; set; }
        public int            Page         { get; set; }
        public int            PageSize     { get; set; }

        public int  TotalPages       => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 0;
        public bool HasPreviousPage  => Page > 1;
        public bool HasNextPage      => Page < TotalPages;
    }
}
