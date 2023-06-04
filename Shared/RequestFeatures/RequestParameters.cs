namespace Shared.RequestFeatures
{
    public abstract class RequestParameters
    {
        const int MAX_PAGE_SIZE = 20;

        public int PageNumber { get; set; } = 1;

        private int _pageSize;

        public int PageSize { 
            get 
            { 
                return _pageSize; 
            } 
            
            set
            {
                _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
            }
        }
    }
}
