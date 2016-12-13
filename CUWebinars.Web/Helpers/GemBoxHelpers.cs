using System;
using System.Linq;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.ViewModel;
using GemBox.Document;
using GemBox.Document.Tables;

namespace CUWebinars.Web.Helpers
{
    public static class GemBoxHelpers
    {

        public static DocumentModel BuildPromoForms(WebinarPromoViewModel model, string bodyLeft,
            string bodyRight)
        {
            double bodyWidth;
            double sideBarWidth;
            DocumentModel document = new DocumentModel();

            Table tbl = new Table(document);
            Table tblRight = new Table(document);
            Table tblForm = new Table(document);
            Table tblRegTypes = new Table(document);
            Table tblHowSign = new Table(document);
            document.Sections.Add(new GemBox.Document.Section(document, tbl));

            PageSetup pageSetup = document.Sections[0].PageSetup;
            pageSetup.PageMargins.Top = 10;
            pageSetup.PageWidth = 612;
            pageSetup.PageMargins.Bottom = 10;
            pageSetup.PageMargins.Left = 35;
            pageSetup.PageMargins.Right = 35;
            pageSetup.Orientation = Orientation.Portrait;


            bodyWidth = 50;
            sideBarWidth = 175; 

            tbl.TableFormat.AutomaticallyResizeToFitContents = true;
            tbl.TableFormat.Alignment = HorizontalAlignment.Center;
            tbl.TableFormat = new TableFormat() {PreferredWidth = new TableWidth(100, TableWidthUnit.Percentage),Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } } };

