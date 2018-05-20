using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muhendislik_4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dosyaYükleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();//dosya almak için filedialog nesnesi
            op.Filter = "Csv Dosyaları |*.csv"; //sadece csv dosyalarını seçmesi için filtreleme işlemi
            op.Multiselect = true;  //çoklu seçim özelliği
            if (op.ShowDialog() == DialogResult.OK)//openfilediaglog penceresni açar ve aç diyince
            {
                string[] dosyaYollari = op.FileNames;//dosya yolları ve 
                string[] dosyaAdlari = op.SafeFileNames;    //isimlerini alır
                for(var i = 0; i < dosyaYollari.Length; i++)//dosya yolları üzerinde dönerek verileri okur
                {
                    string dosyaYolu = dosyaYollari[i];//dosya yolunu alır
                    DataTable dt = new DataTable();//data table nesnesi oluşturulur
                    string[] lines = System.IO.File.ReadAllLines(dosyaYolu);//tüm satırla io nesnesi ile okunur
                    if (lines.Length > 0)//eğer boş değilse
                    {
                        string firsLine = lines[0];//ilk satır alınır
                        string[] headerLabels = firsLine.Split(',');//, ile split edilir
                        int j = 0;//Header Oluştur
                        foreach(string headerWord in headerLabels)
                        {//header nesnesi oluşturulur
                            dt.Columns.Add(new DataColumn(j.ToString()));
                            j++;
                        }

                        
                        for (int r = 0; r < lines.Length; r++)
                        {//data içerisinde gez datayı oluştur
                            string[] dataWords = lines[r].Split(',');
                            DataRow dr = dt.NewRow();
                            int index = 0, index1=0,index2=0;
                            foreach (string headerWord in headerLabels)
                            {
                                if (dosyaAdlari[i].Equals("ogrenciProfil.csv") && index != 0)
                                {//, ile ayrılan datalar ogrenciprofil ise puanları 10 da 1 oranında düşür
                                    //bias hesaplarken double değişkeninin boyutu yetmemektedir o yüzden bu yola başvuruldu
                                    double d1 = Convert.ToDouble(dataWords[index]) * 0.1;
                                    //double d1 = Convert.ToDouble(dataWords[index]);
                                    dr[index++] = d1.ToString();
                                    index1 = index;
                                }
                                else
                                {
                                    dr[index++] = dataWords[index1++];
                                }

                            }
                            //okunan satırlar datatable a atılır
                            dt.Rows.Add(dr);
                           
                        }

                    }
                    if (dt.Rows.Count >0 && dosyaAdlari[i].Equals("ogrenciProfil.csv"))
                    {//eğer okunan dosya ne ise ilgili datagridview atılır
                        DataProfilcsv.DataSource = dt;
                    }
                    if (dt.Rows.Count > 0 && dosyaAdlari[i].Equals("ogrenciNetwork.csv"))
                    {
                        DataNetworkcsv.DataSource = dt;
                    } 
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {//arkadaş bul butonuna basıldığında
            int sonuc = 0;
            for(int k = 0; k < DataNetworkcsv.Rows.Count-1; k++)
            {//datagrid view üzerinde dönerek textbox içerisine yazılan numarayı arar 
                //buluduğunda yeniArkadasMatrisi fonksiyonuna gider
                if (DataNetworkcsv.Rows[k].Cells[0].Value.ToString().Equals(Arama.Text))
                {
                    yeniArkdasMatrisi(k);
                    sonuc = 1;
                    break;
                }
            }
            if (sonuc ==0)
            {//bulamaz ise hata mesajı döndürür
                MessageBox.Show(Arama.Text + " Nolu Öğrenci Bulunamadı!!");
            }
        }
        public void yeniArkdasMatrisi(int rowIndex)
        {
            ArrayList arkadasRow = new ArrayList();
            for( var i = 0; i< DataNetworkcsv.Rows[rowIndex].Cells.Count; i++)
            {//data netword üzerinde dönülerek
                string str = DataNetworkcsv.Rows[rowIndex].Cells[i].Value.ToString();
                for(int k = 0; k< DataProfilcsv.Rows.Count - 1; k++)
                {//data profil üzerinde dönülür
                    string str1 = DataProfilcsv.Rows[k].Cells[0].Value.ToString();
                    if (str.Equals(str1) && !str.Equals(Arama.Text))
                    {//network üzerinde eşlesen kayıt olursa kendisi hariç aradasrow arraylistine atılır
                        arkadasRow.Add(k);
                    }
                }
            }
            DataTable yeniData = new DataTable();
            for(int k = 0; k < 17; k++)
            {//yeni data oluşturulur
                yeniData.Columns.Add(new DataColumn(k.ToString()));
            }
            int deneme;
            foreach (int index in arkadasRow)
            {//ilk satırlara kişinin arkadaşları gelmektedir
                DataRow row = yeniData.NewRow();
                for(var i = 0; i< DataProfilcsv.Rows[index].Cells.Count; i++)
                {
                    row[i] = DataProfilcsv.Rows[index].Cells[i].Value.ToString();
                }
                row[16] = "1";//en son column 1 olarak atanır
                yeniData.Rows.Add(row);//data table ekleme yapılır
            }
            for(var i = 0; i< DataProfilcsv.Rows.Count-1; i++)
            {//data profil üzerinde gezilerek geri kalan kişileri datatable a ekleme yapılır
                int index = arkadasRow.IndexOf(i);
                if (index == -1)
                {
                    DataRow row = yeniData.NewRow();
                    if (!DataProfilcsv.Rows[i].Cells[0].Value.ToString().Equals(Arama.Text))
                    {
                        for (int j = 0; j < DataProfilcsv.Rows[i].Cells.Count; j++)
                        {
                            row[j] = DataProfilcsv.Rows[i].Cells[j].Value.ToString();
                        }
                        row[16] = "0";//arkadaş olmadıkları için 0 atanır column
                        yeniData.Rows.Add(row);//satırlar datatable a eklenir
                        deneme = yeniData.Rows.Count;//debug için
                    }
                }
            }
            //kenidis hariç olduğu için 90 data değil 89 adet kayıt mevcuttur. 
            //DataProfilcsv.DataSource = yeniData;
            Form2 frm = new Form2();
            frm.veri = yeniData;//yeni data form2 ye aktarılır
            frm.arkadasSayisi = arkadasRow.Count;//arkadaş sayısı diğer forma aktarılır
            frm.ShowDialog();
        }
    }
}
