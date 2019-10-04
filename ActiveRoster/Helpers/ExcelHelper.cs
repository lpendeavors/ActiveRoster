using System;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace ActiveRoster.Helpers
{
  public class ExcelHelper
  {
    public static void ReadFile(string filePath)
    {
      string teamName = Path.GetFileNameWithoutExtension(filePath);

      int teamId;
      if (DataHelper.TeamExist(teamName))
      {
        teamId = DataHelper.GetTeamId(teamName);
      }
      else
      {
        teamId = DataHelper.SaveTeam(teamName);
      }

      using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
      {
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
          var workbook = reader.AsDataSet(new ExcelDataSetConfiguration()
          {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
            {
              UseHeaderRow = true
            }
          });
          DataTable teamSheet = workbook.Tables[0];

          int rowCount = teamSheet.Rows.Count;
          for (int i = 0; i < rowCount; i++)
          {
            var firstName = teamSheet.Rows[i][4].ToString();
            if (!String.IsNullOrEmpty(firstName) && "TYSA" != firstName)
            {
              var registrationNumber = String.Format(
               "{0}-{1}-{2}",
               teamSheet.Rows[i][1].ToString(),
               teamSheet.Rows[i][2].ToString(),
               teamSheet.Rows[i][3].ToString()
              );

              var lastName = teamSheet.Rows[i][5].ToString();
              var weight = teamSheet.Rows[i][10].ToString();
              var dateOfBirth = teamSheet.Rows[i][6].ToString();
              var level = teamSheet.Rows[i][11].ToString();

              DataHelper.SavePlayer(registrationNumber, firstName, lastName, weight, dateOfBirth, level, teamId);
            }
          }
        }
      }
    }
  }
}
