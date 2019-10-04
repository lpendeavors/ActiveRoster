using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveRoster.Models;

namespace ActiveRoster.Helpers
{
  public class DataHelper
  {
    public static int SaveTeam(string teamName)
    {
      tbl_Team team = new tbl_Team()
      {
        Name = teamName
      };

      using (var context = new ActiveRosterDBEntities())
      {
        context.tbl_Team.Add(team);
        context.SaveChanges();
      }

      return team.Id;
    }

    public static bool TeamExist(string teamName)
    {
      using (var context = new ActiveRosterDBEntities())
      {
        var team = context.tbl_Team.Where(t => t.Name == teamName).FirstOrDefault();
        if (null == team)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    public static List<tbl_Team> GetTeams()
    {
      using (var context = new ActiveRosterDBEntities())
      {
        return context.tbl_Team.ToList();
      }
    }

    public static int GetTeamId(string teamName)
    {
      using (var context = new ActiveRosterDBEntities())
      {
        var team = context.tbl_Team.Where(t => t.Name == teamName).FirstOrDefault();
        if (null == team)
        {
          return -1;
        }
        else
        {
          return team.Id;
        }
      }
    }

    public static void SavePlayer(string registrationNumber, string first, string last, string weight, string dateOfBirth, string level, int team)
    {
      decimal convertedWeight = 0.0m;
      try
      {
        convertedWeight = Convert.ToDecimal(weight);
      }
      catch(Exception ex)
      {
      }

      tbl_Player player = new tbl_Player()
      {
        FirstName = first,
        LastName = last,
        Weight = convertedWeight,
        RegistrationNumber = registrationNumber,
        DateOfBirth = DateTime.ParseExact(dateOfBirth, "M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture),
        Team = team
      };

      using (var context = new ActiveRosterDBEntities())
      {
        context.tbl_Player.Add(player);
        context.SaveChanges();
      }
    }

    public static void SaveImage(string playerNumber, Image image)
    {
      using (var context = new ActiveRosterDBEntities())
      {
        tbl_Player player = context.tbl_Player.Where(p => p.RegistrationNumber.Contains(playerNumber)).FirstOrDefault();
        if (player != null)
        {
          player.Image = ImageHelper.ImageToByteArray(image);
          context.SaveChanges();
        }
      }
    }

    public static tbl_Player GetPlayerById(int playerId)
    {
      using (var context = new ActiveRosterDBEntities())
      {
        return context.tbl_Player.Where(p => p.Id == playerId).FirstOrDefault();
      }
    }

    public static List<tbl_Player> GetPlayersByTeamId(int teamId)
    {
      using (var context = new ActiveRosterDBEntities())
      {
        return context.tbl_Player.Where(p => p.Team == teamId).ToList();
      }
    }
  }
}