            tblForm.TableFormat = new TableFormat() { PreferredWidth = new TableWidth(100, TableWidthUnit.Percentage), Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } } };
            tblRight.TableFormat = new TableFormat() { PreferredWidth = new TableWidth(sideBarWidth, TableWidthUnit.Point), Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } } };
            tblRegTypes.TableFormat = new TableFormat() { Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } } };
            tblHowSign.TableFormat = new TableFormat() { Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } } };

            tbl.Columns.Add(new TableColumn() { PreferredWidth = bodyWidth });
            tbl.Columns.Add(new TableColumn() { PreferredWidth = sideBarWidth });
            tblRight.Columns.Add(new TableColumn());
            tblForm.Columns.Add(new TableColumn());
            tblRegTypes.Columns.Add(new TableColumn() { PreferredWidth = sideBarWidth });
            tblRegTypes.Columns.Add(new TableColumn() { PreferredWidth = sideBarWidth });
            tblHowSign.Columns.Add(new TableColumn() { PreferredWidth = sideBarWidth });

            TableRow bodyRow = new TableRow(document);
            TableRow footerRow = new TableRow(document);
            TableRow nameRow = new TableRow(document);
            TableRow bankRow = new TableRow(document);
            TableRow addressRow = new TableRow(document);
            TableRow cityRow = new TableRow(document);
            TableRow phoneRow = new TableRow(document);
            TableRow emailRow = new TableRow(document);
            TableRow regLeftRow = new TableRow(document);
            TableRow regRightRow = new TableRow(document);
            TableRow tblRightRow = new TableRow(document);
            nameRow.RowFormat.Height = new TableRowHeight(12, TableRowHeightRule.Exact);
            bankRow.RowFormat.Height = new TableRowHeight(12, TableRowHeightRule.Exact);
            addressRow.RowFormat.Height = new TableRowHeight(12, TableRowHeightRule.Exact);
            cityRow.RowFormat.Height = new TableRowHeight(12, TableRowHeightRule.Exact);
            phoneRow.RowFormat.Height = new TableRowHeight(12, TableRowHeightRule.Exact);
            emailRow.RowFormat.Height = new TableRowHeight(12, TableRowHeightRule.Exact);

            tbl.Rows.Add(bodyRow);
            tbl.Rows.Add(footerRow);
            tblRight.Rows.Add(tblRightRow);
            tblForm.Rows.Add(nameRow);
            tblForm.Rows.Add(bankRow);
            tblForm.Rows.Add(addressRow);
            tblForm.Rows.Add(cityRow);
            tblForm.Rows.Add(phoneRow);
            tblForm.Rows.Add(emailRow);
            tblRegTypes.Rows.Add(regLeftRow);
            tblRegTypes.Rows.Add(regRightRow);


            nameRow.Cells.Add(new TableCell(document, new Paragraph(document, "Name") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 12 } }) { CellFormat = new TableCellFormat() { Borders = { { MultipleBorderTypes.Bottom, BorderStyle.Dotted, Color.DarkGray, 1 } } } });
            bankRow.Cells.Add(new TableCell(document, new Paragraph(document, "Bank") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 12 } }) { CellFormat = new TableCellFormat() { Borders = { { MultipleBorderTypes.Bottom, BorderStyle.Dotted, Color.DarkGray, 1 } } } });
            addressRow.Cells.Add(new TableCell(document, new Paragraph(document, "Address") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 12 } }) { CellFormat = new TableCellFormat() { Borders = { { MultipleBorderTypes.Bottom, BorderStyle.Dotted, Color.DarkGray, 1 } } } });
            cityRow.Cells.Add(new TableCell(document, new Paragraph(document, "City, St, Zip") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 12 } }) { CellFormat = new TableCellFormat() { Borders = { { MultipleBorderTypes.Bottom, BorderStyle.Dotted, Color.DarkGray, 1 } } } });
            phoneRow.Cells.Add(new TableCell(document, new Paragraph(document, "Phone") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 12 } }) { CellFormat = new TableCellFormat() { Borders = { { MultipleBorderTypes.Bottom, BorderStyle.Dotted, Color.DarkGray, 1 } } } });
            emailRow.Cells.Add(new TableCell(document, new Paragraph(document, "Email (required)") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 12 } }) { CellFormat = new TableCellFormat() { Borders = { { MultipleBorderTypes.Bottom, BorderStyle.Dotted, Color.DarkGray, 1 } } } });

            //footer
            footerRow.Cells.Add(new TableCell(document, new Paragraph(document, "footer") { ParagraphFormat = new ParagraphFormat() { Alignment = HorizontalAlignment.Center } })
            {
                CellFormat = new TableCellFormat() { VerticalAlignment = VerticalAlignment.Center, BackgroundColor = new Color(76, 89, 102) },
                ColumnSpan = 2
            });

            bodyRow.Cells.Add(new TableCell(document, new Paragraph(document, "bodyLeft") { ParagraphFormat = new ParagraphFormat() { LineSpacingRule = LineSpacingRule.Exactly, LineSpacing = 11 } }));

            bodyRow.Cells.Add(new TableCell(document, tblRight));
            tblRightRow.Cells.Add(new TableCell(document, tblForm));


            foreach (ContentRange item in document.Content.Find("logoBanner").Reverse())
                item.LoadText(model.Affiliate.EmailBanner, new HtmlLoadOptions());
            foreach (ContentRange item in document.Content.Find("bodyLeft").Reverse())
                item.LoadText(bodyLeft, new HtmlLoadOptions());
            foreach (ContentRange item in document.Content.Find("bodyRight").Reverse())
                item.LoadText(bodyRight, new HtmlLoadOptions());
            foreach (ContentRange item in document.Content.Find("footer").Reverse())
                item.LoadText(model.Affiliate.EmailFooter, new HtmlLoadOptions());
            return document;
        }
        public static DocumentModel BuildPromoReplica(WebinarPromoViewModel model)
        {
            Size size = DomainHelpers.ResizePhoto(model.Webinar.Presenter.PhotoFull);

            double bodyWidth;
            double sideBarWidth;
            DocumentModel document = new DocumentModel();

            Table table = new Table(document);
            Table tableInset = new Table(document);
            document.Sections.Add(new GemBox.Document.Section(document, table));
            document.Sections[0].PageSetup.PaperType = PaperType.Letter;
            document.Sections[0].PageSetup.PageWidth = 640;

            double width = document.Sections[0].PageSetup.PageWidth;
            double height = document.Sections[0].PageSetup.PageHeight;
            PageSetup pageSetup = document.Sections[0].PageSetup;
            pageSetup.PageMargins.Top = 10;
            pageSetup.PageMargins.Bottom = 10;
            pageSetup.PageMargins.Left = 35;
            pageSetup.PageMargins.Right = 35;
            pageSetup.Orientation = Orientation.Portrait;

            bodyWidth = 380; // width / (66 * 100);
            sideBarWidth = 140; // width / (33 * 100);

            table.TableFormat.AutomaticallyResizeToFitContents = true;
            table.TableFormat.Alignment = HorizontalAlignment.Center;

            table.Columns.Add(new TableColumn() { PreferredWidth = bodyWidth });
            table.Columns.Add(new TableColumn() { PreferredWidth = sideBarWidth });
            tableInset.Columns.Add(new TableColumn());
            tableInset.Columns.Add(new TableColumn());
            table.TableFormat = new TableFormat()
            {
                Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } }
            };
            tableInset.TableFormat = new TableFormat()
            {
                Borders = { { MultipleBorderTypes.All, BorderStyle.None, Color.Black, 0 } }
            };
            ;

            //TableRow preheadRow = new TableRow(document);
            TableRow logoRow = new TableRow(document);
            TableRow bodyRow = new TableRow(document);
            TableRow footerRow = new TableRow(document);
            TableRow insetRowTitle = new TableRow(document);
            TableRow insetRowPresenter = new TableRow(document);
            TableRow insetRowBody = new TableRow(document);


            insetRowTitle.Cells.Add(new TableCell(document,
                new Paragraph(document, model.Webinar.Title)
                { ParagraphFormat = { Style = (ParagraphStyle)document.Styles.GetOrAdd(StyleTemplateType.Title) } }
            )
            {
                CellFormat = new TableCellFormat() { VerticalAlignment = VerticalAlignment.Center },
                ColumnSpan = 2
            });

            insetRowPresenter.Cells.Add(new TableCell(document, new Paragraph(document,
                    new Picture(document, model.Webinar.Presenter.PhotoFull, size.Width, size.Height))
            { ParagraphFormat = { Alignment = HorizontalAlignment.Right } }));

            insetRowPresenter.Cells.Add(new TableCell(document, new Paragraph(document, "timeBlock")));

            insetRowBody.Cells.Add(new TableCell(document,
                new Paragraph(document, "model.Webinar.Description"))
            {
                ColumnSpan = 2
            });

            tableInset.Rows.Add(insetRowTitle);
            tableInset.Rows.Add(insetRowPresenter);
            tableInset.Rows.Add(insetRowBody);
            //affLogo
            logoRow.Cells.Add(new TableCell(document, new Paragraph(document, new Picture(document, model.Affiliate.Logo)))
            {
                CellFormat = new TableCellFormat() { VerticalAlignment = VerticalAlignment.Center, BackgroundColor = new Color(76, 89, 102) }
            });

            //calendar
            logoRow.Cells.Add(new TableCell(document, new Paragraph(document, new Picture(document, "https://www.bankwebinars.com/content/images/email/calendar.jpg")) { ParagraphFormat = { Alignment = HorizontalAlignment.Right } })
            {
                CellFormat = new TableCellFormat() { VerticalAlignment = VerticalAlignment.Center, BackgroundColor = new Color(76, 89, 102) }
            });

            footerRow.Cells.Add(new TableCell(document, new Paragraph(document, "footer")
            {
                ParagraphFormat = new ParagraphFormat()
                {
                    Alignment = HorizontalAlignment.Center
                }
            })
            {
                CellFormat = new TableCellFormat() { VerticalAlignment = VerticalAlignment.Center, BackgroundColor = new Color(76, 89, 102) },
                ColumnSpan = 2
            });


            bodyRow.Cells.Add(new TableCell(document, tableInset));

            bodyRow.Cells.Add(new TableCell(document, new Paragraph(document, "bodyRight"))
            {
                CellFormat = new TableCellFormat() { BackgroundColor = new Color(35, 43, 46), Padding = new Padding(5, 5, LengthUnit.Pixel) },
            });

            table.Rows.Add(logoRow);
            table.Rows.Add(bodyRow);
            table.Rows.Add(footerRow);

            foreach (ContentRange item in document.Content.Find("model.Webinar.Description").Reverse())
                item.LoadText(model.Webinar.Description, new HtmlLoadOptions());
            foreach (ContentRange item in document.Content.Find("bodyRight").Reverse())
                item.LoadText(model.BodyRight, new HtmlLoadOptions());
            foreach (ContentRange item in document.Content.Find("footer").Reverse())
                item.LoadText(model.Affiliate.EmailFooter, new HtmlLoadOptions());
            foreach (ContentRange item in document.Content.Find("timeBlock").Reverse())
                item.LoadText("<div align=\"center\">" + model.TimeFormatDisplay + "<br>Presented by " + model.Webinar.Presenter.WebUser.FullName +
                    "<br><br> *Can't attend? OnDemand playback included with your registration.</span></div>", new HtmlLoadOptions());
            return document;
        }

    }
}
