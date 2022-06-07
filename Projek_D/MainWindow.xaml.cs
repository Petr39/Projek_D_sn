using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Projek_D
{

    public partial class MainWindow : Window
    {
        //MainWindow go...
        public MainWindow()
        {
            InitializeComponent();
            LoadTime();
            LoadDatagrid();
            DeleteItems();
        }
        //Spojení se serverem
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-9FN2S5BG;Initial Catalog=Databaze_WPF_Sql;Integrated Security=True");
        //Chytit myší okno a pohybovat s ním
        private void HlavniOkno_main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        //Ukončení aplikace
        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Opravdu chceš ukončit aplikaci?", "Pozor !", MessageBoxButton.OKCancel);
            //Podmínka ukončení aplikce
            if (result == MessageBoxResult.OK)
            {
                App.Current.Shutdown();
            }
            return;
        }
        //Metoda reálného času
        public void LoadTime()
        {
            DispatcherTimer time = new DispatcherTimer();
            time.Interval = TimeSpan.FromSeconds(1);
            time.Tick += TikTak;
            time.Start();
        }
        //Přepis hodin v labelu po změně podle metody LoadTime()
        private void TikTak(object sender, EventArgs e)
        {
            this.Time_txt.Content = DateTime.Now.ToString(@"HH\:mm\:ss");
        }
        //Vložení obrázku
        private void VlozitObrazek_btn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "Vyber si obrázek";
            op.Filter = "All files(*.bmp;-.jpeg;*.jpg)|*.bmp;-.jpeg;*.jpg";
            op.ShowDialog();
            Uri zdroj = new Uri(op.FileName);
            this.Obrazek_img.Source = new BitmapImage(zdroj);
        }
        //Načtení dat ze serveru
        public void LoadDatagrid()
        {
            SqlCommand comm = new SqlCommand("select * from Stul_98", con);
            con.Open();
            SqlDataReader read = comm.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(read);
            con.Close();
            this.Datagrid_dt.ItemsSource = dt.DefaultView;




        }
        //Vložit data
        private void VloziData_btn_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand comm = new SqlCommand("insert into Stul_98 values (@Příjmení,@Jméno,@Věk,@Obrázek)", con);
            con.Open();
            comm.CommandType = CommandType.Text;
            comm.Parameters.AddWithValue("@Příjmení", this.Prijmeni_txt.Text);
            comm.Parameters.AddWithValue("@Jméno", this.Jmeno_txt.Text);
            comm.Parameters.AddWithValue("@Věk", this.Vek_txt.Text);
            comm.Parameters.AddWithValue("@Obrázek", this.Obrazek_img.Source.ToString());
            comm.ExecuteNonQuery();
            con.Close();
            LoadDatagrid();
        }
        //Smazání hlaviček tlačítko
        private void SmazatHlavicky_btn_Click(object sender, RoutedEventArgs e)
        {
            DeleteItems();
        }
        //Metoda smazání hlaviček
        public void DeleteItems()
        {
            this.Prijmeni_txt.Clear();
            this.Jmeno_txt.Clear();
            this.Vek_txt.Clear();
            this.Id_txt.Clear();
            this.Obrazek_img.Source = null;
            this.Prijmeni_txt.Focus();


        }
        //Upravit data tlačítko
        private void Upravit_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsValid())
                {

                    SqlCommand comm = new SqlCommand("Update Stul_98 set " +
                     "Příjmení ='" + Prijmeni_txt.Text + "'," +
                     "Jméno ='" + Jmeno_txt.Text + "', " +
                     "Věk='" + Vek_txt.Text + "'," +
                     "Obrázek='" + Obrazek_img.Source.ToString() + "' where id='" + Id_txt.Text + "'", con);
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@Příjmení", Prijmeni_txt.Text);
                    comm.Parameters.AddWithValue("@Jméno", Jmeno_txt.Text);
                    comm.Parameters.AddWithValue("@Věk", Vek_txt.Text);
                    comm.Parameters.AddWithValue("@Obrázek", Obrazek_img.Source.ToString());
                    con.Open();
                    comm.ExecuteNonQuery();
                    con.Close();
                    LoadDatagrid();

                }

                return;
                
            }
            catch (Exception)
            {

                MessageBox.Show("Zadej správně data");
                return;
            }


        }
        //Smazání dat z databáze
        private void SmazatData_btn_Click(object sender, RoutedEventArgs e)
        {            
            SqlCommand comm = new SqlCommand("Delete from Stul_98 where Id=" + Id_txt.Text + " ", con);
            comm.CommandType = CommandType.Text;
            con.Open();
            comm.ExecuteNonQuery();
            con.Close();
            LoadDatagrid();
        }
        //Tisk a uložení dat do PDF
        private void UlozitPdf_btn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog prinn = new PrintDialog();            
            if (prinn.ShowDialog() == true)
            {
                
                prinn.PrintVisual(Datagrid_dt, "ahoj");
            }
            return;
        }
        //Zapsané hodnoty jsou ověřeny
        public bool IsValid()
        {  
            if (this.Prijmeni_txt.Text == string.Empty && this.Jmeno_txt.Text == string.Empty && this.Vek_txt.Text==string.Empty && this.Obrazek_img.Source==null) 
            {
                MessageBox.Show("Zadej hodnoty píčo");
                return false;
            }
            return true;
        }
    }

}
