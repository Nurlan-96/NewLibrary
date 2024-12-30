namespace NewLibrary.Application.Response
{
    public class Pagination<T>
    {
        public Pagination(List<T> allItems, int page, int size)
        {
            //Failsafe
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;

            Data = allItems.Skip(size * (page - 1)).Take(size).ToList();
            TotalPages = (int)Math.Ceiling(allItems.Count / (double)size);
            CurrentPage = page;
            HasAfter = page < TotalPages;
            HasBefore = page > 1;
            TotalCount = allItems.Count;
        }

        public List<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasBefore { get; set; }
        public bool HasAfter { get; set; }
    }
}