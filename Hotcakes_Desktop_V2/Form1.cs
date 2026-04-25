using Hotcakes.Commerce.Orders;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json; // Hozzáadva a klónozáshoz
using Newtonsoft.Json.Linq; // Hozzáadva a biztonságos árolvasáshoz

namespace Hotcakes_Desktop_V2
{
    public partial class Form1 : Form
    {
        private readonly string rootUrl = "http://74.234.38.79";
        private readonly string apiKey = "1-c82617a1-53ab-41ce-bc85-d4d1efc12f9e";
        private string currentOrderBvin = "";

        public Form1()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            InitializeComponent();
            lbOrders.SelectedIndexChanged += LbOrders_SelectedIndexChanged;
        }

        // Betöltő gomb és szűrések
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var proxy = new Api(rootUrl, apiKey);
                var response = proxy.OrdersFindAll();

                if (response.Errors.Count > 0)
                {
                    MessageBox.Show("Hiba a lekéréskor: " + response.Errors[0].Description);
                    return;
                }

                // Csak a "Received" státuszúak maradjanak
                // Listboxban a kód és név is jelenjen meg
                var filteredOrders = response.Content
                    .Where(o => o.StatusName == "Received")
                    .Select(o => new LocalOrderSnapshot
                    {
                        Bvin = o.bvin,
                        OrderNumber = o.OrderNumber,
                        FirstName = o.BillingAddress.FirstName,
                        LastName = o.BillingAddress.LastName
                    }).ToList();

                lbOrders.DataSource = null;
                lbOrders.DisplayMember = "DisplayText"; // Ez az új kombinált szöveg ami megjelenik a Listboxban
                lbOrders.ValueMember = "Bvin";
                lbOrders.DataSource = filteredOrders;

