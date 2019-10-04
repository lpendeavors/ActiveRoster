using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using ActiveRoster.Helpers;
using ActiveRoster.Models;
using Microsoft.Win32;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;

namespace ActiveRoster
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    List<tbl_Team> teams;
    UserControlTeam usc;

    public MainWindow()
    {
      InitializeComponent();

      usc = null;

      using (var context = new ActiveRosterDBEntities())
      {
        teams = context.tbl_Team.ToList();
        ListViewMenu.ItemsSource = teams;
      }
    }

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openfile = new Microsoft.Win32.OpenFileDialog();
      openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

      var browsefile = openfile.ShowDialog();

      if (browsefile == true)
      {
        ImportStatus.Visibility = Visibility.Visible;

        using (var bgw = new BackgroundWorker())
        {
          bgw.DoWork += (_, __) =>
          {
            ExcelHelper.ReadFile(openfile.FileName);
          };
          bgw.RunWorkerCompleted += (_, __) =>
          {
            ImportStatus.Visibility = Visibility.Collapsed;

            using (var context = new ActiveRosterDBEntities())
            {
              teams = context.tbl_Team.ToList();
              ListViewMenu.ItemsSource = teams;
            }
          };

          bgw.RunWorkerAsync();
        }
      }
    }

    private void ImageButton_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog diag = new FolderBrowserDialog();
      if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        ImportStatus.Visibility = Visibility.Visible;

        string folder = diag.SelectedPath;  //selected folder path
        var files = Directory.GetFiles(folder);

        using (var bgw = new BackgroundWorker())
        {
          bgw.DoWork += (_, __) =>
          {
            ImageHelper.ImportImages(files);
          };
          bgw.RunWorkerCompleted += (_, __) =>
          {
            ImportStatus.Visibility = Visibility.Collapsed;

            if (usc != null)
            {
              using (var context = new ActiveRosterDBEntities())
              {
                List<tbl_Player> players = context.tbl_Player.ToList();
                usc.PlayerList.ItemsSource = players;
              }
            }
          };

          bgw.RunWorkerAsync();
        }
      }
    }

    private void PDFButton_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog diag = new FolderBrowserDialog();
      if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        ImportStatus.Visibility = Visibility.Visible;

        string folder = diag.SelectedPath;

        using (var bgw = new BackgroundWorker())
        {
          bgw.DoWork += (_, __) =>
          {
            PDFHelper.GeneratePDFs(folder);
          };
          bgw.RunWorkerCompleted += (_, __) =>
          {
            ImportStatus.Visibility = Visibility.Collapsed;
          };

          bgw.RunWorkerAsync();
        }
      }
    }

    private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
    {
      ButtonCloseMenu.Visibility = Visibility.Visible;
      ButtonOpenMenu.Visibility = Visibility.Collapsed;
    }

    private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
    {
      ButtonCloseMenu.Visibility = Visibility.Collapsed;
      ButtonOpenMenu.Visibility = Visibility.Visible;
    }

    private void ListViewMenu_SelectionChanged(object sender, RoutedEventArgs e)
    {
      if (ListViewMenu.SelectedItem != null)
      {
        tbl_Team team = (tbl_Team)ListViewMenu.SelectedItem;

        usc = null;
        GridMain.Children.Clear();

        usc = new UserControlTeam(team.Id);
        GridMain.Children.Add(usc);
      }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
      using (var context = new ActiveRosterDBEntities())
      {
        List<tbl_Team> teams = context.tbl_Team.ToList();
        foreach (tbl_Team team in teams)
        {
          List<tbl_Player> players = context.tbl_Player.Where(p => p.Team == team.Id).ToList();

          foreach (tbl_Player player in players)
          {
            context.tbl_Player.Remove(player);
          }

          context.tbl_Team.Remove(team);

          context.SaveChanges();
        }

        teams = new List<tbl_Team>();
        ListViewMenu.ItemsSource = teams;

        GridMain.Children.Clear();
      }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
