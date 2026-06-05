using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;
using GuayaquilBank.Domain.Entities.Sales;
using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;

namespace GuayaquilBank.Application.Services
{
    public class SalesAppService : ISalesAppService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<SalesAppService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SalesAppService(
            IApplicationDbContext context,
            ILogger<SalesAppService> logger,
            ICurrentUser currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _logger = logger;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequestDto request)
        {
            var companyId = _currentUser.CompanyId ?? Guid.Empty;
            _logger.LogInformation("Iniciando creación de factura para el cliente '{ClientName}'", request.ClientName);

            if (_context is not DbContext efContext)
            {
                throw new InvalidOperationException("El contexto de base de datos no soporta transacciones.");
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                throw new UserFriendlyException("La configuración corporativa de su empresa no pudo ser localizada.");
            }

            using var transaction = await efContext.Database.BeginTransactionAsync();

            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Identification == request.ClientIdentification.Trim());

                if (customer == null)
                {
                    _logger.LogInformation("Cliente nuevo detectado. Registrando en la base de datos...");
                    customer = new Customer(
                        id: Guid.NewGuid(),
                        companyId: companyId,
                        identification: request.ClientIdentification.Trim(),
                        fullName: request.ClientName.Trim(),
                        email: $"{request.ClientIdentification.Trim()}@guayaquilbank.tmp",
                        phoneNumber: string.Empty,
                        address: string.Empty
                    );
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync(default);
                }

                var lastInvoiceNumber = await _context.Invoices
                    .Where(i => i.CompanyId == companyId)
                    .OrderByDescending(i => i.IssuedAtUtc)
                    .Select(i => i.InvoiceNumber)
                    .FirstOrDefaultAsync();

                string nextInvoiceNumber = GenerateNextInvoiceNumber(lastInvoiceNumber);

                var invoice = new Invoice(
                    id: Guid.NewGuid(),
                    companyId: companyId,
                    customerId: customer.Id,
                    invoiceNumber: nextInvoiceNumber,
                    issuedAtUtc: _dateTimeProvider.UtcNow
                );

                foreach (var detailDto in request.Details)
                {
                    var batch = await _context.ProductBatches
                        .Include(b => b.Product)
                        .FirstOrDefaultAsync(b => b.Id == detailDto.ProductBatchId && b.Product.CompanyId == companyId);

                    if (batch == null)
                    {
                        throw new UserFriendlyException($"El lote seleccionado para el producto no existe en sus registros de inventario.");
                    }

                    if (batch.IsExpired(_dateTimeProvider.UtcNow.Date))
                    {
                        throw new UserFriendlyException($"No se puede facturar el producto '{batch.Product.Name}' porque el lote '{batch.BatchNumber}' se encuentra expirado.");
                    }

                    try
                    {
                        batch.DeductStock(detailDto.Quantity);
                    }
                    catch (ArgumentException)
                    {
                        throw new UserFriendlyException($"Stock insuficiente en el lote '{batch.BatchNumber}' para el producto '{batch.Product.Name}'. Disponible: {batch.Quantity}");
                    }

                    invoice.AddItem(
                        detailId: Guid.NewGuid(),
                        productBatchId: batch.Id,
                        quantity: detailDto.Quantity,
                        unitPrice: detailDto.UnitPrice,
                        taxPercentage: company.Iva
                    );
                }

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(default);

                await transaction.CommitAsync();

                _logger.LogInformation("Factura {InvoiceNumber} generada de manera exitosa.", invoice.InvoiceNumber);

                return new InvoiceResponseDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    IssuedAt = invoice.IssuedAtUtc,
                    ClientName = customer.FullName,
                    ClientIdentification = customer.Identification,
                    Subtotal = invoice.SubTotal,
                    Tax = invoice.TotalTax,
                    Total = invoice.Total
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginationResponseDto<InvoiceResponseDto>> GetPagedInvoicesAsync(PaginationRequestDto request)
        {
            var query = _context.Invoices
                .Include(i => i.Customer)
                .Where(i => i.CompanyId == _currentUser.CompanyId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(i => i.InvoiceNumber.Contains(search) || i.Customer.FullName.ToLower().Contains(search));
            }

            var totalItems = await query.CountAsync();

            var invoices = await query
                .OrderByDescending(i => i.IssuedAtUtc)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = invoices.Select(i => new InvoiceResponseDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                IssuedAt = i.IssuedAtUtc,
                ClientName = i.Customer.FullName,
                ClientIdentification = i.Customer.Identification,
                Subtotal = i.SubTotal,
                Tax = i.TotalTax,
                Total = i.Total
            }).ToList();

