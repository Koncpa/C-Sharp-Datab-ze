using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoopProjektKoncitySipek
{
    public partial class DatabazeLeku : Form
    {

        private int id;
        private string name=string.Empty;
        private readonly string soubor = "databaze.csv";
        SqlConnection spojeni = new SqlConnection(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=DatabazeLeku;Integrated Security=True");
        public DatabazeLeku()
        {
            InitializeComponent();
            showIdLabel.Text = "";
            showNazevLabel.Text = "";
            showAtcLabel.Text = "";
            showUcinnaLatkaLabel.Text = "";
            showCestaPodaniLabel.Text = "";
        }

        private void tlacitkoImport_Click(object sender, EventArgs e) //pro import léků do sql databáze
        {
            try
            {
                using (SqlCommand prikaz = new SqlCommand("DELETE FROM Lek", spojeni))
                {
                    spojeni.Open();
                    prikaz.ExecuteNonQuery();
                }
                using (StreamReader sr = new StreamReader(soubor))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] rozdeleny = s.Split(';');
                        string nazev = rozdeleny[1];
                        string atc = rozdeleny[2];
                        string ucinnaLatka = rozdeleny[3];
                        string cestaPodani = rozdeleny[4];
                        using (SqlCommand prikaz = new SqlCommand("INSERT INTO Lek (Nazev, Atc, UcinnaLatka, CestaPodani) VALUES (@nazev, @atc, @ucinnaLatka, @cestaPodani)", spojeni))
                        {
                            prikaz.Parameters.AddWithValue("@nazev", nazev);
                            prikaz.Parameters.AddWithValue("@atc", atc);
                            prikaz.Parameters.AddWithValue("@ucinnaLatka", ucinnaLatka);
                            prikaz.Parameters.AddWithValue("@cestaPodani", cestaPodani);
                            prikaz.ExecuteNonQuery();
                        }
                    }
                    spojeni.Close();
                    
                }
                MessageBox.Show("Databáze úspěšně importována.", "Úspěch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Databázi se nepodařilo importovat.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void tlacitkoExport_Click(object sender, EventArgs e) //pro Export databaze do excelu
        {
            try
            {
                using (SqlCommand prikaz = new SqlCommand("SELECT Id, Nazev, Atc, UcinnaLatka, CestaPodani FROM Lek", spojeni))
                {
                    spojeni.Open();
                    using (StreamWriter sw = new StreamWriter(soubor))
                    {
                        SqlDataReader dataReader = prikaz.ExecuteReader();
                        while (dataReader.Read())
                        {
                            string[] hodnoty = { dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString() };
                            string radek = String.Join(";", hodnoty);
                            sw.WriteLine(radek);
                        }
                        
                    }
                    spojeni.Close();
                }
                MessageBox.Show("Databáze úspěšně exportována.", "Úspěch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Databázi se nepodařilo exportovat.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tlacitkoPridat_Click(object sender, EventArgs e) //pridani leku do databaze
        {

            string nazev = pridatNazevBox.Text;
            string atc = pridatAtcBox.Text;
            string ucinnaLatka = pridatUcinnaLatkaBox.Text;
            string  cestaPodani = pridatCestaPodaniBox.Text;
        




            if (pridatNazevBox.Text == "" && pridatAtcBox.Text == ""  && pridatUcinnaLatkaBox.Text == "" && pridatCestaPodaniBox.Text == "")
            {
                MessageBox.Show("Nezadal si žádné údaje", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (pridatNazevBox.Text == "" || pridatAtcBox.Text == "" || pridatUcinnaLatkaBox.Text == "" || pridatCestaPodaniBox.Text == "")
            {
                MessageBox.Show("Nějáké údaje si nezadal", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SqlCommand prikaz = new SqlCommand("INSERT INTO Lek (Nazev, Atc, UcinnaLatka, CestaPodani) VALUES (@nazev, @atc, @ucinnaLatka, @cestaPodani)", spojeni))
            {
                spojeni.Open();
               

                if (hledatJmena(nazev) == true)
                {
                    MessageBox.Show("Lék už je v databázi", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    pridatNazevBox.Clear();
                    pridatAtcBox.Clear();
                    pridatUcinnaLatkaBox.Clear();
                    pridatCestaPodaniBox.Clear();
                    spojeni.Close();

                    return;


                }

                else
                {

                    prikaz.Parameters.AddWithValue("@nazev", nazev);
                    prikaz.Parameters.AddWithValue("@atc", atc);
                    prikaz.Parameters.AddWithValue("@ucinnaLatka", ucinnaLatka);
                    prikaz.Parameters.AddWithValue("@cestaPodani", cestaPodani);
                    prikaz.ExecuteNonQuery();
                    spojeni.Close();
                    pridatNazevBox.Clear();
                    pridatAtcBox.Clear();
                    pridatUcinnaLatkaBox.Clear();
                    pridatCestaPodaniBox.Clear();
                }
            }
        }  



        private void tlacitkoNazevSmaz_Click(object sender, EventArgs e) //smazani leku z databaze
        {
            string nazev = smazNazevBox.Text;


            if (smazNazevBox.Text == "")
            {
                MessageBox.Show("Nezadal si žádný lék na smazání", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            spojeni.Open();
            if (hledatJmena(nazev) == true)
            {
                
                using (SqlCommand prikaz = new SqlCommand("DELETE FROM Lek WHERE Nazev=@nazev", spojeni))
                {
                    prikaz.Parameters.AddWithValue("@nazev", nazev);
                    prikaz.ExecuteNonQuery();
                    MessageBox.Show("Lék byl nalezen a smazán!");
                    spojeni.Close();
                    smazNazevBox.Clear();
                }


            }
            else
            {
                smazNazevBox.Clear();
                MessageBox.Show("Lék nebyl nalezen!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                spojeni.Close();

                return;

            }

          
        }



        private void tlacitkoNazevHledej_Click(object sender, EventArgs e) //hledani leku v databazi
        {
            showIdLabel.Text = string.Empty;
            showNazevLabel.Text = string.Empty;
            showAtcLabel.Text = string.Empty;
            showUcinnaLatkaLabel.Text = string.Empty;
            showCestaPodaniLabel.Text = string.Empty;
            string nazev = hledejNazevBox.Text;
            if (hledejNazevBox.Text == "")
            {
                MessageBox.Show("Nezadal si žádný lék na vyhledání", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SqlCommand prikaz = new SqlCommand("SELECT Id, Nazev, Atc, UcinnaLatka, CestaPodani FROM Lek WHERE Nazev=@nazev", spojeni))
            {
               
                spojeni.Open();
                if (hledatJmena(nazev) == false)
                {
                    MessageBox.Show("Žadný takový lék se zde nenachází!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    spojeni.Close();
                    hledejNazevBox.Clear();
                    return;
                }
                prikaz.Parameters.AddWithValue("@nazev", nazev);
                SqlDataReader dataReader = prikaz.ExecuteReader();
                while (dataReader.Read())
                {   

                    showIdLabel.Text = dataReader["Id"].ToString();
                    showNazevLabel.Text = dataReader["Nazev"].ToString();
                    showAtcLabel.Text = dataReader["Atc"].ToString();
                    showUcinnaLatkaLabel.Text = dataReader["UcinnaLatka"].ToString();
                    showCestaPodaniLabel.Text = dataReader["CestaPodani"].ToString();
                }
                spojeni.Close();
                hledejNazevBox.Clear();
            }
        }

        private void tlacitkoEdituj_Click(object sender, EventArgs e) //editace leku v databazi
        {
            string temp = string.Empty;



            if (textBoxNazevEditace.Text == temp)
            {
                MessageBox.Show("Zadejte název léku, který chcete editovat.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string nazev = textBoxEditujNazev.Text;
           

            using (SqlCommand kontrola = new SqlCommand("SELECT Id, Nazev, Atc, UcinnaLatka, CestaPodani FROM Lek WHERE Nazev=@nazev", spojeni))
            {
                spojeni.Open();
                kontrola.Parameters.AddWithValue("@nazev", textBoxNazevEditace.Text);
                SqlDataReader dataReader = kontrola.ExecuteReader();
                while (dataReader.Read())
                {



                    temp = dataReader["Nazev"].ToString();
                    if (temp == textBoxNazevEditace.Text)
                    {



                        name = Convert.ToString(dataReader["Nazev"]);
                        id = Convert.ToInt32(dataReader["Id"]);
                        textBoxEditujNazev.Text = dataReader["Nazev"].ToString();
                        textBoxATCEditace.Text = dataReader["Atc"].ToString();
                        textBoxUcinnaLatkaEditace.Text = dataReader["UcinnaLatka"].ToString();
                        textBoxCestaEditace.Text = dataReader["CestaPodani"].ToString();

                        


                    }




                }
                 if (temp != textBoxNazevEditace.Text)
                {
                    spojeni.Close();
                    MessageBox.Show("Hledané jméno není v databázi!" ,"Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }



                textBoxNazevEditace.Clear();
                dataReader.Close();
                spojeni.Close();



            }
        }
        private void buttonUlozit_Click(object sender, EventArgs e) //ulozeni editovaneho leku do databaze
        {   

            string nazev = textBoxEditujNazev.Text;
            string atc = textBoxATCEditace.Text;
            string ucinnaLatka = textBoxUcinnaLatkaEditace.Text;
            string cestaPodani = textBoxCestaEditace.Text;
            using (SqlCommand prikaz = new SqlCommand("UPDATE Lek SET Nazev=@nazev, Atc=@atc, UcinnaLatka=@ucinnaLatka, CestaPodani=@cestaPodani WHERE id=@Id", spojeni))
            {

          
                spojeni.Open();
              


                if (textBoxEditujNazev.Text == "" && textBoxATCEditace.Text == "" && textBoxUcinnaLatkaEditace.Text == "" && textBoxCestaEditace.Text == "")
                {
                    MessageBox.Show("Nezadal si žádné údaje!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    spojeni.Close();
                    return;
                }


                if (textBoxEditujNazev.Text == "" || textBoxATCEditace.Text == "" || textBoxUcinnaLatkaEditace.Text == "" || textBoxCestaEditace.Text == "")
                {
                    MessageBox.Show("Nějáké údaje si nezadal!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    spojeni.Close();
                    return;
                }
                

                else if (hledatJmena(nazev) == true)
                {
                    spojeni.Close();
                    MessageBox.Show("Tento název je už v databázi,zkus něco jiného!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxEditujNazev.Clear();
                    return;
                }
                else
                {
                    if (id == 0)
                    {
                        spojeni.Close();
                        MessageBox.Show("Musíš dát nejdříve editaci!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxEditujNazev.Clear();
                        textBoxATCEditace.Clear();
                        textBoxUcinnaLatkaEditace.Clear();
                        textBoxCestaEditace.Clear();
                        return;

                    }
                    prikaz.Parameters.AddWithValue("@id", id);
                    prikaz.Parameters.AddWithValue("@nazev", nazev);
                    prikaz.Parameters.AddWithValue("@atc", atc);
                    prikaz.Parameters.AddWithValue("@ucinnaLatka", ucinnaLatka);
                    prikaz.Parameters.AddWithValue("@cestaPodani", cestaPodani);
                    prikaz.ExecuteNonQuery();
                    spojeni.Close();
                    textBoxEditujNazev.Clear();
                    textBoxATCEditace.Clear();
                    textBoxUcinnaLatkaEditace.Clear();
                    textBoxCestaEditace.Clear();
                    id = 0;
                }
            }

        }

        public Boolean hledatJmena(string nazev)// pro funkci tlacitka ulozit a pridat aby se neduplikoval kod
        {
            
            string temp = string.Empty;
           
            using (SqlCommand prikaz = new SqlCommand("SELECT Id, Nazev, Atc, UcinnaLatka, CestaPodani FROM Lek WHERE Nazev=@nazev", spojeni))
            {
                
                prikaz.Parameters.AddWithValue("@nazev", nazev);
                SqlDataReader dataReader = prikaz.ExecuteReader();
                while (dataReader.Read())
                {

                    temp = dataReader["Nazev"].ToString();

                }
                dataReader.Close();

                if (temp == nazev)
                {
                    if (temp == name)
                    {
                        name = string.Empty;
                        return false;
                    }
                    
                
                    return true;


                }
                return false;
            }
        }



    }

}