using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.html.simpleparser;
using System.Web;
using System.Text.RegularExpressions;

namespace SHABS.COMMON
{
    public static partial class CustomExtensions
    {
        public static void ConvertToPDF(DataTable data, string filePath)
        {
            data.ToPDF(filePath);
        }

        public static void ToPDF(this DataTable data, string filePath)
        {
            PdfPTable table = new PdfPTable(data.Columns.Count);
            Rectangle pageSize = new Rectangle(PageSize.A1.Rotate());
            Document doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();
            foreach (DataColumn column in data.Columns)
            {
                //  table.AddCell(new Phrase(column.ColumnName));
                table.AddCell(GetHeaderCell(column.ColumnName));
            }
            table.HeaderRows = 1;
            table.SpacingBefore = 0f;
            table.SpacingAfter = 0f;
            for (int cntr = 0; cntr < data.Rows.Count; cntr++)
            {
                foreach (DataColumn column in data.Columns)
                {
                    table.AddCell(new Phrase(data.Rows[cntr][column.ColumnName].ToString()));
                }

            }
            doc.Add(table);
            doc.AddAuthor("ITEKSolutions");
            // doc.AddProducer("Sumit Group Exe");
            doc.Close();
        }

        public static void ToPDF(this string data, string filePath)
        {
            Rectangle pageSize = new Rectangle(PageSize.A4);
            Document doc = new Document(pageSize);
            StringReader reader = new StringReader(data);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            HTMLWorker parser = new HTMLWorker(doc);
            doc.Open();

            parser.Parse(reader);
            doc.AddAuthor("ITEKSolutions");
            doc.Close();
        }

        private static PdfPCell GetHeaderCell(string Text)
        {

            PdfPCell cell = new PdfPCell(new Phrase(Text, new Font() { Size = 15f, Color = Color.WHITE /*new BaseColor(255,255,255)*/}));
            cell.BackgroundColor = Color.BLACK;// new BaseColor(0, 0, 0);
            return cell;
        }



        public static string getImage(string input)
        {
            if (input == null)
                return string.Empty;
            string tempInput = input;
            string pattern = @"<img(.|\n)+?>";
            string src = string.Empty;
            HttpContext context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (Match m in Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline |

            RegexOptions.RightToLeft))
            {
                if (m.Success)
                {
                    string tempM = m.Value;
                    string pattern1 = "src=[\'|\"](.+?)[\'|\"]";
                    Regex reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Match mImg = reImg.Match(m.Value);

                    if (mImg.Success)
                    {
                        src = mImg.Value.ToLower().Replace("src=", "").Replace("\"", "");

                        if (src.ToLower().Contains("http://") == false)
                        {
                            //Insert new URL in img tag
                            src = "src=\"" + context.Request.Url.Scheme + "://" +
                            context.Request.Url.Authority + src + "\"";
                            try
                            {
                                tempM = tempM.Remove(mImg.Index, mImg.Length);
                                tempM = tempM.Insert(mImg.Index, src);

                                //insert new url img tag in whole html code
                                tempInput = tempInput.Remove(m.Index, m.Length);
                                tempInput = tempInput.Insert(m.Index, tempM);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }
            return tempInput;
        }

        static string getSrc(string input)
        {
            string pattern = "src=[\'|\"](.+?)[\'|\"]";
            System.Text.RegularExpressions.Regex reImg = new System.Text.RegularExpressions.Regex(pattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase |

            System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Match mImg = reImg.Match(input);
            if (mImg.Success)
            {
                return mImg.Value.Replace("src=", "").Replace("\"", ""); ;
            }

            return string.Empty;
        }
    }


}
