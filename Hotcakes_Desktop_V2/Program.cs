using System;
using System.Windows.Forms;
using Hotcakes.CommerceDTO.v1.Client; // A hivatalos DLL hivatkozása

namespace Hotcakes_Desktop_V2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Windows Forms alapbeállítások
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // --- ÖNÁLLÓ KAPCSOLAT TESZT ---
            // Ezeket az adatokat használjuk a teszthez (ugyanaz, mint ami a Form1-ben lesz)
            string rootUrl = "http://74.234.38.79";
            string apiKey = "1-c82617a1-53ab-41ce-bc85-d4d1efc12f9e";

            try
            {
                // 1. Lépés: DLL betöltés ellenőrzése
                // Létrehozunk egy teszt API objektumot. Ha a DLL nem tölthető be (pl. x86 hiba), itt megáll.
                var proxy = new Api(rootUrl, apiKey);

                // 2. Lépés: Szerver elérés ellenőrzése
                // Megpróbálunk egy nagyon gyors hívást (pl. kategóriák listázása), 
                // csak hogy lássuk, él-e a vonal.
                var testCall = proxy.CategoriesFindAll();

                if (testCall.Errors.Count > 0)
                {
                    // Ha a szerver válaszol, de hiba van (pl. rossz kulcs)
                    MessageBox.Show("A Hotcakes szerver elérhető, de az API kulcs hibás lehet!\n\n" +
                                    "Hiba: " + testCall.Errors[0].Description,
                                    "Kapcsolati Figyelmeztetés",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Ez akkor fut le, ha a DLL hiányzik, vagy ARM/x86 ütközés van
                MessageBox.Show("KRITIKUS HIBA: A program nem tud kapcsolódni a Hotcakes DLL-hez!\n\n" +
                                "Lehetséges okok:\n" +
                                "1. A projekt nincs 'x86' platformra állítva (ARM Mac miatt fontos!)\n" +
                                "2. Hiányzik a Hotcakes.CommerceDTO.v1.Client.dll fájl.\n\n" +
                                "Részletek: " + ex.Message,
                                "Indítási Hiba",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

                return; // Nem indítjuk el a Form1-et, ha alapvető hiba van
            }

            // --- HA A TESZT SIKERES, INDUL AZ ABLAK ---
            try
            {
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a grafikus felület (Form1) betöltésekor:\n\n" + ex.ToString(),
                                "Megjelenítési Hiba",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}