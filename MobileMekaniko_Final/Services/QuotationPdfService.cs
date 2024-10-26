using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MobileMekaniko_Final.Models.Dto;
using System.IO;
using MigraDoc.DocumentObjectModel.Tables;

namespace MobileMekaniko_Final.Services
{
    public class QuotationPdfService : IQuotationPdfService
    {
        public byte[] CreateQuotationPdf(QuotationDetailsDto quotation)
        {
            var document = new Document();
            var section = document.AddSection();

            // Set page margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);

            AddHeaderWithLogo(section);
            AddCustomerCarAndQuotationDetails(section, quotation);
            AddCompanyDetails(section);
            AddQuotationItemsTable(section, quotation);
            AddQuotationTotal(section, quotation);
            AddNotes(section, quotation);

            var renderer = new PdfDocumentRenderer();
            renderer.Document = document;
            renderer.RenderDocument();

            using (MemoryStream stream = new MemoryStream())
            {
                renderer.PdfDocument.Save(stream, false);
                return stream.ToArray();
            }
        }

        private void AddHeaderWithLogo(Section section)
        {
            var headerTable = section.Headers.Primary.AddTable();

            // Define columns for the logo, an empty spacer column, and the title
            headerTable.AddColumn(Unit.FromCentimeter(3));  // Column for the logo
            headerTable.AddColumn(Unit.FromCentimeter(1.8));  // Spacer column to adjust centering of title
            headerTable.AddColumn(Unit.FromCentimeter(10));  // Column for the title

            var row = headerTable.AddRow();

            // Set row height to ensure both logo and title are aligned properly
            row.Height = Unit.FromCentimeter(2.5);
            row.HeightRule = RowHeightRule.AtLeast;

            // Add the logo and ensure vertical alignment is set properly
            var logoPath = "wwwroot/Images/MMLogo-png.png"; // Path to the logo
            var logo = row.Cells[0].AddImage(logoPath);
            logo.LockAspectRatio = true;
            logo.Width = Unit.FromCentimeter(2.0);
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center; // Align logo to the center of the cell

            // Add an empty paragraph in the spacer column to help center the title
            var spacer = row.Cells[1].AddParagraph();
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;

            // Add the title
            var header = row.Cells[2].AddParagraph("Mobile Mekaniko Quotation");
            header.Format.Font.Size = 20;
            header.Format.Font.Bold = true;
            header.Format.Alignment = ParagraphAlignment.Left; // Align title within its own column to the left
            row.Cells[2].VerticalAlignment = VerticalAlignment.Center; // Align title to the center of the cell

            // Add extra space after the header for clarity
            var spacerParagraph = section.AddParagraph();
            spacerParagraph.Format.SpaceAfter = 80; // Adds extra space below the header to separate it from the details
        }


        private void AddCustomerCarAndQuotationDetails(Section section, QuotationDetailsDto quotation)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;

            table.AddColumn(Unit.FromCentimeter(10));
            table.AddColumn(Unit.FromCentimeter(8));

            var row = table.AddRow();

            var leftCell = row.Cells[0];
            leftCell.VerticalAlignment = VerticalAlignment.Top;

            var customerParagraph = leftCell.AddParagraph();
            customerParagraph.AddFormattedText("Customer Name: ", TextFormat.Bold);
            customerParagraph.AddText(quotation.CustomerName + "\n");

            customerParagraph.AddFormattedText("Email Address: ", TextFormat.Bold);
            customerParagraph.AddText(quotation.CustomerEmail + "\n");

            customerParagraph.AddFormattedText("Contact #: ", TextFormat.Bold);
            customerParagraph.AddText(quotation.CustomerNumber + "\n");

            customerParagraph.Format.SpaceAfter = 10;

            var carParagraph = leftCell.AddParagraph();
            carParagraph.AddFormattedText("Car Rego: ", TextFormat.Bold);
            carParagraph.AddText(quotation.CarRego + "\n");

            carParagraph.AddFormattedText("Car Model: ", TextFormat.Bold);
            carParagraph.AddText(quotation.CarModel + "\n");

            carParagraph.AddFormattedText("Car Year: ", TextFormat.Bold);
            carParagraph.AddText(quotation.CarYear.HasValue ? quotation.CarYear.Value.ToString() : "N/A");

