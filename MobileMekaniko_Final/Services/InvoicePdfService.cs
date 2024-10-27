using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MobileMekaniko_Final.Models.Dto;
using System.IO;
using MigraDoc.DocumentObjectModel.Tables;

namespace MobileMekaniko_Final.Services
{
    public class InvoicePdfService : IInvoicePdfService
    {
        public byte[] CreateInvoicePdf(InvoiceDetailsDto invoice)
        {
            var document = new Document();
            var section = document.AddSection();

            // Set page margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);

            AddHeaderWithLogo(section);
            AddCustomerCarAndInvoiceDetails(section, invoice);
            AddCompanyDetails(section);
            AddInvoiceItemsTable(section, invoice);
            AddInvoiceTotal(section, invoice);
            AddNotes(section, invoice);

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
            headerTable.AddColumn(Unit.FromCentimeter(4));  // Spacer column to adjust centering of title
            headerTable.AddColumn(Unit.FromCentimeter(9));  // Column for the title

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
            var header = row.Cells[2].AddParagraph("Mobile Mekaniko");
            header.Format.Font.Size = 20;
            header.Format.Font.Bold = true;
            header.Format.Alignment = ParagraphAlignment.Left; // Align title within its own column to the left
            row.Cells[2].VerticalAlignment = VerticalAlignment.Center; // Align title to the center of the cell

            // Add extra space after the header for clarity
            var spacerParagraph = section.AddParagraph();
            spacerParagraph.Format.SpaceAfter = 80; // Adds extra space below the header to separate it from the details
        }


        private void AddCustomerCarAndInvoiceDetails(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;

            // Define columns for the table
            table.AddColumn(Unit.FromCentimeter(10)); // Wider column for customer and car details
            table.AddColumn(Unit.FromCentimeter(8));  // Narrower column for invoice details

            // Create a row for Customer and Invoice Details
            var row = table.AddRow();

            // Add customer and car details in the first cell
            var leftCell = row.Cells[0];
            leftCell.VerticalAlignment = VerticalAlignment.Top;

            var customerParagraph = leftCell.AddParagraph();
            customerParagraph.AddFormattedText("Customer Name: ", TextFormat.Bold);
            customerParagraph.AddText(invoice.CustomerName + "\n");

            customerParagraph.AddFormattedText("Email Address: ", TextFormat.Bold);
            customerParagraph.AddText(invoice.CustomerEmail + "\n");

            customerParagraph.AddFormattedText("Contact #: ", TextFormat.Bold);
            customerParagraph.AddText(invoice.CustomerNumber + "\n");

            customerParagraph.AddFormattedText("Address: ", TextFormat.Bold);
            customerParagraph.AddText(invoice.CustomerAddress + "\n");

            customerParagraph.Format.SpaceAfter = 10;

            // Add car details in the same left cell
            var carParagraph = leftCell.AddParagraph();
            carParagraph.AddFormattedText("Car Rego: ", TextFormat.Bold);
            carParagraph.AddText(invoice.CarRego + "\n");

            carParagraph.AddFormattedText("Car Model: ", TextFormat.Bold);
            carParagraph.AddText(invoice.CarModel + "\n");

            carParagraph.AddFormattedText("Car Year: ", TextFormat.Bold);
            carParagraph.AddText(invoice.CarYear.HasValue ? invoice.CarYear.Value.ToString() : "N/A");

            carParagraph.Format.SpaceAfter = 10;

            // Add invoice details in the second cell (right side)
            var rightCell = row.Cells[1];
            rightCell.VerticalAlignment = VerticalAlignment.Top;
            rightCell.Format.Alignment = ParagraphAlignment.Right;

            var taxInvoice = rightCell.AddParagraph();
            taxInvoice.AddFormattedText("TAX INVOICE", TextFormat.Bold);

            var invoiceParagraph = rightCell.AddParagraph();
            invoiceParagraph.AddFormattedText("Invoice #: ", TextFormat.Bold);
            invoiceParagraph.AddText(invoice.InvoiceId.ToString() + "\n");

            invoiceParagraph.AddFormattedText("Date Issued: ", TextFormat.Bold);
            invoiceParagraph.AddText(invoice.DateAdded.HasValue ? invoice.DateAdded.Value.ToString("dd/MM/yyyy") : "N/A" + "\n");

            // Add "Due Date" on a separate line below "Date Issued"
            var dueDateParagraph = rightCell.AddParagraph();
            dueDateParagraph.AddFormattedText("Due Date: ", TextFormat.Bold);
            dueDateParagraph.AddText(invoice.DueDate.HasValue ? invoice.DueDate.Value.ToString("dd/MM/yyyy") : "N/A");

            // Add "Due Date" on a separate line below "Date Issued"
            var taxParagraph = rightCell.AddParagraph();
            taxParagraph.AddFormattedText("GST No: ", TextFormat.Bold);
            taxParagraph.AddText("130-121-942" + "\n");

            taxParagraph.Format.SpaceAfter = 10; // Add space after Due Date
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

        private void AddInvoiceItemsTable(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Width = 0.5;
            table.Borders.Color = Colors.Black;
            table.AddColumn(Unit.FromCentimeter(9));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));

