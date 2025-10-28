using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mychurch.Common.Services;

namespace MyChurch.Infrastructure.Services.Reports
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        public async Task<byte[]> GenerateFinancialReportPdfAsync(FinancialReportDto report, string churchName)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // HEADER
                    page.Header()
                        .Height(100)
                        .Background(Colors.Blue.Medium)
                        .Padding(20)
                        .Text(text =>
                        {
                            text.Span($"{churchName}\n").FontSize(20).Bold().FontColor(Colors.White);
                            text.Span("Relatório Financeiro\n").FontSize(14).FontColor(Colors.White);
                            text.Span($"Período: {report.StartDate:dd/MM/yyyy} a {report.EndDate:dd/MM/yyyy}").FontSize(10).FontColor(Colors.White);
                        });

                    // CONTENT
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // RESUMO FINANCEIRO
                            column.Item().Text("📊 Resumo Financeiro").FontSize(16).Bold();
                            column.Item().PaddingLeft(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                });

                                table.Cell().Border(1).Padding(5).Text("Total de Receitas").Bold();
                                table.Cell().Border(1).Padding(5).Text($"R$ {report.TotalIncome:N2}").AlignRight();

                                table.Cell().Border(1).Padding(5).Text("Total de Despesas").Bold();
                                table.Cell().Border(1).Padding(5).Text($"R$ {report.TotalExpenses:N2}").AlignRight();

                                table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3).Text("Saldo").Bold();
                                table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3).Text($"R$ {report.NetBalance:N2}").Bold().AlignRight()
                                    .FontColor(report.NetBalance >=0 ? Colors.Green.Medium : Colors.Red.Medium);
                            });

                            column.Item().PaddingTop(15).Text("📥 Receitas Detalhadas").FontSize(14).Bold();
                            column.Item().PaddingLeft(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                });

                                table.Cell().Border(1).Padding(5).Text("Dízimos").Bold();
                                table.Cell().Border(1).Padding(5).Text($"R$ {report.TotalTithes:N2}").AlignRight();

                                table.Cell().Border(1).Padding(5).Text("Ofertas").Bold();
                                table.Cell().Border(1).Padding(5).Text($"R$ {report.TotalOfferings:N2}").AlignRight();

                                table.Cell().Border(1).Padding(5).Text("Doações").Bold();
                                table.Cell().Border(1).Padding(5).Text($"R$ {report.TotalDonations:N2}").AlignRight();

                                table.Cell().Border(1).Padding(5).Text("Outras Receitas").Bold();
                                table.Cell().Border(1).Padding(5).Text($"R$ {report.OtherIncome:N2}").AlignRight();
                            });

                            // DESPESAS POR CATEGORIA
                            if (report.ExpensesByCategory != null && report.ExpensesByCategory.Any())
                            {
                                column.Item().PaddingTop(15).Text("🧾 Despesas por Categoria").FontSize(14).Bold();
                                column.Item().PaddingLeft(10).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Categoria").Bold();
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Valor").Bold();
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("%").Bold();
                                    });

                                    foreach (var expense in report.ExpensesByCategory)
                                    {
                                        table.Cell().Border(1).Padding(5).Text(expense.CategoryName);
                                        table.Cell().Border(1).Padding(5).Text($"R$ {expense.TotalAmount:N2}").AlignRight();
                                        table.Cell().Border(1).Padding(5).Text($"{expense.Percentage:N1}%").AlignRight();
                                    }
                                });
                            }

                            // TOP DOADORES
                            if (report.TopDonors != null && report.TopDonors.Any())
                            {
                                column.Item().PaddingTop(15).Text("🏆 Top10 Doadores").FontSize(14).Bold();
                                column.Item().PaddingLeft(10).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(4);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Nome").Bold();
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Total").Bold();
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Qtd").Bold();
                                        header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Média").Bold();
                                    });

                                    foreach (var donor in report.TopDonors.Take(10))
                                    {
                                        table.Cell().Border(1).Padding(5).Text(donor.MemberName);
                                        table.Cell().Border(1).Padding(5).Text($"R$ {donor.TotalDonated:N2}").AlignRight();
                                        table.Cell().Border(1).Padding(5).Text(donor.DonationCount.ToString()).AlignRight();
                                        table.Cell().Border(1).Padding(5).Text($"R$ {donor.AverageDonation:N2}").AlignRight();
                                    }
                                });
                            }
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Gerado em: ");
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).Bold();
                            x.Span(" | Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return await Task.FromResult(document.GeneratePdf());
        }

        public async Task<byte[]> GenerateFinancialReportExcelAsync(FinancialReportDto report, string churchName)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Relatório Financeiro");

            // HEADER
            worksheet.Cell(1,1).Value = churchName;
            worksheet.Cell(1,1).Style.Font.FontSize =16;
            worksheet.Cell(1,1).Style.Font.Bold = true;

            worksheet.Cell(2,1).Value = "Relatório Financeiro";
            worksheet.Cell(2,1).Style.Font.FontSize =14;

            worksheet.Cell(3,1).Value = $"Período: {report.StartDate:dd/MM/yyyy} a {report.EndDate:dd/MM/yyyy}";

            // RESUMO
            var row =5;
            worksheet.Cell(row,1).Value = "RESUMO FINANCEIRO";
            worksheet.Cell(row,1).Style.Font.Bold = true;
            worksheet.Cell(row,1).Style.Fill.BackgroundColor = XLColor.LightBlue;

            row++;
            worksheet.Cell(row,1).Value = "Total de Receitas";
            worksheet.Cell(row,2).Value = report.TotalIncome;
            worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";

            row++;
            worksheet.Cell(row,1).Value = "Total de Despesas";
            worksheet.Cell(row,2).Value = report.TotalExpenses;
            worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";

            row++;
            worksheet.Cell(row,1).Value = "Saldo";
            worksheet.Cell(row,1).Style.Font.Bold = true;
            worksheet.Cell(row,2).Value = report.NetBalance;
            worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";
            worksheet.Cell(row,2).Style.Font.Bold = true;
            worksheet.Cell(row,2).Style.Font.FontColor = report.NetBalance >=0 ? XLColor.Green : XLColor.Red;

            // RECEITAS DETALHADAS
            row +=2;
            worksheet.Cell(row,1).Value = "RECEITAS DETALHADAS";
            worksheet.Cell(row,1).Style.Font.Bold = true;
            worksheet.Cell(row,1).Style.Fill.BackgroundColor = XLColor.LightGreen;

            row++;
            worksheet.Cell(row,1).Value = "Dízimos";
            worksheet.Cell(row,2).Value = report.TotalTithes;
            worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";

            row++;
            worksheet.Cell(row,1).Value = "Ofertas";
            worksheet.Cell(row,2).Value = report.TotalOfferings;
            worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";

            row++;
            worksheet.Cell(row,1).Value = "Doações";
            worksheet.Cell(row,2).Value = report.TotalDonations;
            worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";

            // DESPESAS POR CATEGORIA
            if (report.ExpensesByCategory != null && report.ExpensesByCategory.Any())
            {
                row +=2;
                worksheet.Cell(row,1).Value = "DESPESAS POR CATEGORIA";
                worksheet.Cell(row,1).Style.Font.Bold = true;
                worksheet.Cell(row,1).Style.Fill.BackgroundColor = XLColor.LightCoral;

                row++;
                worksheet.Cell(row,1).Value = "Categoria";
                worksheet.Cell(row,2).Value = "Valor";
                worksheet.Cell(row,3).Value = "%";
                worksheet.Range(row,1, row,3).Style.Font.Bold = true;

                foreach (var expense in report.ExpensesByCategory)
                {
                    row++;
                    worksheet.Cell(row,1).Value = expense.CategoryName;
                    worksheet.Cell(row,2).Value = expense.TotalAmount;
                    worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";
                    worksheet.Cell(row,3).Value = expense.Percentage /100;
                    worksheet.Cell(row,3).Style.NumberFormat.Format = "0.0%";
                }
            }

            // TOP DOADORES
            if (report.TopDonors != null && report.TopDonors.Any())
            {
                row +=2;
                worksheet.Cell(row,1).Value = "TOP DOADORES";
                worksheet.Cell(row,1).Style.Font.Bold = true;
                worksheet.Cell(row,1).Style.Fill.BackgroundColor = XLColor.Gold;

                row++;
                worksheet.Cell(row,1).Value = "Nome";
                worksheet.Cell(row,2).Value = "Total Doado";
                worksheet.Cell(row,3).Value = "Quantidade";
                worksheet.Cell(row,4).Value = "Média";
                worksheet.Range(row,1, row,4).Style.Font.Bold = true;

                foreach (var donor in report.TopDonors.Take(10))
                {
                    row++;
                    worksheet.Cell(row,1).Value = donor.MemberName;
                    worksheet.Cell(row,2).Value = donor.TotalDonated;
                    worksheet.Cell(row,2).Style.NumberFormat.Format = "R$ #,##0.00";
                    worksheet.Cell(row,3).Value = donor.DonationCount;
                    worksheet.Cell(row,4).Value = donor.AverageDonation;
                    worksheet.Cell(row,4).Style.NumberFormat.Format = "R$ #,##0.00";
                }
            }

            // AUTO-FIT
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }

        // STUBS para os outros métodos (podem ser implementados depois)
        public Task<byte[]> GenerateEngagementReportPdfAsync(EngagementReportDto report, string churchName)
        {
            throw new NotImplementedException("Engagement PDF report - to be implemented");
        }

        public Task<byte[]> GenerateEngagementReportExcelAsync(EngagementReportDto report, string churchName)
        {
            throw new NotImplementedException("Engagement Excel report - to be implemented");
        }

        public Task<byte[]> GenerateVisitorAnalyticsPdfAsync(VisitorAnalyticsDto report, string churchName)
        {
            throw new NotImplementedException("Visitor Analytics PDF - to be implemented");
        }

        public Task<byte[]> GenerateVisitorAnalyticsExcelAsync(VisitorAnalyticsDto report, string churchName)
        {
            throw new NotImplementedException("Visitor Analytics Excel - to be implemented");
        }
    }
}
