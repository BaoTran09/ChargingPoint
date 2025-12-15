using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using ChargingApp.Helpers;
using ChargingApp.Helper;
using DevExpress.XtraCharts.Design;

namespace ChargingApp.ThanhPhan
{
    public partial class frmDashboard : DevExpress.XtraEditors.XtraForm
    {
        private DateTime currentFromDate;
        private DateTime currentToDate;
        private Timer refreshTimer;

        public frmDashboard()
        {
            InitializeComponent();

        }

        private void frmDashboard_Load(object sender, EventArgs e)
        {
            InitializeForm();
            LoadDashboardData();
            StartAutoRefresh();
            this.ControlBox = false;

        }

        #region Initialization

        private void InitializeForm()
        {
            // Thiết lập ngày mặc định (tháng hiện tại)
            currentFromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            currentToDate = DateTime.Now;

            DEfromDate.EditValue = currentFromDate;
            DEtoDate.EditValue = currentToDate;

            // Wire up events
            DEfromDate.EditValueChanged += DateEdit_EditValueChanged;
            DEtoDate.EditValueChanged += DateEdit_EditValueChanged;

            // Initialize charts
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            // Column Chart - Station Revenue
            ConfigureColumnChart(ColChartCompareStation, "Doanh thu theo Trạm sạc");

            // Pie Chart - Charger Usage
            ConfigurePieChart(PieChartChargerUseMost, "Trụ sạc sử dụng nhiều nhất");

            // Line Chart - Revenue Trend
            ConfigureLineChart(LineChartRevenue, "Xu hướng Doanh thu");

            // Pie Chart - Payment Status
            ConfigurePieChart(PieChartPaymentStatus, "Trạng thái Thanh toán");
        }

        private void ConfigureColumnChart(ChartControl chart, string title)
        {
            chart.Titles.Clear();
            chart.Titles.Add(new ChartTitle { Text = title, Font = new Font("Tahoma", 12, FontStyle.Bold) });

            Series series = new Series("Doanh thu", ViewType.Bar);
            series.ArgumentScaleType = ScaleType.Qualitative;
            chart.Series.Clear();
            chart.Series.Add(series);

            BarSeriesView view = series.View as BarSeriesView;
            if (view != null)
            {
                view.Color = Color.FromArgb(79, 129, 189);
            }

            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series.Label.TextPattern = "{V:N0} VND";
        }

        private void ConfigurePieChart(ChartControl chart, string title)
        {
            chart.Titles.Clear();
            chart.Titles.Add(new ChartTitle { Text = title, Font = new Font("Tahoma", 12, FontStyle.Bold) });

            Series series = new Series("Data", ViewType.Pie);
            chart.Series.Clear();
            chart.Series.Add(series);

            PieSeriesView view = series.View as PieSeriesView;
            if (view != null)
            {
                view.RuntimeExploding = false;
            }

            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series.Label.TextPattern = "{A}: {VP:P0}";
            series.LegendTextPattern = "{A}";
        }

        private void ConfigureLineChart(ChartControl chart, string title)
        {
            chart.Titles.Clear();
            chart.Titles.Add(new ChartTitle { Text = title, Font = new Font("Tahoma", 12, FontStyle.Bold) });

            chart.Series.Clear();

            // Series 1: Total Revenue
            Series seriesRevenue = new Series("Doanh thu", ViewType.Line);
            seriesRevenue.ArgumentScaleType = ScaleType.DateTime;
            LineSeriesView viewRevenue = seriesRevenue.View as LineSeriesView;
            if (viewRevenue != null)
            {
                viewRevenue.Color = Color.FromArgb(79, 129, 189);
                viewRevenue.LineStyle.Thickness = 2;
                viewRevenue.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            }
            chart.Series.Add(seriesRevenue);

            // Series 2: Paid Revenue
            Series seriesPaid = new Series("Đã thanh toán", ViewType.Line);
            seriesPaid.ArgumentScaleType = ScaleType.DateTime;
            LineSeriesView viewPaid = seriesPaid.View as LineSeriesView;
            if (viewPaid != null)
            {
                viewPaid.Color = Color.FromArgb(155, 187, 89);
                viewPaid.LineStyle.Thickness = 2;
                viewPaid.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            }
            chart.Series.Add(seriesPaid);

            seriesRevenue.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            seriesPaid.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;

            chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
        }

        private void StartAutoRefresh()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 60000; // 1 minute
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        #endregion

        #region Load Data

        private void LoadDashboardData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // 1. Load Summary Cards
                LoadSummaryCards();

                // 2. Load Charts
                LoadStationRevenueChart();
                LoadChargerUsageChart();
                LoadRevenueLineChart();
                LoadPaymentStatusChart();

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                XtraMessageBox.Show($"Lỗi tải dữ liệu Dashboard: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

   