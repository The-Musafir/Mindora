using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<byte[]> DownloadInvoicePdfAsync(Guid invoiceId);
    }
}