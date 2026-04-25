using System.Net.Http;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Orders;
using System.Text;
using System.Text.Json;

namespace Hotcakes_Desktop
{
    public partial class Form1 : Form
    {
        private readonly string apiUrl = "https://localhost:7245/api/orders"; // Ellenőrizd a portot!
        private readonly HttpClient httpClient = new HttpClient();

        private string currentOrderBvin = "";

        public Form1()
        {
            InitializeComponent();

            //kiválasztás listboxban
            lbOrders.SelectedIndexChanged += LbOrders_SelectedIndexChanged;
        }

        //listboxba betöltés 
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<OrderDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (orders != null)
                {
                    lbOrders.DataSource = null;
                    // Előbb mondjuk meg neki a szabályokat:
                    lbOrders.DisplayMember = "Id";
                    lbOrders.ValueMember = "bvin";
                    // Utána öntjük bele az adatot:
                    lbOrders.DataSource = orders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hibaüzenet: " + ex.Message);
            }
        }

        // a kiválasztott listbox sor alapján betöltés dgv-ba
        private async void LbOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ha van kiválasztott elem
            if (lbOrders.SelectedValue != null && lbOrders.SelectedValue is string selectedBvin)
            {
                // EZ A SOR HIÁNYZOTT: Eltároljuk a bvin-t a "Kész" gomb számára!
                currentOrderBvin = selectedBvin;

                try
                {
                    var response = await httpClient.GetAsync($"{apiUrl}/{selectedBvin}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var fullOrder = JsonSerializer.Deserialize<OrderDTO>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Items lista betöltés
                        dgvItems.DataSource = null;
                        if (fullOrder != null && fullOrder.Items != null)
                        {
                            dgvItems.DataSource = fullOrder.Items;

                            // formázás külön -- melyik oszlopot ne mutassa
                            FormatItemGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hiba a részleteknél: " + ex.Message);
                }
            }
        }

        private void FormatItemGrid()
        {
            // oszlopok elrejtése
            if (dgvItems.Columns["OrderBvin"] != null) dgvItems.Columns["OrderBvin"].Visible = false;
            if (dgvItems.Columns["ProductId"] != null) dgvItems.Columns["ProductId"].Visible = false;
            if (dgvItems.Columns["BasePricePerItem"] != null) dgvItems.Columns["BasePricePerItem"].Visible = false;
            if (dgvItems.Columns["AdjustedPricePerItem"] != null) dgvItems.Columns["AdjustedPricePerItem"].Visible = false;
            if (dgvItems.Columns["LineTotal"] != null) dgvItems.Columns["LineTotal"].Visible = false;


            // magyarítás
            if (dgvItems.Columns["Id"] != null) dgvItems.Columns["Id"].HeaderText = "Termék azonosító";
            if (dgvItems.Columns["ProductSku"] != null) dgvItems.Columns["ProductSku"].HeaderText = "Vonalkód";
            if (dgvItems.Columns["ProductName"] != null) dgvItems.Columns["ProductName"].HeaderText = "Termék neve";
            if (dgvItems.Columns["Quantity"] != null) dgvItems.Columns["Quantity"].HeaderText = "Darab";

            // oszlop átméretezés
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            string scannedSku = txtSku.Text.Trim();

            // Ellenőrizzük, hogy számot írt-e a mennyiséghez
            if (!int.TryParse(txtQuantity.Text, out int scannedQty))
            {
                MessageBox.Show("A mennyiség csak szám lehet!");
                return;
            }

            // Végigmegyünk a táblázat mögötti adatokon
            var items = dgvItems.DataSource as List<LineItemDTO>;
            if (items == null) return;

            bool itemFound = false;

            foreach (var item in items)
            {
                // Ha egyezik az SKU
                if (item.ProductSku.Equals(scannedSku, StringComparison.OrdinalIgnoreCase))
                {
                    itemFound = true;

                    // Ha a mennyiség is stimmel
                    if (item.Quantity == scannedQty)
                    {
                        item.IsPacked = true; // Bepipáljuk a háttérben

                        // Frissítjük a táblázatot, hogy a pipa meg is jelenjen
                        dgvItems.Refresh();

                        // Készítjük elő a UI-t a következő termékre
                        txtSku.Clear();
                        txtQuantity.Text = "1";
                        txtSku.Focus();
                    }
                    else
                    {
                        MessageBox.Show($"Hibás mennyiség! Ebből a termékből {item.Quantity} db kell.");
                    }
                    break; // Megtaláltuk a terméket, kilépünk a ciklusból
                }
            }

            if (!itemFound)
            {
                MessageBox.Show("Ez a termék (SKU) nem szerepel a rendelésben!");
            }
        }

        private async void btnFinish_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentOrderBvin)) return;
            var items = dgvItems.DataSource as List<LineItemDTO>;
            if (items == null || items.Count == 0) return;

            var packedItems = items.Where(i => i.IsPacked).ToList();
            var missingItems = items.Where(i => !i.IsPacked).ToList();

            if (packedItems.Count == 0)
            {
                MessageBox.Show("Még semmit sem csomagoltál be!");
                return;
            }

            if (missingItems.Count == 0)
            {
                // ESET: Minden tétel kész -> Státusz váltás
                btnFinish.Enabled = false;
                await SetOrderToReadyForShippingAsync(currentOrderBvin);
                btnFinish.Enabled = true;
            }
            else
            {
                // ESET: Részleges teljesítés -> Bontás (amit előbb írtunk)
                var result = MessageBox.Show($"Csak részben vagy kész. Kettébontod a rendelést?", "Bontás", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    btnFinish.Enabled = false;
                    await SplitOrderAsync(currentOrderBvin, packedItems, missingItems);
                    btnFinish.Enabled = true;
                }
            }
        }

        private async Task SplitOrderAsync(string originalBvin, List<LineItemDTO> packed, List<LineItemDTO> missing)
        {
            try
            {
                // 1. LÉPÉS: Lekérjük a teljes, eredeti rendelést a C# API-nktól.
                // Erre azért van szükség, hogy pontosan le tudjuk másolni a vevő nevét, címét (BillingAddress, stb.)
                var getResponse = await httpClient.GetAsync($"{apiUrl}/{originalBvin}");
                getResponse.EnsureSuccessStatusCode();

                var jsonString = await getResponse.Content.ReadAsStringAsync();
                var originalOrder = JsonSerializer.Deserialize<OrderDTO>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (originalOrder == null) return;

                // 2. LÉPÉS: Létrehozunk egy teljesen ÚJ rendelést (POST) a HIÁNYZÓ tételekkel
                var newOrder = new OrderDTO
                {
                    UserEmail = originalOrder.UserEmail,
                    BillingAddress = originalOrder.BillingAddress,
                    ShippingAddress = originalOrder.ShippingAddress,
                    Items = missing, // Ide csak azokat tesszük, amiket NEM pipáltunk be
                    Instructions = $"Bontott rendelés. Eredeti rendelésszám: {originalOrder.OrderNumber}"
                };

                var postContent = new StringContent(JsonSerializer.Serialize(newOrder), Encoding.UTF8, "application/json");
                var postResponse = await httpClient.PostAsync(apiUrl, postContent);
                postResponse.EnsureSuccessStatusCode(); // Ha hiba van, itt megáll és átugrik a catch blokkba

                // 3. LÉPÉS: Módosítjuk az EREDETI rendelést (PUT), hogy csak a BECSOMAGOLT tételek maradjanak benne
                originalOrder.Items = packed;

                var putContent = new StringContent(JsonSerializer.Serialize(originalOrder), Encoding.UTF8, "application/json");
                var putResponse = await httpClient.PutAsync($"{apiUrl}/{originalBvin}", putContent);
                putResponse.EnsureSuccessStatusCode();

                MessageBox.Show("A rendelés kettébontása sikeresen befejeződött a szerveren!", "Siker", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Opcionális: Automatikusan "megnyomjuk" a betöltés gombot, hogy frissüljön a bal oldali lista az új rendeléssel
                btnLoad_Click(null, EventArgs.Empty);
                dgvItems.DataSource = null; // Kiürítjük a táblázatot
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az API-val való kommunikáció során:\n{ex.Message}", "Hálózati Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SetOrderToReadyForShippingAsync(string bvin)
        {
            try
            {
                // 1. Kapcsolódás a szerverhez a hivatalos DLL-lel (Közvetlenül a WinForms-ból!)
                string url = "http://74.234.38.79";
                string key = "1-c82617a1-53ab-41ce-bc85-d4d1efc12f9e";
                var proxy = new Api(url, key);

                // 2. Rendelés lekérése
                var orderResponse = proxy.OrdersFind(bvin);

                if (orderResponse.Errors.Count > 0 || orderResponse.Content == null)
                {
                    MessageBox.Show("Hiba a rendelés lekérésekor a DLL-lel!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var order = orderResponse.Content;

                // 3. Státusz átírása az objektumban
                order.StatusCode = "0c6d4b57-3e46-4c20-9361-6b0e5827db5a";
                order.StatusName = "Ready for Shipping";

                // 4. Mentés a szerverre
                var updateResponse = proxy.OrdersUpdate(order);

                if (updateResponse.Errors.Count > 0)
                {
                    MessageBox.Show("Hiba a mentésnél: " + updateResponse.Errors[0].Description, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("A rendelés állapota mostantól: Ready for Shipping", "Sikeres Mentés", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Frissítjük a felületet (Lista újratöltése, táblázat ürítése)
                    btnLoad_Click(null, EventArgs.Empty);
                    dgvItems.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kivétel történt a DLL futtatásakor: {ex.Message}\n\n(Ha ez a 'System.Web.Extensions' hiba, akkor a WinForms projektet is át kell állítanunk .NET Frameworkre!)", "Rendszerhiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