            return new PaginationResponseDto<InvoiceResponseDto>(dtos, totalItems, request.Page, request.PageSize);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.CompanyId == _currentUser.CompanyId);

            if (invoice == null) return null;

            return new InvoiceResponseDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                IssuedAt = invoice.IssuedAtUtc,
                ClientName = invoice.Customer.FullName,
                ClientIdentification = invoice.Customer.Identification,
                Subtotal = invoice.SubTotal,
                Tax = invoice.TotalTax,
                Total = invoice.Total
            };
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId)
        {
            _logger.LogInformation("Generando reporte físico PDF profesional para la factura ID: {InvoiceId}", invoiceId);

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var invoice = await _context.Invoices
                .Include(i => i.Company)
                .Include(i => i.Customer)
                .Include(i => i.Details).ThenInclude(d => d.ProductBatch).ThenInclude(b => b.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.CompanyId == _currentUser.CompanyId);

            if (invoice == null)
            {
                throw new UserFriendlyException("La factura solicitada para la generación del PDF no existe.");
            }

            var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40, QuestPDF.Infrastructure.Unit.Point);
                    page.Size(QuestPDF.Helpers.PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(invoice.Company.Name.ToUpper()).Bold().FontSize(16).FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);
                            col.Item().Text($"RUC: {invoice.Company.TaxId}").Bold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            col.Item().Text($"Dirección: {invoice.Company.Address}, {invoice.Company.City}");
                            col.Item().Text($"Teléfono: {invoice.Company.PhoneNumber}");
                            col.Item().Text($"Email: {invoice.Company.Email}");
                        });

                        row.ConstantItem(180).Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(10).Column(col =>
                        {
                            col.Spacing(5);
                            col.Item().Text("FACTURA").Bold().FontSize(14).FontColor(QuestPDF.Helpers.Colors.Blue.Darken4).AlignCenter();
                            col.Item().Text($"Nº: {invoice.InvoiceNumber}").Bold().FontSize(11).AlignCenter();
                            col.Item().Text($"Fecha: {invoice.IssuedAtUtc:dd/MM/yyyy HH:mm}").FontSize(9).AlignCenter();
                        });
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(8).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(x => { x.Span("CLIENTE: ").Bold(); x.Span(invoice.Customer.FullName); });
                                col.Item().Text(x => { x.Span("RUC/CÉDULA: ").Bold(); x.Span(invoice.Customer.Identification); });
                            });
                        });

                        column.Item().PaddingBottom(15);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4); // Producto
                                columns.RelativeColumn(2); // Lote
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(2); // P. Unitario
                                columns.RelativeColumn(2); // Subtotal
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken4).Padding(5).Text("Descripción").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken4).Padding(5).Text("Lote").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken4).Padding(5).Text("Cant.").Bold().FontColor(QuestPDF.Helpers.Colors.White).AlignRight();
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken4).Padding(5).Text("P. Unit").Bold().FontColor(QuestPDF.Helpers.Colors.White).AlignRight();
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken4).Padding(5).Text("Total").Bold().FontColor(QuestPDF.Helpers.Colors.White).AlignRight();
                            });

                            foreach (var detail in invoice.Details)
                            {
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5)
                                    .Text(detail.ProductBatch.Product.Name);

                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5)
                                    .Text(detail.ProductBatch.BatchNumber);

                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5)
                                    .Text(detail.Quantity.ToString()).AlignRight();

                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5)
                                    .Text($"{invoice.Company.CurrencySymbol}{detail.UnitPrice:F2}").AlignRight();

                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5)
                                    .Text($"{invoice.Company.CurrencySymbol}{detail.SubTotal:F2}").AlignRight();
                            }
                        });

                        column.Item().PaddingBottom(15);

                        column.Item().AlignRight().Width(180).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Cell().Padding(3).Text("Subtotal:").AlignRight();
                            table.Cell().Padding(3).Text($"{invoice.Company.CurrencySymbol}{invoice.SubTotal:F2}").AlignRight();

                            table.Cell().Padding(3).Text($"IVA ({(invoice.Company.Iva * 100):F0}%):").AlignRight();
                            table.Cell().Padding(3).Text($"{invoice.Company.CurrencySymbol}{invoice.TotalTax:F2}").AlignRight();

                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(5).Text("TOTAL:").Bold().AlignRight();
                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(5).Text($"{invoice.Company.CurrencySymbol}{invoice.Total:F2}").Bold().AlignRight();
                        });
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Página ").FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        x.CurrentPageNumber().FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        x.Span(" de ").FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                        x.TotalPages().FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        private static string GenerateNextInvoiceNumber(string? lastNumber)
        {
            if (string.IsNullOrWhiteSpace(lastNumber))
            {
                return "001-001-000000001";
            }

            var parts = lastNumber.Split('-');
            if (parts.Length == 3 && long.TryParse(parts[2], out long currentSequence))
            {
                long nextSequence = currentSequence + 1;
                return $"{parts[0]}-{parts[1]}-{nextSequence.ToString("D9")}";
            }

            return "001-001-000000001";
        }
    }
}