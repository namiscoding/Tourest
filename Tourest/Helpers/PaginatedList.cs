using Microsoft.EntityFrameworkCore;

namespace Tourest.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }


        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            // Đảm bảo pageIndex hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            // Tính toán lại pageIndex nếu nó vượt quá số trang thực tế
            int calculatedTotalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageIndex > calculatedTotalPages && calculatedTotalPages > 0)
            {
                pageIndex = calculatedTotalPages;
            }

            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        // Overload for IEnumerable if needed after filtering in memory (less efficient)
        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex < 1) pageIndex = 1;
            int calculatedTotalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageIndex > calculatedTotalPages && calculatedTotalPages > 0)
            {
                pageIndex = calculatedTotalPages;
            }
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
