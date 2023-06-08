namespace Shared.RequestFeatures
{
    public abstract class RequestParameters
    {
        #region Pagination
        const int MAX_PAGE_SIZE = 20;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = MAX_PAGE_SIZE;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = (value > MAX_PAGE_SIZE) || value == 0 ? MAX_PAGE_SIZE : value;
            }
        }
        #endregion

        #region Sorting
        public string? OrderBy { get; set; }
        #endregion

        #region Sharding
        // We should consider that our API is really need to use sharding.
        // Because use sharding we need to use reflection => It will make our application slow
        public string? Fields { get; set; }
        #endregion
    }
}