            AddTableHeader(table, "Item Name", "Quantity", "Price", "Total");

            foreach (var item in invoice.InvoiceItemDto)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(item.ItemName);
                row.Cells[1].AddParagraph(item.Quantity.ToString());
                row.Cells[2].AddParagraph(item.ItemPrice.ToString("C"));
                row.Cells[3].AddParagraph(item.ItemTotal.ToString("C"));
            }

            // Add space below the item table without adding a bordered row
            section.AddParagraph().Format.SpaceAfter = 20; // Increase space for clarity
        }

        private void AddInvoiceTotal(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;

            // Add a single column for labels and values together
            table.AddColumn(Unit.FromCentimeter(10));

            // Add each row as a label followed by the value to keep them together closely
            AddTableRowWithLabelAndValue(table, "Subtotal: ", invoice.SubTotal?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Labor Price: ", invoice.LabourPrice?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Discount: ", invoice.Discount?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Shipping Fee: ", invoice.ShippingFee?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "GST: ", invoice.TaxAmount?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Total Amount: ", invoice.TotalAmount?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Amount Paid: ", invoice.AmountPaid?.ToString("C") ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Payment Term: ", invoice.PaymentTerm ?? "N/A");
            AddTableRowWithLabelAndValue(table, "Payment Status: ", invoice.isPaid == true ? "Paid" : "Not Paid");

            // Add space after invoice totals before Notes section
            section.AddParagraph().Format.SpaceAfter = 20;
        }

        private void AddNotes(Section section, InvoiceDetailsDto invoice)
        {
            var paragraph = section.AddParagraph("Notes:");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 5;

            section.AddParagraph(invoice.Notes);
        }

        private void AddTableRowWithLabelAndValue(Table table, string label, string value)
        {
            var row = table.AddRow();

            // Create a paragraph combining the label and value in the same cell
            var paragraph = row.Cells[0].AddParagraph();

            // Add formatted label
            paragraph.AddFormattedText(label, TextFormat.Bold);

            // Use a single tab stop for consistent positioning of the value
            paragraph.AddTab();  // Adding a tab to separate label and value
            paragraph.AddText(value);

            // Set a closer tab stop to align all values properly, making them closer to the labels
            paragraph.Format.TabStops.ClearAll();
            paragraph.Format.TabStops.AddTabStop(Unit.FromCentimeter(5.5));  // Adjusted to bring the value closer

            // Align the paragraph to the left for both label and value
            paragraph.Format.Alignment = ParagraphAlignment.Left;

            // Reduce the padding around the text for compactness
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
