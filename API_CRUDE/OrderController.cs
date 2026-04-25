using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Hotcakes.CommerceDTO.v1.Client; // KIZÁRÓLAG IDE KELL A DLL!

namespace API_CRUDE.videoAPI.Controllers
{
    // Modellek (Meghagyjuk a sajátokat a WinForms kompatibilitás és az IsPacked miatt!)
    public class HotcakesResponse<T> { public T Content { get; set; } public List<HotcakesError> Errors { get; set; } }
    public class HotcakesError { public string? Code { get; set; } public string? Description { get; set; } }
    public class CustomPropertyDTO { public string? DeveloperId { get; set; } public string? Key { get; set; } public string? Value { get; set; } }
    public class AddressDTO { public string? Bvin { get; set; } public string? FirstName { get; set; } public string? LastName { get; set; } public string? Line1 { get; set; } public string? City { get; set; } public string? PostalCode { get; set; } public string? CountryName { get; set; } }
    public class OrderDTO
    {
        public int Id { get; set; }
        public string? bvin { get; set; }
        public string? OrderNumber { get; set; }
        public string? UserEmail { get; set; }
        public List<CustomPropertyDTO> CustomProperties { get; set; }
        public List<LineItemDTO> Items { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusName { get; set; }
        public AddressDTO BillingAddress { get; set; }
        public AddressDTO ShippingAddress { get; set; }
        public string? Instructions { get; set; }
    }
    public class LineItemDTO
    {
        public string? bvin { get; set; }
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSku { get; set; }
        public int Quantity { get; set; }
    }
    public class StatusUpdateDTO { public string StatusCode { get; set; } public string StatusName { get; set; } }

    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiKey = "1-c82617a1-53ab-41ce-bc85-d4d1efc12f9e";
        private readonly string baseUrl = "http://74.234.38.79/DesktopModules/Hotcakes/API/rest/v1/";
        private readonly string rootUrl = "http://74.234.38.79";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await httpClient.GetAsync($"{baseUrl}orders?key={apiKey}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<HotcakesResponse<List<OrderDTO>>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result?.Content);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var response = await httpClient.GetAsync($"{baseUrl}orders/{id}?key={apiKey}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<HotcakesResponse<OrderDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result?.Content);
        }

        [HttpPost] // VISSZARAKVA A RENDELÉSBONTÁSHOZ
        public async Task<IActionResult> Post([FromBody] OrderDTO newOrder)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(newOrder), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}orders?key={apiKey}", jsonContent);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<HotcakesResponse<OrderDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result?.Content);
        }

        [HttpPut("{id}")] // VISSZARAKVA A RENDELÉSBONTÁSHOZ
        public async Task<IActionResult> Put(string id, [FromBody] OrderDTO updatedOrder)
        {
            var getResponse = await httpClient.GetAsync($"{baseUrl}orders/{id}?key={apiKey}");
            var rawJson = await getResponse.Content.ReadAsStringAsync();
            var jsonNode = System.Text.Json.Nodes.JsonNode.Parse(rawJson);
            var contentNode = jsonNode?["Content"];

            if (contentNode != null)
            {
                contentNode["Items"] = System.Text.Json.Nodes.JsonNode.Parse(JsonSerializer.Serialize(updatedOrder.Items));
                var putContent = new StringContent(contentNode.ToJsonString(), Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{baseUrl}orders?key={apiKey}&recalculateOrder=true", putContent);
                return Ok("Rendelés tételei frissítve!");
            }
            return BadRequest();
        }

        // A DLL-ES VARÁZSLAT KIZÁRÓLAG A STÁTUSZVÁLTÁSHOZ!
        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatusSafely(string id, [FromBody] StatusUpdateDTO statusUpdate)
        {
            try
            {
                var proxy = new Api(rootUrl, apiKey);
                var getResponse = proxy.OrdersFind(id);

                if (getResponse.Errors.Count > 0 || getResponse.Content == null) return BadRequest("Nem sikerült lekérni a rendelést a DLL-lel.");

                var order = getResponse.Content;
                order.StatusCode = statusUpdate.StatusCode;
                order.StatusName = statusUpdate.StatusName;

                var updateResponse = proxy.OrdersUpdate(order);

                if (updateResponse.Errors.Count > 0) return StatusCode(500, "Hotcakes DLL frissítési hiba.");

                return Ok("A rendelés státusza SIKERESEN frissült a hivatalos DLL-lel!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}