                if (filteredOrders.Count == 0)
                {
                    MessageBox.Show("Nincs feldolgozandó (Received) rendelés.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kritikus DLL hiba: " + ex.Message);
            }
        }

        // DGV betöltése kijelölt listbox sor alapján
        private void LbOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbOrders.SelectedValue != null && lbOrders.SelectedValue is string selectedBvin)
            {
                currentOrderBvin = selectedBvin;
                try
                {
                    var proxy = new Api(rootUrl, apiKey);
                    var response = proxy.OrdersFind(selectedBvin);

                    if (response.Content != null)
                    {
                        var uiItems = response.Content.Items.Select(i => new LocalLineItemDTO
                        {
                            ProductSku = i.ProductSku,
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            IsPacked = false
                        }).ToList();

                        dgvItems.DataSource = null;
                        dgvItems.DataSource = uiItems;
                        FormatItemGrid();
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        // Ha stimmel az sku és a mennyiség, akkor pipa
        private void btnPack_Click(object sender, EventArgs e)
        {
            string scannedSku = txtSku.Text.Trim();
            if (!int.TryParse(txtQuantity.Text, out int scannedQty)) return;

            var items = dgvItems.DataSource as List<LocalLineItemDTO>;
            if (items == null) return;

            foreach (var item in items)
            {
                if (item.ProductSku.Equals(scannedSku, StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Quantity == scannedQty)
                    {
                        item.IsPacked = true;
                        dgvItems.Refresh();
                        txtSku.Clear();
                        txtQuantity.Text = "1";
                        txtSku.Focus();
                    }
                    else MessageBox.Show($"Rossz darabszám! {item.Quantity} db kell.");
                    return;
                }
            }
            MessageBox.Show("Nincs ilyen SKU ebben a rendelésben!");
        }

        // Ha nincs semmi feltöltve, figyelmeztessen, ha minden, akkor csak státusz váltás
        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentOrderBvin)) return;

            var items = dgvItems.DataSource as List<LocalLineItemDTO>;
            if (items == null) return;

            var packed = items.Where(i => i.IsPacked).ToList();
            var missing = items.Where(i => !i.IsPacked).ToList();

            if (packed.Count == 0)
            {
                MessageBox.Show("Még semmit sem csomagoltál be!");
                return;
            }

            if (missing.Count == 0)
            {
                UpdateStatusToReady(currentOrderBvin);
            }
            else
            {
                // Ha egyik sem: kettébontás útvonala -- figyelmeztetés 
                if (MessageBox.Show("Kettébontod a rendelést? A kijelölt tételek 'Kész' státuszba kerülnek, a maradékból új rendelés jön létre.", "Rendelés bontása", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SplitOrderWithDll(currentOrderBvin, packed, missing);
                }
            }
        }

        // Kész verzió 
        private void UpdateStatusToReady(string bvin)
        {
            var proxy = new Api(rootUrl, apiKey);
            var res = proxy.OrdersFind(bvin);
            if (res.Content == null) return;

            var order = res.Content;
            order.StatusCode = "0c6d4b57-3e46-4c20-9361-6b0e5827db5a";
            order.StatusName = "Ready for Shipping";

            var updateRes = proxy.OrdersUpdate(order);
            if (updateRes.Errors.Count == 0)
            {
                MessageBox.Show("Sikeres státuszváltás! A rendelés eltűnik a listából.");
                // Az automatikus frissítés miatt el fog tűnni, mert már nem "Received"
                btnLoad_Click(null, EventArgs.Empty);
                dgvItems.DataSource = null;
            }
        }

        // Félkész verzió, kettébontás + pénzügyek
        private void SplitOrderWithDll(string bvin, List<LocalLineItemDTO> packed, List<LocalLineItemDTO> missing)
        {
            try
            {
                var proxy = new Api(rootUrl, apiKey);

                var originalRes = proxy.OrdersFind(bvin);
                if (originalRes.Content == null) return;
                var originalOrder = originalRes.Content;

                // Pénzügyek számolása bontáshoz, beolvasással a json-ből
                decimal missingItemsTotal = 0;
                decimal packedItemsTotal = 0;

                foreach (var m in missing)
                {
                    var origItem = originalOrder.Items.FirstOrDefault(i => i.ProductSku == m.ProductSku);
                    if (origItem != null)
                    {
                        var jItem = JObject.FromObject(origItem);
                        decimal lineTotal = jItem["LineTotal"] != null ? jItem["LineTotal"].Value<decimal>() : 0;
                        if (lineTotal == 0 && jItem["AdjustedPrice"] != null) lineTotal = jItem["AdjustedPrice"].Value<decimal>() * origItem.Quantity;
                        decimal unitPrice = origItem.Quantity > 0 ? (lineTotal / origItem.Quantity) : 0;
                        missingItemsTotal += (unitPrice * m.Quantity);
                    }
                }

                foreach (var p in packed)
                {
                    var origItem = originalOrder.Items.FirstOrDefault(i => i.ProductSku == p.ProductSku);
                    if (origItem != null)
                    {
                        var jItem = JObject.FromObject(origItem);
                        decimal lineTotal = jItem["LineTotal"] != null ? jItem["LineTotal"].Value<decimal>() : 0;
                        if (lineTotal == 0 && jItem["AdjustedPrice"] != null) lineTotal = jItem["AdjustedPrice"].Value<decimal>() * origItem.Quantity;
                        decimal unitPrice = origItem.Quantity > 0 ? (lineTotal / origItem.Quantity) : 0;
                        packedItemsTotal += (unitPrice * p.Quantity);
                    }
                }

                // Arányok kiszámítása az adó elosztásához
                decimal originalItemsTotal = originalOrder.TotalOrderBeforeDiscounts > 0 ? originalOrder.TotalOrderBeforeDiscounts : (missingItemsTotal + packedItemsTotal);
                if (originalItemsTotal == 0) originalItemsTotal = 1; // Nullával osztás elkerülése

                decimal missingRatio = missingItemsTotal / originalItemsTotal;
                decimal packedRatio = packedItemsTotal / originalItemsTotal;

                // Maradékokból új rendelés létrehozás
                string suffixOrderNumber = originalOrder.OrderNumber + "-B"; // Új "Bontott" azonosító, hogy ne akadjon össze a Hotckaes

                var newOrder = new OrderDTO
                {
                    StoreId = originalOrder.StoreId,
                    OrderNumber = suffixOrderNumber,
                    UserEmail = originalOrder.UserEmail,
                    BillingAddress = originalOrder.BillingAddress,
                    ShippingAddress = originalOrder.ShippingAddress,
                    ShippingMethodId = originalOrder.ShippingMethodId,

                    IsPlaced = true,
                    StatusCode = "F37EC405-1EC6-4a91-9AC4-6836215FBBBC", // Received kódja
                    StatusName = "Received",
                    PaymentStatus = originalOrder.PaymentStatus,
                    Instructions = "Bontott hátralék. Eredeti: " + originalOrder.OrderNumber, // ez hotcakesben ott lesz a részletekben

                    // Pénzügyek az új (maradék) csomagon (ez már szállítási díj nélkül)
                    TotalOrderBeforeDiscounts = missingItemsTotal,
                    ItemsTax = Math.Round(originalOrder.ItemsTax * missingRatio, 2),
                    TotalTax = Math.Round(originalOrder.TotalTax * missingRatio, 2),
                    TotalShippingBeforeDiscounts = 0, // A futárt a másik csomagnál fizeti
                    TotalGrand = missingItemsTotal + Math.Round(originalOrder.TotalTax * missingRatio, 2)
                };

                // Klónozás -- árak miatt egészet átviszi
                foreach (var m in missing)
                {
                    var origItem = originalOrder.Items.FirstOrDefault(i => i.ProductSku == m.ProductSku);
                    if (origItem != null)
                    {
                        string json = JsonConvert.SerializeObject(origItem);
                        var clonedItem = JsonConvert.DeserializeObject<LineItemDTO>(json);
                        clonedItem.Quantity = m.Quantity;
                        newOrder.Items.Add(clonedItem);
                    }
                }

                var createRes = proxy.OrdersCreate(newOrder);
                if (createRes.Errors.Count > 0)
                {
                    MessageBox.Show("Hiba az új rendelésnél: " + createRes.Errors[0].Description);
                    return;
                }

                // Kész részek frissítése eredetiben
                var packedItemsToKeep = new List<LineItemDTO>();
                foreach (var p in packed)
                {
                    var origItem = originalOrder.Items.FirstOrDefault(i => i.ProductSku == p.ProductSku);
                    if (origItem != null)
                    {
                        origItem.Quantity = p.Quantity; // Megtartjuk az eredetit, csak mennyiséget módosítunk
                        packedItemsToKeep.Add(origItem);
                    }
                }

                originalOrder.Items = packedItemsToKeep;
                originalOrder.StatusCode = "0c6d4b57-3e46-4c20-9361-6b0e5827db5a"; // Ready for shipping kódja
                originalOrder.StatusName = "Ready for Shipping";

                // Pénzügyek beállítása az eredeti rendelésen (megtartja a szállítási díjat)
                originalOrder.TotalOrderBeforeDiscounts = packedItemsTotal;
                originalOrder.ItemsTax = Math.Round(originalOrder.ItemsTax * packedRatio, 2);
                originalOrder.TotalTax = Math.Round(originalOrder.TotalTax * packedRatio, 2);
                originalOrder.TotalGrand = packedItemsTotal + originalOrder.TotalTax + originalOrder.TotalShippingBeforeDiscounts;

                var updateRes = proxy.OrdersUpdate(originalOrder);

                if (updateRes.Errors.Count == 0)
                {
                    MessageBox.Show(string.Format("Sikeres bontás!\n\nÚj hátralék rendelésszáma: {0}\nA végösszegek és adók elosztása sikeresen megtörtént.", suffixOrderNumber));
                    btnLoad_Click(null, EventArgs.Empty);
                    dgvItems.DataSource = null;
                }
            }
            catch (Exception ex) { MessageBox.Show("Hiba: " + ex.Message); }
        }

        private void FormatItemGrid()
        {
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }

    // Listbox osztálya
    public class LocalOrderSnapshot
    {
        public string Bvin { get; set; }
        public string OrderNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Ez fog megjelenni 
        public string DisplayText
        {
            get { return string.Format("{0} - {1} {2}", OrderNumber, FirstName, LastName); }
        }
    }

    // DGV osztálya
    public class LocalLineItemDTO
    {
        public string ProductSku { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public bool IsPacked { get; set; }
    }
}