using Microsoft.EntityFrameworkCore;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mindora.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly MindoraDbContext _context;

        public InvoiceService(MindoraDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> DownloadInvoicePdfAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payment)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found.");

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Mindora")
                                .FontSize(24)
                                .Bold()
                                .FontColor("#06B6D4");
                            col.Item().Text("AI-Powered Mental Wellness Platform")
                                .FontSize(10)
                                .FontColor("#64748B");
                        });
                        row.ConstantItem(160).Column(col =>
                        {
                            col.Item().Text("INVOICE")
                                .FontSize(18)
                                .Bold()
                                .FontColor("#0F172A");
                            col.Item().Text(invoice.InvoiceNumber)
                                .FontSize(11)
                                .FontColor("#334155");
                            col.Item().Text(invoice.IssuedAt.ToString("dd MMM yyyy"))
                                .FontSize(10)
                                .FontColor("#64748B");
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Billed To")
                                    .Bold()
                                    .FontColor("#0F172A");
                                c.Item().Text(invoice.Payment.User?.UserName ?? invoice.Payment.User?.Email ?? "User")
                                    .FontColor("#334155");
                                c.Item().Text(invoice.Payment.User?.Email ?? "")
                                    .FontColor("#64748B");
                            });
                            row.ConstantItem(160).Column(c =>
                            {
                                c.Item().Text("Payment Details")
                                    .Bold()
                                    .FontColor("#0F172A");
                                c.Item().Text($"Gateway: {invoice.Payment.Gateway}")
                                    .FontColor("#334155");
                                c.Item().Text($"Transaction ID: {invoice.Payment.TransactionId}")
                                    .FontSize(9)
                                    .FontColor("#64748B");
                            });
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#E2E8F0");

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Description").Bold().FontColor("#0F172A");
                            row.ConstantItem(100).AlignRight().Text("Amount").Bold().FontColor("#0F172A");
                        });

                        col.Item().PaddingVertical(6).Row(row =>
                        {
                            row.RelativeItem().Text(invoice.Payment.PaymentType);
                            row.ConstantItem(100).AlignRight().Text($"{invoice.Amount} {invoice.Payment.Currency}");
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#E2E8F0");

                        col.Item().AlignRight().Row(row =>
                        {
                            row.RelativeItem().Text("Total").Bold().FontColor("#0F172A");
                            row.ConstantItem(100).AlignRight().Text($"{invoice.Amount} {invoice.Payment.Currency}")
                                .Bold()
                                .FontColor("#06B6D4");
                        });
                    });

                    page.Footer().AlignCenter().Text("Thank you for using Mindora.")
                        .FontSize(10)
                        .FontColor("#94A3B8");
                });
            });

            return pdf.GeneratePdf();
        }
    }
}