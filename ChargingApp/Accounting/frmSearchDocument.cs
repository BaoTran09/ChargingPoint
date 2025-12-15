using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;

namespace ChargingApp.Accounting
{
    public partial class frmSearchDocument : XtraForm
    {
        private readonly string connectionString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";

        private readonly string documentType;
        public string SelectedDocumentId { get; private set; }

        public frmSearchDocument(string docType)
        {
            InitializeComponent();
            documentType = docType;
        }

        private void frmSearchDocument_Load(object sender, EventArgs e)
        {
            InitializeForm();
            SearchDocuments();
        }

        private void InitializeForm()
        {
            this.Text = documentType == "Invoice" ? "Tìm kiếm Hóa đơn" : "Tìm kiếm Phiếu thu";

            deFromDate.EditValue = DateTime.Today.AddDays(-30);
            deToDate.EditValue = DateTime.Today;

            SetupGridColumns();
        }

        private void SetupGridColumns()
        {
            gvDocuments.Columns.Clear();

            if (documentType == "Invoice")
            {
                AddColumn("InvoiceId", "Mã HĐ", 120);
                AddColumn("InvoiceNumber", "Số hóa đơn", 100);
                AddColumn("CreatedAt", "Ngày lập", 130, "dd/MM/yyyy HH:mm");
                AddColumn("CustomerName", "Khách hàng", 200);
                AddColumn("StationName", "Trạm sạc", 180);
                AddColumn("Total", "Tổng tiền", 120, "n0");
                AddColumn("Status", "Trạng thái", 100);
            }
            else
            {
                AddColumn("Id", "Mã phiếu", 100);
                AddColumn("ReceiptNo", "Số phiếu", 100);
                AddColumn("ReceiptDate", "Ngày lập", 130, "dd/MM/yyyy");
                AddColumn("Customer_name", "Khách hàng", 200);
                AddColumn("TotalAmount", "Số tiền", 120, "n0");
                AddColumn("Method_payment", "PTTT", 100);
            }

            gvDocuments.BestFitColumns();
        }

        private void AddColumn(string fieldName, string caption, int width, string format = "")
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = caption;
            col.Width = width;
            col.Visible = true;

            if (!string.IsNullOrEmpty(format))
            {
                if (format.Contains("/"))
                {
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    col.DisplayFormat.FormatString = format;
                }
                else if (format == "n0")
                {
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "n0";
                }
            }

            gvDocuments.Columns.Add(col);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchDocuments();
        }

        private void SearchDocuments()
        {
            try
            {
                DataTable dt = documentType == "Invoice" ? SearchInvoices() : SearchReceipts();
                gcDocuments.DataSource = dt;

                if (dt.Rows.Count == 0)
                    XtraMessageBox.Show("Không tìm thấy dữ liệu phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable SearchInvoices()
        {
            string query = @"
                SELECT 
                    i.InvoiceId,
                    i.InvoiceNumber,
                    i.CreatedAt,
                    ISNULL(c.FullName, i.Snashot_CustomerName) AS CustomerName,
                    ISNULL(s.Name, 'Trạm không xác định') AS StationName,
                    ISNULL(t.Total, 0) AS Total,
                    i.Status
                FROM Invoices i
                LEFT JOIN Customer c ON i.CustomerId = c.CustomerId
                LEFT JOIN ChargingSession cs ON i.SessionId = cs.SessionId
                LEFT JOIN Connector con ON cs.ConnectorId = con.ConnectorId
                LEFT JOIN Charger ch ON con.ChargerId = ch.ChargerId
                LEFT JOIN Station s ON ch.StationId = s.StationId
                LEFT JOIN (
                    SELECT InvoiceId,SUM((Quantities*UnitPrice) - Quantities + TaxAmount) AS Total
                    FROM InvoiceDetail
                    GROUP BY InvoiceId
                ) t ON i.InvoiceId = t.InvoiceId
                WHERE CAST(i.CreatedAt AS DATE) BETWEEN @From AND @To
            ";

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                query += @"
                    AND (
                        CAST(i.InvoiceId AS NVARCHAR) LIKE @Search 
                        OR CAST(i.InvoiceNumber AS NVARCHAR) LIKE @Search
                        OR ISNULL(c.FullName, i.Snashot_CustomerName) LIKE @Search
                        OR ISNULL(s.Name, '') LIKE @Search
                    )";
            }

            query += " ORDER BY i.CreatedAt DESC";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@From", ((DateTime)deFromDate.EditValue).Date);
                    cmd.Parameters.AddWithValue("@To", ((DateTime)deToDate.EditValue).Date);
                    if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                        cmd.Parameters.AddWithValue("@Search", $"%{txtSearch.Text.Trim()}%");

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        private DataTable SearchReceipts()
        {
            string query = @"
                SELECT 
                    r.Id,
                    r.ReceiptNo,
                    r.ReceiptDate,
                    ISNULL(c.FullName, c.Customer_name) AS Customer_name,
                    r.TotalAmount,
                    r.Method_payment
                FROM Receipt r
                LEFT JOIN Customer c ON r.CustomerId = c.CustomerId
                WHERE CAST(r.ReceiptDate AS DATE) BETWEEN @From AND @To
            ";

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                query += " AND (r.ReceiptNo LIKE @Search OR ISNULL(c.FullName, '') LIKE @Search)";
            }

            query += " ORDER BY r.ReceiptDate DESC";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@From", ((DateTime)deFromDate.EditValue).Date);
                    cmd.Parameters.AddWithValue("@To", ((DateTime)deToDate.EditValue).Date);
                    if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                        cmd.Parameters.AddWithValue("@Search", $"%{txtSearch.Text.Trim()}%");

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        private void gvDocuments_DoubleClick(object sender, EventArgs e)
        {
            SelectDocument();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectDocument();
        }

        private void SelectDocument()
        {
            int rowHandle = gvDocuments.FocusedRowHandle;
            if (rowHandle < 0)
            {
                XtraMessageBox.Show("Vui lòng chọn một dòng!", "Chọn chứng từ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idColumn = documentType == "Invoice" ? "InvoiceId" : "Id";
            object value = gvDocuments.GetRowCellValue(rowHandle, idColumn);

            if (value == null || value == DBNull.Value)
            {
                XtraMessageBox.Show("Không lấy được mã chứng từ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedDocumentId = value.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void frmSearchDocument_Load_1(object sender, EventArgs e)
        {

        }
    }
}