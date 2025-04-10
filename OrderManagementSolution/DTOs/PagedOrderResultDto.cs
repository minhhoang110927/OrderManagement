namespace OrderManagementSolution.DTOs
{
    public class PagedOrderResultDto
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<OrderDto> Data { get; set; }
    }
}
