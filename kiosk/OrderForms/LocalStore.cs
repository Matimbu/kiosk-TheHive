using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace kiosk
{
    public class SavedOrder
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public List<Order> Lines { get; set; }
        public decimal Total { get { return Lines.Sum(o => o.Total); } }

        // Counts up from 1 each day, so the cashier can call "zero four two"
        // instead of spelling out a hex code where 0/O and 8/B sound alike.
        // Orders saved before daily numbers existed read 0 and keep their code.
        public int Number { get; set; }

        public string DisplayNumber
        {
            get
            {
                if (Number > 0) return Number.ToString("000");
                if (string.IsNullOrEmpty(Id)) return "—";
                int separator = Id.LastIndexOf('-');
                string code = separator >= 0 ? Id.Substring(separator + 1) : Id;
                return "#" + code.ToUpperInvariant();
            }
        }
        public override string ToString() { return DisplayNumber + "  " + Created.ToString("MM/dd HH:mm") + "  " + Method + "  " + Total.ToString("N2") + "  " + Status; }
    }

    public static class LocalStore
    {
        public static string Root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TheHiveCafe");
        public static string ReceiptFolder { get { return Path.Combine(Root, "Receipts"); } }

        public static void Write<T>(string path, T value)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    new XmlSerializer(typeof(T)).Serialize(stream, value);
                    stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temp, path, null);
                else File.Move(temp, path);
            }
            finally { if (File.Exists(temp)) File.Delete(temp); }
        }

        public static T Read<T>(string path)
        {
            using (var stream = File.OpenRead(path)) return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
        }

        public static SavedOrder Submit(string method)
        {
            OrderStorage.ValidateCart();
            DateTime now = DateTime.Now;
            var order = new SavedOrder {
                Id = now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6),
                Number = NextNumber(now),
                Created = now, Method = method,
                Status = method == "Cash" ? "Pending counter payment" : "Demo - no payment taken",
                Lines = OrderStorage.Orders.Select(o => new Order { Product = o.Product, Size = o.Size,
                    Temperature = o.Temperature, Sweetness = o.Sweetness, Quantity = o.Quantity,
                    Price = o.Price, ImagePath = o.ImagePath }).ToList()
            };
            Write(Path.Combine(Root, "Orders", order.Id + ".xml"), order);
            return order;
        }

        // Worked out from the day's saved orders rather than kept in a separate
        // counter, so it survives a restart and cannot drift from the records.
        // Record ids start with the date, so only today's files are read. One
        // that cannot be read still counts, so its number is never reused.
        private static int NextNumber(DateTime now)
        {
            string folder = Path.Combine(Root, "Orders");
            if (!Directory.Exists(folder)) return 1;
            string[] today = Directory.GetFiles(folder, now.ToString("yyyyMMdd") + "-*.xml");
            int highest = today.Length;
            foreach (string file in today)
            {
                try { highest = Math.Max(highest, Read<SavedOrder>(file).Number); }
                catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is UnauthorizedAccessException) { }
            }
            return highest + 1;
        }

        public static List<SavedOrder> History(out int unreadable)
        {
            unreadable = 0;
            var result = new List<SavedOrder>();
            string folder = Path.Combine(Root, "Orders");
            if (!Directory.Exists(folder)) return result;
            foreach (string file in Directory.GetFiles(folder, "*.xml"))
            {
                try { var order = Read<SavedOrder>(file); if (order.Lines == null) throw new InvalidDataException(); result.Add(order); }
                catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is UnauthorizedAccessException) { unreadable++; }
            }
            return result.OrderByDescending(o => o.Created).ToList();
        }

        public static HashSet<string> SoldOut = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public static void LoadAvailability()
        {
            string path = Path.Combine(Root, "availability.xml");
            if (File.Exists(path)) SoldOut = new HashSet<string>(Read<List<string>>(path), StringComparer.OrdinalIgnoreCase);
        }
        public static void SetSoldOut(string name, bool soldOut)
        {
            var updated = new HashSet<string>(SoldOut, StringComparer.OrdinalIgnoreCase);
            if (soldOut) updated.Add(name); else updated.Remove(name);
            Write(Path.Combine(Root, "availability.xml"), updated.ToList());
            SoldOut = updated;
        }
    }
}
