using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ActiveRoster.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ActiveRoster.Helpers
{
  public class PDFHelper
  {
    public static void GeneratePDFs(string folder)
    {
      List<tbl_Team> teams = DataHelper.GetTeams();
      foreach (tbl_Team team in teams)
      {
        string savePath = String.Format("{0}\\{1}.pdf", folder, team.Name);

        Document document = new Document(PageSize.A4.Rotate());

        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(savePath, FileMode.Create));
        document.Open();

        Paragraph title = new Paragraph(team.Name);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        PdfPTable table = new PdfPTable(8);

        List<tbl_Player> players = DataHelper.GetPlayersByTeamId(team.Id);

        int colCount = 0;

        for (int i = 0; i < players.Count; i++)
        {
          if (i == 0)
          {
            Paragraph levelTitle = new Paragraph("Flag");
            levelTitle.Alignment = Element.ALIGN_CENTER;
            document.Add(levelTitle);
          }

          var playerLevel = players[i].RegistrationNumber.Split('-')[1].ToString();

          if (i > 0)
          {
            var lastLevel = players[i - 1].RegistrationNumber.Split('-')[1].ToString();

            if (playerLevel != lastLevel)
            {
              Paragraph levelTitle;

              switch (playerLevel)
              {
                case "FRS":
                  levelTitle = new Paragraph("Freshman");
                  break;
                case "JRV":
                  levelTitle = new Paragraph("Junior Varsity");
                  break;
                case "VAR":
                  levelTitle = new Paragraph("Varsity");
                  break;
                case "CHEER":
                  levelTitle = new Paragraph("Cheer");
                  break;
                default:
                  levelTitle = new Paragraph("Staff");
                  break;
              }

              levelTitle.Alignment = Element.ALIGN_CENTER;

              var columnsNeeded = 8 - colCount;

              while (columnsNeeded > 0)
              {
                var emptyCell = new PdfPCell();
                emptyCell.Border = 0;
                table.AddCell(emptyCell);
                columnsNeeded--;
                colCount++;
              }

              if (colCount == 8)
              {
                colCount = 0;
              }

              document.Add(table);

              document.NewPage();
              document.Add(levelTitle);

              table = new PdfPTable(8);
            }
          }

          PdfPTable playerTable = new PdfPTable(1);

          if (players[i].Image != null)
          {
            Image playerImage = Image.GetInstance(players[i].Image);
            PdfPCell imageCell = new PdfPCell(playerImage, true);
            imageCell.Border = 0;
            playerTable.AddCell(imageCell);
          }

          Paragraph regNum = new Paragraph(players[i].RegistrationNumber, new Font(Font.FontFamily.HELVETICA, 8));
          PdfPCell regNumCell = new PdfPCell(regNum);
          regNumCell.Border = 0;
          playerTable.AddCell(regNumCell);

          Paragraph name = new Paragraph(players[i].FirstName + " " + players[i].LastName, new Font(Font.FontFamily.HELVETICA, 8));
          PdfPCell nameCell = new PdfPCell(name);
          nameCell.Border = 0;
          playerTable.AddCell(nameCell);

          Paragraph dob = new Paragraph(players[i].DateOfBirth.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 8));
          PdfPCell dobCell = new PdfPCell(dob);
          dobCell.Border = 0;
          playerTable.AddCell(dobCell);

          Paragraph weight = new Paragraph(players[i].Weight.ToString(), new Font(Font.FontFamily.HELVETICA, 8));
          PdfPCell weightCell = new PdfPCell(weight);
          weightCell.Border = 0;
          playerTable.AddCell(weightCell);

          PdfPCell playerCell = new PdfPCell(playerTable);
          playerCell.Border = 0;
          playerCell.Padding = 4;

          table.AddCell(playerCell);
          colCount++;

          if (colCount == 8)
          {
            colCount = 0;
          }

          if (i == players.Count - 1)
          {
            var columnsNeeded = 8 - colCount;

            while (columnsNeeded > 0)
            {
              var emptyCell = new PdfPCell();
              emptyCell.Border = 0;
              table.AddCell(emptyCell);
              columnsNeeded--;
              colCount++;
            }
          }
        }

        document.Add(table);
        document.Close();
      }
    }
  }
}
