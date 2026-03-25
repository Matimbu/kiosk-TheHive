using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;   // NuGet: System.Data.SQLite (Install-Package System.Data.SQLite)
using System.IO;
using System.Windows.Forms;

namespace kiosk
{
    /// <summary>
    /// Handles all SQLite database operations for The Hive Cafe kiosk.
    ///
    /// SETUP — one-time step:
    ///   In Visual Studio, open the Package Manager Console and run:
    ///     Install-Package System.Data.SQLite
    ///   The database file (HiveCafe.db) will be created automatically in the
    ///   same folder as the .exe on first run.
    ///
    /// TABLES:
    ///   Transactions  – one row per completed payment session
    ///   TransactionItems – individual line items per transaction
    /// </summary>
    public static class DatabaseManager
    {
        // ── Connection ────────────────────────────────────────────────────
        private static string DbPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HiveCafe.db");

        private static string ConnectionString =>
            $"Data Source={DbPath};Version=3;";

        // ── Initialise (call once from Program.cs or Form1 constructor) ───
        public static void Initialise()
        {
            try
            {
                if (!File.Exists(DbPath))
                    SQLiteConnection.CreateFile(DbPath);

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        // Transactions table
                        cmd.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Transactions (
                                Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                                TransactionDate TEXT    NOT NULL,
                                PaymentMethod TEXT    NOT NULL,
                                TotalAmount   REAL     NOT NULL
                            );";
                        cmd.ExecuteNonQuery();

                        // Line items table
                        cmd.CommandText = @"
                            CREATE TABLE IF NOT EXISTS TransactionItems (
                                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                                TransactionId   INTEGER NOT NULL,
                                ProductName     TEXT    NOT NULL,
                                Size            TEXT,
                                Temperature     TEXT,
                                Quantity        INTEGER NOT NULL,
                                UnitPrice       REAL    NOT NULL,
                                Subtotal        REAL    NOT NULL,
                                FOREIGN KEY (TransactionId) REFERENCES Transactions(Id)
                            );";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database initialisation failed:\n{ex.Message}\n\n" +
                    "Make sure System.Data.SQLite is installed via NuGet.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ── Save a completed transaction ──────────────────────────────────
        /// <summary>
        /// Saves a completed order to the database.
        /// Returns the new Transaction ID, or -1 on failure.
        /// </summary>
        public static long SaveTransaction(List<Order> orders, string paymentMethod)
        {
            if (orders == null || orders.Count == 0)
                return -1;

            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        long transactionId;

                        // 1. Insert header row
                        using (var cmd = conn.CreateCommand())
                        {
                            decimal total = 0;
                            foreach (var o in orders)
                                total += o.Price * o.Quantity;

                            cmd.CommandText = @"
                                INSERT INTO Transactions (TransactionDate, PaymentMethod, TotalAmount)
                                VALUES (@date, @method, @total);
                                SELECT last_insert_rowid();";

                            cmd.Parameters.AddWithValue("@date",   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@method", paymentMethod);
                            cmd.Parameters.AddWithValue("@total",  (double)total);

                            transactionId = (long)cmd.ExecuteScalar();
                        }

                        // 2. Insert each line item
                        foreach (var order in orders)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"
                                    INSERT INTO TransactionItems
                                        (TransactionId, ProductName, Size, Temperature, Quantity, UnitPrice, Subtotal)
                                    VALUES
                                        (@txId, @product, @size, @temp, @qty, @price, @sub);";

                                cmd.Parameters.AddWithValue("@txId",    transactionId);
                                cmd.Parameters.AddWithValue("@product", order.Product ?? "");
                                cmd.Parameters.AddWithValue("@size",    order.Size ?? "");
                                cmd.Parameters.AddWithValue("@temp",    order.Temperature ?? "");
                                cmd.Parameters.AddWithValue("@qty",     order.Quantity);
                                cmd.Parameters.AddWithValue("@price",   (double)order.Price);
                                cmd.Parameters.AddWithValue("@sub",     (double)(order.Price * order.Quantity));

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return transactionId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to save transaction to database:\n{ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return -1;
            }
        }

        // ── Query helpers ─────────────────────────────────────────────────

        /// <summary>Returns all transactions as a DataTable (for a future Sales Report form).</summary>
        public static DataTable GetAllTransactions()
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT
                                Id            AS '#',
                                TransactionDate AS 'Date',
                                PaymentMethod AS 'Payment',
                                TotalAmount   AS 'Total (₱)'
                            FROM Transactions
                            ORDER BY Id DESC;";

                        using (var adapter = new SQLiteDataAdapter(cmd))
                            adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load transactions:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        /// <summary>Returns all items for a specific transaction.</summary>
        public static DataTable GetTransactionItems(long transactionId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT
                                ProductName AS 'Item',
                                Size        AS 'Size',
                                Temperature AS 'Temp',
                                Quantity    AS 'Qty',
                                UnitPrice   AS 'Unit Price (₱)',
                                Subtotal    AS 'Subtotal (₱)'
                            FROM TransactionItems
                            WHERE TransactionId = @id;";

                        cmd.Parameters.AddWithValue("@id", transactionId);

                        using (var adapter = new SQLiteDataAdapter(cmd))
                            adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load items:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        /// <summary>Returns total revenue between two dates.</summary>
        public static decimal GetRevenueBetween(DateTime from, DateTime to)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT COALESCE(SUM(TotalAmount), 0)
                            FROM   Transactions
                            WHERE  TransactionDate BETWEEN @from AND @to;";

                        cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@to",   to.ToString("yyyy-MM-dd HH:mm:ss"));

                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0m;
                    }
                }
            }
            catch
            {
                return 0m;
            }
        }
    }
}
