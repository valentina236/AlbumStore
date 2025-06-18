using AlbumStore.Helpers;
using Stimulsoft.Report;
using System.Windows.Input;

namespace AlbumStore.ViewModels
{
    public class ReportViewModel : BaseViewModel
    {
        public ICommand OpenStatisticsSalesReportCommand { get; }
        public ICommand OpenRevenueByMonthReportCommand { get; }

        public ReportViewModel()
        {
            OpenStatisticsSalesReportCommand = new RelayCommand(o => OpenStatisticsSalesReport());
            OpenRevenueByMonthReportCommand = new RelayCommand(o => OpenRevenueByMonthReport());
        }

        private void OpenStatisticsSalesReport()
        {
            var report = new StiReport();
            report.Load("../../Reports/StatisticsSales.mrt");
            report.ShowWithWpf();
        }

        private void OpenRevenueByMonthReport()
        {
            var report = new StiReport();
            report.Load("../../Reports/SalesMonths.mrt");
            report.ShowWithWpf();
        }
    }
}