            carParagraph.Format.SpaceAfter = 10;

            var rightCell = row.Cells[1];
            rightCell.VerticalAlignment = VerticalAlignment.Top;
            rightCell.Format.Alignment = ParagraphAlignment.Right;

            var quotationParagraph = rightCell.AddParagraph();
            quotationParagraph.AddFormattedText("Quotation #: ", TextFormat.Bold);
            quotationParagraph.AddText(quotation.QuotationId.ToString() + "\n");

            quotationParagraph.AddFormattedText("Date Created: ", TextFormat.Bold);
            quotationParagraph.AddText(quotation.DateAdded.ToString("dd/MM/yyyy") + "\n");


            quotationParagraph.Format.SpaceAfter = 10;
        }

        private void AddCompanyDetails(Section section)
        {
            var paragraph = section.AddParagraph("Mobile Mekaniko");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Bold = true;

            paragraph = section.AddParagraph("\"71 Glendale Rd Glen Eden, Auckland\"");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph = section.AddParagraph("\"027 266 6146\"");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph = section.AddParagraph("\"06-0878-0905123-00\"");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph.Format.SpaceAfter = 10;
        }

        private void AddQuotationItemsTable(Section section, QuotationDetailsDto quotation)
        {
            var table = section.AddTable();
            table.Borders.Width = 0.5;
            table.Borders.Color = Colors.Black;
            table.AddColumn(Unit.FromCentimeter(9));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));

            AddTableHeader(table, "Item Name", "Quantity", "Price", "Total");

            foreach (var item in quotation.QuotationItemDto)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(item.ItemName);
                row.Cells[1].AddParagraph(item.Quantity.ToString());
                row.Cells[2].AddParagraph(item.ItemPrice.ToString("C"));
                row.Cells[3].AddParagraph(item.ItemTotal?.ToString("C"));
            }

            section.AddParagraph().Format.SpaceAfter = 20;
        }

        private void AddQuotationTotal(Section section, QuotationDetailsDto quotation)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;

            table.AddColumn(Unit.FromCentimeter(10));

            AddTableRowWithLabelAndValue(table, "Subtotal: ", quotation.SubTotal?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Labor Price: ", quotation.LaborPrice?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Discount: ", quotation.Discount?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Shipping Fee: ", quotation.ShippingFee?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "GST: ", quotation.TaxAmount?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Total Amount: ", quotation.TotalAmount?.ToString("C") ?? "N/A");

            section.AddParagraph().Format.SpaceAfter = 20;
        }

        private void AddNotes(Section section, QuotationDetailsDto quotation)
        {
            var paragraph = section.AddParagraph("Notes:");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 5;

            // Optionally handle the case where there are no notes
            if (string.IsNullOrEmpty(quotation.Notes))
            {
                section.AddParagraph("No additional notes provided.");
            }
            else
            {
                section.AddParagraph(quotation.Notes);
            }

            // Add a final paragraph stating the validity of the quotation
            var validityParagraph = section.AddParagraph("This quotation is valid for 30 days from the date of issue.");
            validityParagraph.Format.Font.Bold = true;
            validityParagraph.Format.SpaceBefore = 60; // Add space before the final message
        }


        private void AddTableRowWithLabelAndValue(Table table, string label, string value)
        {
            var row = table.AddRow();

            var paragraph = row.Cells[0].AddParagraph();

            paragraph.AddFormattedText(label, TextFormat.Bold);

            paragraph.AddTab();
            paragraph.AddText(value);

            paragraph.Format.TabStops.ClearAll();
            paragraph.Format.TabStops.AddTabStop(Unit.FromCentimeter(5.5));

            paragraph.Format.Alignment = ParagraphAlignment.Left;

            row.TopPadding = Unit.FromCentimeter(0);
            row.BottomPadding = Unit.FromCentimeter(0);
        }

        private void AddTableHeader(Table table, params string[] headers)
        {
            var row = table.AddRow();
            row.Shading.Color = Colors.LightGray;
            for (int i = 0; i < headers.Length; i++)
            {
                row.Cells[i].AddParagraph(headers[i]).Format.Font.Bold = true;
            }
        }
    }
}