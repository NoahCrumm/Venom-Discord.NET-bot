using System;
using System.Collections.Generic;
using System.Linq;

namespace VenomBot.Services
{
    public class Table
    {
        private List<Col> Columns = new List<Col>();
        private List<List<string>> Rows = new List<List<string>>();

        public Table(params Col[] args)
        {
            this.Columns = new List<Col>(args);
        }

        public Table(params string[] headers)
        {
            headers.ForEach(h => { this.Columns.Add(new Col(h)); });
            this.Rows.Add(new List<string>(headers));
        }

        public void AddRow(params string[] cells)
        {
            if (cells.Length != this.Columns.Count)
                throw new ArgumentException($"Invalid number of columns. Expected {this.Columns.Count} recieved {cells.Length}.", "AddRow");

            this.Rows.Add(new List<string>(cells));
        }

        /**
         * Retrieve this table as a fully rendered string
         */
        public string AsString()
        {
            return "```prolog\n" + String.Join("\n", this.AsList()) + "\n```";
        }

        /**
         * Retrieve this table as string chunks no larger than 2000 characters in size so that they can be transmitted to Discord.
         */
        public List<string> AsStringChunks(string preHeader = "")
        {
            string header = "";
            int rowCounter = 0;
            int charCounter = 14;
            List<string> tableContents = new List<string>();
            string buffer = "";

            if (!string.IsNullOrEmpty(preHeader))
                tableContents.Add(preHeader);

            foreach (string row in this.AsList())
            {
                if (rowCounter == 0)
                {
                    // Record this as the data header; it'll be repeated across different segments.

                    header = row;
                    buffer = row;
                }
                else if (rowCounter == 1)
                {
                    // I need the first row *AND* the second row; which contains the dashes in the header.
                    header += "\n" + row;
                    buffer += "\n" + row;
                }
                else
                {
                    // Perform checks on the current buffer size and other dealings with the buffer.
                    if ((charCounter + row.Length + 1) > 2000)
                    {
                        // Maximum buffer size exceeded!  Dump the contents to the table and clear the buffer for the next chunk of rows.
                        tableContents.Add("```prolog\n" + buffer + "\n```");
                        buffer = header;
                        charCounter = header.Length + 14; // 8 is the size of the manual header and footer.
                    }

                    buffer += "\n" + row;
                }

                charCounter += row.Length + 1;
                rowCounter++;
            }

            // Whatever's left in the buffer, dump that out to the List too!
            tableContents.Add("```prolog\n" + buffer + "\n```");

            return tableContents;
        }

        /**
         * Retrieve this table as a List of fully rendered rows
         */
        public List<string> AsList()
        {
            string rowContents;
            List<int> columnWidths = new List<int>();
            int cellCounter = 0;
            int rowCounter = 0;
            List<string> tableContents = new List<string>();


            // First, iterate through the headers and all rows to calculate the maximum width of each column.
            foreach (Col c in this.Columns)
            {
                columnWidths.Add(c.Header.Length);
                cellCounter++;
            }
            foreach (List<string> l in this.Rows)
            {
                cellCounter = 0;
                foreach (string s in l)
                {
                    columnWidths[cellCounter] = Math.Max(s.Length, columnWidths[cellCounter]);
                    cellCounter++;
                }
            }


            // Now that we have simple sizes calculated, we can draw the actual table!
            cellCounter = 0;
            rowContents = "";
            foreach (Col c in this.Columns)
            {
                rowContents += "| " + c.Header.PadRight(columnWidths[cellCounter]) + " ";
                cellCounter++;
            }
            // End the header record and append to the list
            rowContents += "|";
            tableContents.Add(rowContents);


            foreach (List<string> l in this.Rows)
            {
                rowContents = "";
                cellCounter = 0;

                foreach (string s in l)
                {
                    if (rowCounter == 0)
                    {
                        // This is the header; we'll use this opportunity to draw the dashes seperator.
                        rowContents += "|-" + "-".PadRight(columnWidths[cellCounter], '-') + "-";
                    }
                    else
                    {
                        rowContents += "| " + s.PadRight(columnWidths[cellCounter]) + " ";
                    }
                    cellCounter++;
                }

                // End the data record.
                rowContents += "|";
                tableContents.Add(rowContents);
                rowCounter++;
            }

            // That's all she wrote folks!
            return tableContents;
        }

        public override string ToString()
        {
            var tableContents = "";

            //Create Rows
            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var columns = Rows[rowIndex];
                var rowContents = "";
                var divider = "";
                //Columns
                for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                {
                    //the cell int he row
                    var cell = columns[colIndex] ?? "";

                    //set Column width automatially if its not defined;
                    int w = this.Columns[colIndex].Width;
                    if (w == 0)
                    {
                        w = this.Columns[colIndex].Width = Rows.Max(r => r[colIndex]?.Length ?? 0);
                    }

                    if (Rows.IsFirst(rowIndex))
                    {
                        //Create Header
                        rowContents += $"| {cell.PadRight(w)} ";

                        //Create Divider
                        divider += $"| {("").PadRight(w, '-')} ";

                        //Add to table contents
                        if (columns.IsLast(colIndex))
                        {
                            rowContents += "| \n";
                            divider += "| \n";

                            tableContents += rowContents;
                            tableContents += divider;
                        }
                    }
                    else
                    {
                        //Create Body
                        rowContents += $"| {cell.PadRight(w)} ";

                        //Add to table contents
                        if (columns.IsLast(colIndex))
                        {
                            rowContents += "| \n";
                            tableContents += rowContents;
                        }
                    }

                }
            }
            return "```" + tableContents + "```";
        }

        public string ToStringNoFormat()
        {
            var tableContents = "";

            //Create Rows
            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var columns = Rows[rowIndex];
                var rowContents = "";
                var divider = "";
                //Columns
                for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                {
                    //the cell int he row
                    var cell = columns[colIndex] ?? "";

                    //set Column width automatially if its not defined;
                    int w = this.Columns[colIndex].Width;
                    if (w == 0)
                    {
                        w = this.Columns[colIndex].Width = Rows.Max(r => r[colIndex]?.Length ?? 0);
                    }

                    if (Rows.IsFirst(colIndex))
                    {
                        //Create Header
                        rowContents += $"| {cell.PadRight(w)} ";

                        //Create Divider
                        divider += $"| {("").PadRight(w, '-')} ";

                        //Add to table contents
                        if (columns.IsLast(colIndex))
                        {
                            rowContents += "| \n";
                            divider += "| \n";

                            tableContents += rowContents;
                            tableContents += divider;
                        }
                    }
                    else
                    {
                        //Create Body
                        rowContents += $"| {cell.PadRight(w)} ";

                        //Add to table contents
                        if (columns.IsLast(colIndex))
                        {
                            rowContents += "| \n";
                            tableContents += rowContents;
                        }
                    }

                }
            }
            return tableContents;
        }
    }
}

public class Col
{
    public string Header { get; set; }
    public int Width { get; set; }
    public Col(string header, int width = 0)
    {
        this.Header = header;
        this.Width = width;
    }
}
