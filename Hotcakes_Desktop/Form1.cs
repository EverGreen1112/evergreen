using System.Net.Http;
using System.Text.Json;

namespace Hotcakes_Desktop
{
    public partial class Form1 : Form
    {
        private readonly string apiUrl = "https://localhost:7245/api/orders"; // Ellenőrizd a portot!
        private readonly HttpClient httpClient = new HttpClient();

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
                    lbOrders.DataSource = orders;

                    // Id szerint lesz a kiválasztás, de háttérben bvin alapján szűr
                    lbOrders.DisplayMember = "Id"; 
                    lbOrders.ValueMember = "bvin";          
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
                try
                {
                    // api/orders/{id} végpontot hívja meg
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
            if (dgvItems.Columns["Id"] != null) dgvItems.Columns["Id"].HeaderText = "Rendelés azonosító";
            if (dgvItems.Columns["ProduktSKU"] != null) dgvItems.Columns["ProduktSKU"].HeaderText = "Vonalkód";
            if (dgvItems.Columns["ProductName"] != null) dgvItems.Columns["ProductName"].HeaderText = "Termék neve";
            if (dgvItems.Columns["Quantity"] != null) dgvItems.Columns["Quantity"].HeaderText = "Darab";

            // oszlop átméretezés
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
