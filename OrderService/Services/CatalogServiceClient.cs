using Helpers;

namespace OrderService.Services
{
    public class CatalogServiceClient
    {
        private readonly HttpClient _httpClient;

        public CatalogServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductDto>> GetProductsAsync(List<int> productIds)
        {
            var idsQuery = string.Join("&", productIds.Select(id => $"productIds={id}"));
            var response = await _httpClient.GetAsync($"/api/catalog/products?{idsQuery}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            }

            return new List<ProductDto>();
        }

        public async Task<bool> ReduceStockAsync(List<(int productId, int quantity)> items)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/catalog/reduce-stock", items);
            return response.IsSuccessStatusCode;
        }
    }
}
